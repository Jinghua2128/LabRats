using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string hubSceneName = "HubScene";

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
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterActiveLab(ILab lab)
    {
        activeLab = lab;
        activeLab.BeginLab();
    }

    public void ExitLabToHub()
    {
        activeLab?.SaveAndClose();
        activeLab = null;
        SceneManager.LoadScene(hubSceneName);
    }

    private void OnApplicationQuit()
    {
        activeLab?.SaveAndClose();
    }
}