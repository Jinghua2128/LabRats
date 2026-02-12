using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GravityLabGameManager : MonoBehaviour
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

    void Start()
    {
        // Subscribe to experiment completion
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
    }

    void UpdateProgressDisplay()
    {
        if (progressText == null) return;

        string progress = "=== EXPERIMENT PROGRESS ===\n\n";

        // Objective 1: Change gravity
        progress += hasChangedGravity ? "✓" : "○";
        progress += $" Adjust gravity slider (by {minGravityChange}+)\n";

        // Objective 2: Change height
        progress += hasChangedHeight ? "✓" : "○";
        progress += $" Adjust height slider (by {minHeightChange}+)\n";

        // Objective 3: Run multiple experiments
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
            string summary = "🎉 EXPERIMENT COMPLETE! 🎉\n\n";
            summary += $"Total experiments: {experimentsCompleted}\n";
            summary += $"Gravity values tested: {gravityValuesUsed.Count}\n";
            summary += $"Heights tested: {heightValuesUsed.Count}\n\n";

            summary += "KEY DISCOVERY:\n";
            summary += "Objects of different masses fall at\n";
            summary += "the SAME RATE in a vacuum!\n\n";
            summary += "(This is Galileo's principle of\nfree fall acceleration)";

            completionText.text = summary;
        }

        Debug.Log("🎓 Student completed the gravity lab!");
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
    }

    void OnDestroy()
    {
        if (experimentController != null)
        {
            experimentController.OnExperimentComplete -= OnExperimentCompleted;
        }
    }
}