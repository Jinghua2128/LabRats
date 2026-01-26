using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginPanelManager : MonoBehaviour
{

    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorText;

    public void Login()
    {
        // Obtain text from input fields
        var email = emailInput.text;
        var password = passwordInput.text;

        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        // Input validation
        if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
        {
            ShowError("Empty or invalid e-mail address");
            return;
        }

        // Password validation
        if (password.Length == 0)
        {
            ShowError("Please enter a password");
            return;
        }
        else if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters");
            return;
        }

        var createTask = FirebaseAuth.DefaultInstance
        .SignInWithEmailAndPasswordAsync(email, password);

        createTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.Exception != null) Debug.Log(task.Exception);
                ShowError("Error logging in");
                return;
            } else
            {
                Debug.Log("User logged in successfully");
            }
        });

        //This method displays an error message on the screen by putting text into a UI text element.
        void ShowError(string error)
        {
            errorText.text = error;
        }
    }
}
