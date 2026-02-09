using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;

public class SignupPanelManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;

    // Optional inline error text (can be null)
    [SerializeField] private TMP_Text inlineErrorText;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Signup()
    {
        if (emailInput == null || passwordInput == null || confirmPasswordInput == null)
        {
            ShowError("Signup UI not configured correctly.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // --- Validation ---
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

        // --- Firebase signup ---
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    ShowError("Signup failed: " + GetFirebaseErrorMessage(task.Exception));
                    return;
                }

                FirebaseUser user = task.Result.User;

                // Write profile (email only)
                FirebaseDatabase.DefaultInstance.RootReference
                    .Child("Users")
                    .Child(user.UserId)
                    .Child("Profile")
                    .Child("Email")
                    .SetValueAsync(email);

                ClearFields();

                // After signup → Login panel
                UIManager.Instance?.ShowLogin();
            });
    }

    // -------- Helpers --------

    private void ShowError(string message)
    {
        ErrorPanelManager.Instance?.ShowError(message);

        if (inlineErrorText != null)
            inlineErrorText.text = message;
    }

    private string GetFirebaseErrorMessage(System.AggregateException ex)
    {
        if (ex == null) return "Unknown error.";

        foreach (var inner in ex.InnerExceptions)
        {
            if (!string.IsNullOrEmpty(inner.Message))
                return inner.Message;
        }

        return ex.Message;
    }

    private void ClearFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
        if (inlineErrorText != null) inlineErrorText.text = "";
    }
}