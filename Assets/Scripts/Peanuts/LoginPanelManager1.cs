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

    [Header("Game Scene")]
    [SerializeField] private string gameSceneName = "02_PhysLab";

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
                    // Log full exception for debugging
                    Debug.LogError("[Login] Firebase Error:\n" + task.Exception);

                    // Show Firebase's message (even if generic)
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

                DatabaseManager.Instance.SetCurrentUserId(user.UserId);

                ClearFields();
                SceneManager.LoadScene(gameSceneName);
            });
    }

    private void ShowError(string message)
    {
        ErrorPanelManager.Instance?.ShowError(message);
    }

    private void ClearFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }
}