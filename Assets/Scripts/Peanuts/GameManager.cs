using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ---------- Scene Names ----------

    [SerializeField] private string loginScene = "00_Login";
    [SerializeField] private string mainAreaScene = "01_MainArea";
    [SerializeField] private string physLabScene = "02_PhysLab";
    [SerializeField] private string chemLabScene = "03_ChemLab";

    private ILab activeLab;

    private void Awake()
    {
        // Singleton that persists across scenes
        if (Instance != null && Instance != this)
            return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- Logout ----------

    public void Logout()
    {
        // Saves any active lab before leaving
        activeLab?.SaveAndClose();
        activeLab = null;

        // Logs out from Firebase Auth
        FirebaseAuth.DefaultInstance.SignOut();

        // Clears user context from DatabaseManager
        DatabaseManager.Instance?.ClearCurrentUser();

        LoadLogin();
    }

    // ---------- Lab Coordination ----------

    public void RegisterActiveLab(ILab lab)
    {
        // Tracks current lab so it can be saved on exit/logout
        activeLab = lab;
        activeLab?.BeginLab();
    }

    public void ExitLabToMainArea()
    {
        // Saves lab progress before returning to main area
        activeLab?.SaveAndClose();
        activeLab = null;

        LoadMainArea();
    }

    private void OnApplicationQuit()
    {
        // Ensures lab data saves if the app closes
        activeLab?.SaveAndClose();
    }

    // ---------- Scene Flow ----------

    public void LoadLogin()
    {
        // Ensures lab save happens before returning to login
        activeLab?.SaveAndClose();
        activeLab = null;

        SceneManager.LoadScene(loginScene);
    }

    public void LoadMainArea()
    {
        SceneManager.LoadScene(mainAreaScene);
    }

    public void LoadPhysLab()
    {
        SceneManager.LoadScene(physLabScene);
    }

    public void LoadChemLab()
    {
        SceneManager.LoadScene(chemLabScene);
    }
}