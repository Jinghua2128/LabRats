using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string loginScene = "00_Login";
    [SerializeField] private string mainAreaScene = "01_MainArea";
    [SerializeField] private string physLabScene = "02_PhysLab";
    [SerializeField] private string chemLabScene = "03_ChemLab";

    private ILab activeLab;
    [Header("Game Scene")]
    [SerializeField] private string MainArea = "01_MainArea";
    [SerializeField] private string PhysLab = "02_PhysLab";
    [SerializeField] private string ChemLab = "03_ChemLab";
    public void LoadPhysLab(string PhysLab)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(PhysLab);
    }
    public void LoadChemLab(string ChemLab)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ChemLab);
    }
    public void LoadMainArea(string MainArea)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainArea);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

        public void Logout()
    {
        activeLab?.SaveAndClose();
        activeLab = null;

        FirebaseAuth.DefaultInstance.SignOut();

        DatabaseManager.Instance?.ClearCurrentUser();

        LoadLogin();
    }

    // -------------------------
    // Lab Coordination
    // -------------------------
    public void RegisterActiveLab(ILab lab)
    {
        activeLab = lab;
        activeLab?.BeginLab();
    }

    public void ExitLabToMainArea()
    {
        activeLab?.SaveAndClose();
        activeLab = null;

        LoadMainArea();
    }

    private void OnApplicationQuit()
    {
        activeLab?.SaveAndClose();
    }

    // -------------------------
    // Scene Flow
    // -------------------------
    public void LoadLogin()
    {
        // Leaving any lab? Save first.
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