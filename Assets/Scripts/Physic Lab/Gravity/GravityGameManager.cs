using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// [ADDED]
using System;
using System.Reflection;

public class GravityLabGameManager : MonoBehaviour, ILab
{
    [Header("References")]
    public ExperimentController experimentController;
    public Slider gravitySlider;
    public Slider heightSlider;

    [Header("Progress Display")]
    public TextMeshProUGUI progressText;
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;

    [Header("Requirements")]
    public int requiredExperiments = 5; // Student must run 5 experiments
    public float minGravityChange = 5f; // Must change gravity by at least 5
    public float minHeightChange = 3f;  // Must change height by at least 3

    // Tracking
    private HashSet<string> gravityValuesUsed = new HashSet<string>();
    private HashSet<string> heightValuesUsed = new HashSet<string>();
    private int experimentsCompleted = 0;
    private List<ExperimentData> allExperimentData = new List<ExperimentData>();
    
    private bool hasChangedGravity = false;
    private bool hasChangedHeight = false;
    private bool hasRunMultipleExperiments = false;
    private float initialGravity;
    private float initialHeight;

    public string LabId => "GravityLab";

    [Header("Firebase Saving (Added)")]
    [SerializeField] private float liveSaveInterval = 3f;

    private float labStartTime;
    private float liveSaveTimer;
    private int experimentCounter = 0;

    void Start()
    {
        if (experimentController != null)
        {
            experimentController.OnExperimentComplete += OnExperimentCompleted;
        }

        // Track initial values
        initialGravity = gravitySlider.value;
        initialHeight = heightSlider.value;

        // Add slider listeners
        gravitySlider.onValueChanged.AddListener(OnGravityChanged);
        heightSlider.onValueChanged.AddListener(OnHeightChanged);

        if (completionPanel != null)
            completionPanel.SetActive(false);

        UpdateProgressDisplay();

        GameManager.Instance?.RegisterActiveLab(this);
    }

    private void Update()
    {
        liveSaveTimer += Time.deltaTime;
        if (liveSaveTimer >= liveSaveInterval)
        {
            liveSaveTimer = 0f;
            SaveLive();
        }
    }

    void OnGravityChanged(float value)
    {
        if (Mathf.Abs(value - initialGravity) >= minGravityChange)
        {
            hasChangedGravity = true;
            UpdateProgressDisplay();
        }
    }

    void OnHeightChanged(float value)
    {
        if (Mathf.Abs(value - initialHeight) >= minHeightChange)
        {
            hasChangedHeight = true;
            UpdateProgressDisplay();
        }
    }

    void OnExperimentCompleted(ExperimentData data)
    {
        experimentsCompleted++;
        allExperimentData.Add(data);

        // Track unique values used
        gravityValuesUsed.Add(data.gravity.ToString("F1"));
        heightValuesUsed.Add(data.height.ToString("F1"));

        if (experimentsCompleted >= 2)
        {
            hasRunMultipleExperiments = true;
        }

        UpdateProgressDisplay();
        CheckCompletion();

        SaveExperimentToFirebase(data);
    }

    void UpdateProgressDisplay()
    {
        if (progressText == null) return;

        string progress = "=== EXPERIMENT PROGRESS ===\n\n";

        progress += hasChangedGravity ? "✓" : "○";
        progress += $" Adjust gravity slider (by {minGravityChange}+)\n";

        progress += hasChangedHeight ? "✓" : "○";
        progress += $" Adjust height slider (by {minHeightChange}+)\n";

        progress += hasRunMultipleExperiments ? "✓" : "○";
        progress += $" Run {requiredExperiments} experiments ({experimentsCompleted}/{requiredExperiments})\n";

        progress += $"\n Different gravities used: {gravityValuesUsed.Count}";
        progress += $"\n Different heights used: {heightValuesUsed.Count}";

        progressText.text = progress;
    }

    void CheckCompletion()
    {
        bool allComplete = hasChangedGravity && 
                hasChangedHeight && 
                experimentsCompleted >= requiredExperiments;

        if (allComplete)
        {
            ShowCompletionScreen();
        }
    }

    void ShowCompletionScreen()
    {
        if (completionPanel != null)
            completionPanel.SetActive(true);

        if (completionText != null)
        {
            string summary = "EXPERIMENT COMPLETE!\n\n";
            summary += $"Total experiments: {experimentsCompleted}\n";
            summary += $"Gravity values tested: {gravityValuesUsed.Count}\n";
            summary += $"Heights tested: {heightValuesUsed.Count}\n\n";

            summary += "KEY DISCOVERY:\n";
            summary += "Objects of different masses fall at\n";
            summary += "the SAME RATE in a vacuum!\n\n";
            summary += "(This is Galileo's principle of\nfree fall acceleration)";

            completionText.text = summary;
        }

        Debug.Log("Student completed the gravity lab!");
    }

    public void RestartLab()
    {
        // Reset all tracking
        experimentsCompleted = 0;
        gravityValuesUsed.Clear();
        heightValuesUsed.Clear();
        allExperimentData.Clear();
        hasChangedGravity = false;
        hasChangedHeight = false;
        hasRunMultipleExperiments = false;

        if (completionPanel != null)
            completionPanel.SetActive(false);

        // Reset sliders
        gravitySlider.value = initialGravity;
        heightSlider.value = initialHeight;

        experimentController.ResetExperiment();
        UpdateProgressDisplay();

        BeginLab();
    }

    void OnDestroy()
    {
        if (experimentController != null)
        {
            experimentController.OnExperimentComplete -= OnExperimentCompleted;
        }
    }

    public void BeginLab()
    {
        labStartTime = Time.time;
        liveSaveTimer = 0f;
        experimentCounter = 0;

        // Initialize branch
        DatabaseManager.Instance?.SetLabFieldPath(LabId, "Time_Passed", 0);
    }

    public void SaveLive()
    {
        int seconds = Mathf.FloorToInt(Time.time - labStartTime);
        DatabaseManager.Instance?.SetLabFieldPath(LabId, "Time_Passed", seconds);
    }

    public void SaveAndClose()
    {
        SaveLive();
    }
    private void SaveExperimentToFirebase(ExperimentData data)
    {
        if (DatabaseManager.Instance == null) return;

        experimentCounter++;
        string expPath = $"{LabId}/Experiments/{experimentCounter}";

        DatabaseManager.Instance.SetLabFieldPath(expPath, "Gravity", data.gravity);
        DatabaseManager.Instance.SetLabFieldPath(expPath, "Distance", data.height);
        DatabaseManager.Instance.SetLabFieldPath(expPath, "Duration", data.fallTimes[0]);

        float seconds = Time.time - labStartTime;
        DatabaseManager.Instance.SetLabFieldPath(expPath, "RecordedAtSeconds", seconds);
    }
}