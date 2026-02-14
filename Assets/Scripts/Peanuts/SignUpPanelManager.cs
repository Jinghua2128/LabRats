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

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    ShowError("Signup failed:\n" + GetFirebaseErrorMessage(task.Exception));
                    return;
                }

                FirebaseUser user = task.Result.User;

                // Save Profile (email only)
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

    private void ShowError(string message)
    {
        ErrorPanelManager.Instance?.ShowError(message);
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
    }
}
