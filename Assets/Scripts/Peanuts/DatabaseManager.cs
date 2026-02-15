using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private DatabaseReference rootRef;
    private string currentUserId;

    public bool HasUser => !string.IsNullOrEmpty(currentUserId);

    private void Awake()
    {
        // Singleton that persists across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        TryInitRootRef();
    }

    // ---------- Initialization ----------

    private bool TryInitRootRef()
    {
        if (rootRef != null) return true;

        try
        {
            // Gets the root reference for Firebase Realtime Database
            rootRef = FirebaseDatabase.DefaultInstance.RootReference;
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ---------- User Context ----------

    public void SetCurrentUserId(string userId)
    {
        // Stores user ID so future saves go to the correct user node
        if (string.IsNullOrEmpty(userId)) return;
        currentUserId = userId;
    }

    public void ClearCurrentUser()
    {
        // Clears user ID on logout
        currentUserId = null;
    }

    // ---------- Lab Saving ----------

    public Task SetLabFieldPath(string relativePath, string fieldName, object value)
    {
        // Prevents saving when no user is logged in
        if (!HasUser || !TryInitRootRef())
            return Task.CompletedTask;

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

            if (!string.IsNullOrEmpty(fieldName))
                r = r.Child(fieldName);

            // Saves a single field to Firebase
            return r.SetValueAsync(value);
        }
        catch
        {
            return Task.CompletedTask;
        }
    }

    public Task SetLabNode(string relativePath, object value)
    {
        // Prevents saving when no user is logged in
        if (!HasUser || !TryInitRootRef())
            return Task.CompletedTask;

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

            // Saves an entire node to Firebase
            return r.SetValueAsync(value);
        }
        catch
        {
            return Task.CompletedTask;
        }
    }
}