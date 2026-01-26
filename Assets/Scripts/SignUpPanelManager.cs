using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;

public class SignupPanelManager : MonoBehaviour
{

    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text errorText;

    public void Signup()
    {
        // Obtain text from input fields
        var email = emailInput.text;
        var password = passwordInput.text;

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
        .CreateUserWithEmailAndPasswordAsync(email, password);

        createTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.Exception != null) Debug.Log(task.Exception);
                ShowError("Error signing up");
                return;
            } else
            {
                Debug.Log("User signed up successfully");
            }
        });

        //This method displays an error message on the screen by putting text into a UI text element.
        void ShowError(string error)
        {
            errorText.text = error;
        }
    }
}
