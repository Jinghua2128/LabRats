using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button startGameButton;
    public Button settingsButton;
    public Button quitButton;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public Button backToMainButton;
    
    [Header("Audio")]
    public AudioClip menuMusic;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Play menu music
        if (menuMusic != null && audioSource != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Setup button listeners
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        if (backToMainButton != null)
            backToMainButton.onClick.AddListener(BackToMainMenu);
        
        // Initialize panels
        ShowMainMenu();
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("03_Street");
    }
    
    public void OpenSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }
    
    void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}