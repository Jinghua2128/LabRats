using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private DatabaseReference rootRef;
    private string currentUserId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        TryInitRootRef();
        Debug.Log("[DBM] Awake");
    }

    private bool TryInitRootRef()
    {
        if (rootRef != null) return true;

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

    // ---------- Generic Lab Writes ----------

    /// <summary>
    /// Writes to:
    /// Users/<uid>/Labs/<relativePath>/<fieldName> = value
    ///
    /// Example:
    /// relativePath = "GravityLab/Experiments/1"
    /// fieldName = "Duration"
    /// value = 2.5f
    /// </summary>
    public Task SetLabFieldPath(string relativePath, string fieldName, object value)
    {
        if (!HasUser)
        {
            Debug.LogWarning("[DBM] SetLabFieldPath called but no user is set.");
            return Task.CompletedTask;
        }

        if (!TryInitRootRef())
        {
            Debug.LogWarning("[DBM] SetLabFieldPath failed (no rootRef).");
            return Task.CompletedTask;
        }

        try
        {
            DatabaseReference r = rootRef
                .Child("Users")
                .Child(currentUserId)
                .Child("Labs");

            // Split "GravityLab/Experiments/1" into parts
            if (!string.IsNullOrEmpty(relativePath))
            {
                var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                    r = r.Child(p);
            }

            if (!string.IsNullOrEmpty(fieldName))
                r = r.Child(fieldName);

            return r.SetValueAsync(value);
        }
        catch (Exception e)
        {
            Debug.LogError("[DBM] SetLabFieldPath exception\n" + e);
            ErrorPanelManager.Instance?.ShowError("Failed to save lab data. Check console.");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Optional helper if you ever want to overwrite a whole node:
    /// Users/<uid>/Labs/<relativePath> = value
    /// </summary>
    public Task SetLabNode(string relativePath, object value)
    {
        if (!HasUser)
        {
            Debug.LogWarning("[DBM] SetLabNode called but no user is set.");
            return Task.CompletedTask;
        }

        if (!TryInitRootRef())
        {
            Debug.LogWarning("[DBM] SetLabNode failed (no rootRef).");
            return Task.CompletedTask;
        }

        try
        {
            DatabaseReference r = rootRef
                .Child("Users")
                .Child(currentUserId)
                .Child("Labs");

            if (!string.IsNullOrEmpty(relativePath))
            {
                var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                    r = r.Child(p);
            }

            return r.SetValueAsync(value);
        }
        catch (Exception e)
        {
            Debug.LogError("[DBM] SetLabNode exception\n" + e);
            ErrorPanelManager.Instance?.ShowError("Failed to save lab data. Check console.");
            return Task.CompletedTask;
        }
    }
}