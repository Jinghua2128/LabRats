using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ExperimentController : MonoBehaviour
{
    [Header("UI")]
    public Slider gravitySlider;  // Set range: Min = -20, Max = 0, Value = -9.81
    public Slider massSlider;     // Set range: Min = 0.1, Max = 10, Value = 1
    public Slider distanceSlider; // Set range: Min = 1, Max = 20, Value = 10

    [Header("Display")]
    public TextMeshProUGUI dataText;

    [Header("Object Prefab")]
    public GameObject objectPrefab; // Drag your ball/cube prefab here
    public int numberOfObjects = 3; // How many objects to spawn
    public Transform spawnHeight;

    [Header("Ground Detection")]
    public float groundHeight = 0f; // Y position of ground
    public float detectionThreshold = 0.1f; // How close to ground = "landed"

    private float startTime;
    private bool experimentRunning;
    private string logText = "";
    private List<bool> objectsLanded;
    private List<GameObject> spawnedObjects; // Track spawned objects

    void Start()
    {
        objectsLanded = new List<bool>();
        spawnedObjects = new List<GameObject>();
    }

    void Update()
    {
        ApplySliders();

        if (experimentRunning)
        {
            CheckForLandings();
            
            // Check if all objects have landed
            bool allLanded = true;
            foreach (bool landed in objectsLanded)
            {
                if (!landed)
                {
                    allLanded = false;
                    break;
                }
            }

            if (allLanded)
            {
                EndExperiment();
            }
        }

        UpdatePanel();
    }

    void ApplySliders()
    {
        // Gravity should be negative for downward direction
        Physics.gravity = new Vector3(0f, gravitySlider.value, 0f);

        // Apply mass to all spawned objects
        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = massSlider.value;
            }
        }
    }

    void InstantiateObjects()
    {
        // Clear lists
        objectsLanded.Clear();
        
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Calculate position in a row
            Vector3 position = spawnHeight.position +
                transform.forward * distanceSlider.value +
                Vector3.right * i * 1.5f; // Spacing between objects

            // Instantiate the object
            GameObject newObj = Instantiate(objectPrefab, position, Quaternion.identity);
            newObj.name = $"ExperimentObject_{i}";
            
            // Make sure it has a Rigidbody
            Rigidbody rb = newObj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = newObj.AddComponent<Rigidbody>();
            }
            
            // Set initial properties
            rb.mass = massSlider.value;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Add to lists
            spawnedObjects.Add(newObj);
            objectsLanded.Add(false);
        }
    }

    void DestroyAllObjects()
    {
        // Destroy all spawned objects
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        // Clear the list
        spawnedObjects.Clear();
        objectsLanded.Clear();
    }

    public void StartFall()
    {
        // Clear previous log
        logText = "";
        
        // Destroy old objects and create new ones
        DestroyAllObjects();
        InstantiateObjects();
        
        experimentRunning = true;
        startTime = Time.timeSinceLevelLoad;

        logText += "--- New Experiment ---\n";
        logText += $"Settings: G={gravitySlider.value:F2} m/s², Mass={massSlider.value:F2} kg\n";

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject obj = spawnedObjects[i];
            if (obj == null) continue;

            float height = obj.transform.position.y - groundHeight;
            logText += $"\nObject {i}: Dropped from height {height:F2}m";
        }

        UpdatePanel();
    }

    void CheckForLandings()
    {
        float currentTime = Time.timeSinceLevelLoad - startTime;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            GameObject obj = spawnedObjects[i];
            if (obj == null || objectsLanded[i]) continue;

            // Check if object reached ground
            if (obj.transform.position.y <= groundHeight + detectionThreshold)
            {
                objectsLanded[i] = true;
                float fallTime = currentTime;
                
                logText += $"\nObject {i}: Landed at {fallTime:F2}s";
                Debug.Log($"Object {i} landed at {fallTime:F2}s");
            }
        }
    }

    void EndExperiment()
    {
        experimentRunning = false;
        float totalTime = Time.timeSinceLevelLoad - startTime;
        logText += $"\n\nExperiment completed in {totalTime:F2}s";
        Debug.Log($"Experiment completed in {totalTime:F2}s");
    }

    void UpdatePanel()
    {
        string header =
            $"Gravity: {gravitySlider.value:F2} m/s²\n" +
            $"Mass: {massSlider.value:F2} kg\n" +
            $"Distance: {distanceSlider.value:F2} m\n";

        string status = experimentRunning ? 
            $"Time: {(Time.timeSinceLevelLoad - startTime):F2}s\n" : 
            "Ready\n";

        dataText.text = header + status + "\n" + logText;
    }

    // Reset button
    public void ResetExperiment()
    {
        experimentRunning = false;
        logText = "";
        DestroyAllObjects();
    }

    // Clean up when script is destroyed
    void OnDestroy()
    {
        DestroyAllObjects();
    }
}