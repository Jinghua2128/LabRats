using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class LoginPanelManager : MonoBehaviour
{
    // ---------- UI References ----------

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    // ---------- Scene ----------

    [SerializeField] private string gameSceneName = "01_MainArea";

    private FirebaseAuth auth;

    private void Start()
    {
        // Gets Firebase Auth instance used for login
        auth = FirebaseAuth.DefaultInstance;
    }

    // ---------- Login ----------

    public void Login()
    {
        if (emailInput == null || passwordInput == null)
        {
            // Handles missing UI references
            ShowError("Login UI not configured correctly.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

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

        // Signs user into Firebase Auth
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    // Handles login errors and shows them in UI
                    string msg = task.Exception?.Flatten().InnerExceptions[0].Message;
                    ShowError("Login failed:\n" + msg);
                    return;
                }

                FirebaseUser user = task.Result.User;

                if (DatabaseManager.Instance == null)
                {
                    ShowError("Internal error: Database not ready.");
                    return;
                }

                // Stores the logged-in user ID for database operations
                DatabaseManager.Instance.SetCurrentUserId(user.UserId);

                ClearFields();
                SceneManager.LoadScene(gameSceneName);
            });
    }

    // ---------- Helpers ----------

    private void ShowError(string message)
    {
        ErrorPanelManager.Instance?.ShowError(message);
    }

    private void ClearFields()
    {
        // Clears input fields after successful login/ leaving the panel
        emailInput.text = "";
        passwordInput.text = "";
    }
}