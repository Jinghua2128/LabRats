using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;

public class SignupPanelManager : MonoBehaviour
{
    // ---------- UI References ----------

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // ---------- Signup ----------

    public void Signup()
    {
        // Validates input fields before attempting signup
        if (emailInput == null || passwordInput == null || confirmPasswordInput == null)
        {
            ShowError("Signup UI not configured correctly.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Basic validation of email and password fields
        if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
        {
            ShowError("Invalid e-mail address.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Please enter a password.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Passwords do not match.");
            return;
        }

        // Creates a new Firebase Auth account
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                // Handles signup errors and shows them in UI
                if (task.IsCanceled || task.IsFaulted)
                {
                    ShowError("Signup failed:\n" + GetFirebaseErrorMessage(task.Exception));
                    return;
                }

                FirebaseUser user = task.Result.User;

                // Saves basic profile info to Realtime Database
                FirebaseDatabase.DefaultInstance.RootReference
                    .Child("Users")
                    .Child(user.UserId)
                    .Child("Profile")
                    .Child("Email")
                    .SetValueAsync(email);

                ClearFields();

                // Returns to login screen after signup
                UIManager.Instance?.ShowLogin();
            });
    }

    // ---------- Helpers ----------

    private void ShowError(string message)
    {
        ErrorPanelManager.Instance?.ShowError(message);
    }

    private string GetFirebaseErrorMessage(System.AggregateException ex)
    {
        if (ex == null) return "Unknown error.";

        // Extracts the most relevant error message from Firebase exceptions
        foreach (var inner in ex.InnerExceptions)
        {
            if (!string.IsNullOrEmpty(inner.Message))
                return inner.Message;
        }

        return ex.Message;
    }

    private void ClearFields()
    {
        //clears input fields after successful signup/ leaving the panel
        emailInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }
}