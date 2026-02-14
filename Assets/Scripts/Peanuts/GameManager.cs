using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string hubSceneName = "HubScene";

    private ILab activeLab;

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