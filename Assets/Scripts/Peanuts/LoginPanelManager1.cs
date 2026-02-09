using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class LoginPanelManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    // Optional inline error text
    [SerializeField] private TMP_Text inlineErrorText;

    [Header("Game Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Login()
    {
        if (emailInput == null || passwordInput == null)
        {
            ShowError("Login UI not configured correctly.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

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

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    ShowError("Login failed: " + GetFirebaseErrorMessage(task.Exception));
                    return;
                }

                FirebaseUser user = task.Result.User;

                if (DatabaseManager.Instance == null)
                {
                    ShowError("Internal error: Database not ready.");
                    return;
                }

                DatabaseManager.Instance.SetCurrentUserId(user.UserId);

                ClearFields();
                SceneManager.LoadScene(gameSceneName);
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
        if (inlineErrorText != null) inlineErrorText.text = "";
    }
}