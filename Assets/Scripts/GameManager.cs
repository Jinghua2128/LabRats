using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game State")]
    public int score = 0;
    public int money = 100;
    public int reputation = 100;
    public float gameTime = 540f; // 9 minutes
    private float currentTime;
    private bool gameActive = true;
    
    [Header("NPC Management")]
    public int maxNPCsInStore = 8;
    public float npcSpawnInterval = 15f;
    private int currentNPCCount = 0;
    
    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text moneyText;
    public TMP_Text reputationText;
    public TMP_Text timerText;
    public TMP_Text alertText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text finalMoneyText;
    public TMP_Text finalReputationText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Audio")]
    public AudioClip groceryStoreMusic;
    public AudioClip alertSound;
    private AudioSource musicSource;
    [Header("Prefabs")]
    public GameObject[] shopperPrefabs;
    public GameObject[] shoplifterPrefabs;
    public GameObject[] distractorPrefabs;
    public Transform[] spawnPoints;
    public Transform exitPoint;
    private bool storeEntered = false;
    private Coroutine alertCoroutine;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // Setup audio sources
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            musicSource = sources[0];
        }
    }
    
    void Start()
    {
        currentTime = gameTime;
        UpdateUI();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    void Update()
    {
        if (!gameActive || !storeEntered) return;
        
        // Update timer 
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameOver("Shift is over.");
        }
        
        UpdateUI();
    }
    
    public void OnStoreEntered()
    {
        if (storeEntered) return;
        
        storeEntered = true;
        Debug.Log("Entered store - game started!");
        
        // Switch to grocery store music
        if (groceryStoreMusic != null && musicSource != null)
        {
            musicSource.clip = groceryStoreMusic;
            musicSource.Play();
        }
        
        // Start NPC spawning
        StartCoroutine(SpawnNPCsRoutine());
    }
    
    IEnumerator SpawnNPCsRoutine()
    {
        while (gameActive && storeEntered)
        {
            if (currentNPCCount < maxNPCsInStore)
            {
                SpawnRandomNPC();
            }
            yield return new WaitForSeconds(npcSpawnInterval);
        }
    }
    
    void SpawnRandomNPC()
    {
        if (spawnPoints.Length == 0) return;
        
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject npcPrefab = null;
        
        // 50% shoppers, 30% shoplifters, 20% distractors
        float rand = Random.Range(0f, 1f);
        if (rand < 0.5f && shopperPrefabs.Length > 0)
            npcPrefab = shopperPrefabs[Random.Range(0, shopperPrefabs.Length)];
        else if (rand < 0.8f && shoplifterPrefabs.Length > 0)
            npcPrefab = shoplifterPrefabs[Random.Range(0, shoplifterPrefabs.Length)];
        else if (distractorPrefabs.Length > 0)
            npcPrefab = distractorPrefabs[Random.Range(0, distractorPrefabs.Length)];
        
        if (npcPrefab != null)
        {
            Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
            currentNPCCount++;
        }
    }
    
    public void OnNPCDestroyed()
    {
        currentNPCCount--;
    }
    
    public void ArrestNPC(NPCType npcType, string npcName)
    {
        switch (npcType)
        {
            case NPCType.Shoplifter:
                score += 20;
                money += 20;
                ShowAlert($"Correct arrest! +20 score, +20 money", Color.green);
                break;
            case NPCType.Distractor:
                score += 10;
                ShowAlert($"Distractor arrested! +10 score", Color.yellow);
                break;
            case NPCType.NormalShopper:
                score -= 20;
                money -= 20;
                reputation -= 20;
                ShowAlert($"Wrong arrest! -20 score, -20 money, -20 reputation", Color.red);
                break;
        }
        
        // Check game over condition
        if (reputation < 50)
        {
            GameOver("Reputation too low!");
        }
        
        UpdateUI();
    }
    
    public void ItemStolen(string itemName)
    {
        ShowAlert($"{itemName} stolen!", Color.red);

        if (alertSound != null && musicSource != null)
            musicSource.PlayOneShot(alertSound);
    }
    
    void ShowAlert(string message, Color color)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.color = color;
            alertText.gameObject.SetActive(true);
            
            if (alertCoroutine != null)
                StopCoroutine(alertCoroutine);
            alertCoroutine = StartCoroutine(ClearAlertAfterDelay(3f));
        }
    }
    
    IEnumerator ClearAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (alertText != null)
        {
            alertText.text = "";
            alertText.gameObject.SetActive(false);
        }
        alertCoroutine = null;
    }
    
    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (moneyText != null) moneyText.text = $"Money: ${money}";
        if (reputationText != null) reputationText.text = $"Reputation: {reputation}";
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    void GameOver(string reason)
    {
        gameActive = false;
        Debug.Log($"Game Over: {reason}");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = $"Final Score: {score}";
            if (finalMoneyText != null) finalMoneyText.text = $"Final Money: ${money}";
            if (finalReputationText != null) finalReputationText.text = $"Final Reputation: {reputation}";
        }
        
        Time.timeScale = 0f; // Pause game
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("01_MainMenu");
    }
}