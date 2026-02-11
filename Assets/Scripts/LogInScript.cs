using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LogInScript : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI errorText;

    private string correctUser = "Hello";
    private string correctPass = "1234";

    public void TryLogin()
    {
        string user = usernameField.text;
        string pass = passwordField.text;

        if (user == correctUser && pass == correctPass)
        {
            SceneManager.LoadScene("ITD_WK6");
        }
        else
        {
            errorText.text = "Incorrect username or password.";
        }
    }
}