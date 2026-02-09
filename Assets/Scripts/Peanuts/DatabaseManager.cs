using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private DatabaseReference rootRef;

    private string currentUserId;
    private string currentLabId;
    private bool hasActiveLab = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[DBM] Awake");
    }

    // ---------- Helpers ----------

    private bool EnsureRootRef()
    {
        if (rootRef != null)
            return true;

        try
        {
            rootRef = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("[DBM] rootRef acquired");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[DBM] Failed to get rootRef\n" + e);
            return false;
        }
    }

    public bool HasUser => !string.IsNullOrEmpty(currentUserId);
    public bool HasActiveLab => hasActiveLab;

    // ---------- User ----------

    public void SetCurrentUserId(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("[DBM] SetCurrentUserId called with empty ID");
            return;
        }

        currentUserId = userId;
        Debug.Log("[DBM] User set: " + currentUserId);
    }

    // ---------- Labs ----------

    public void StartLab(string labId)
    {
        if (!EnsureRootRef() || !HasUser)
        {
            Debug.LogWarning("[DBM] Cannot start lab (no root/user)");
            return;
        }

        currentLabId = labId;
        hasActiveLab = true;

        var labRef = rootRef
            .Child("Users")
            .Child(currentUserId)
            .Child("Labs")
            .Child(labId);

        // Ensure base fields exist
        labRef.Child("Time_Passed").SetValueAsync("0");
        labRef.Child("Input_Text").SetValueAsync("");

        Debug.Log($"[DBM] Lab started: {labId}");
    }

    public void UpdateCurrentLab(int seconds, string inputText)
    {
        if (!hasActiveLab || !EnsureRootRef())
            return;

        var labRef = rootRef
            .Child("Users")
            .Child(currentUserId)
            .Child("Labs")
            .Child(currentLabId);

        labRef.Child("Time_Passed").SetValueAsync(seconds.ToString());
        labRef.Child("Input_Text").SetValueAsync(inputText ?? "");
    }

    // Generic field writer for lab-specific data
    public void SetLabField(string labId, string fieldName, string value)
    {
        if (!EnsureRootRef() || !HasUser)
            return;

        rootRef
            .Child("Users")
            .Child(currentUserId)
            .Child("Labs")
            .Child(labId)
            .Child(fieldName)
            .SetValueAsync(value);
    }

    public void EndCurrentLab(int finalSeconds, string finalInput)
    {
        if (!hasActiveLab)
            return;

        UpdateCurrentLab(finalSeconds, finalInput);

        Debug.Log($"[DBM] Lab ended: {currentLabId}");
        hasActiveLab = false;
        currentLabId = null;
    }
}