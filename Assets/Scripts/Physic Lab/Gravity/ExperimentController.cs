using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ExperimentController : MonoBehaviour
{
    [Header("UI")]
    public Slider gravitySlider;
    public Slider heightSlider;

    [Header("Display")]
    public TextMeshProUGUI dataText;

    [Header("Object Prefabs")]
    public GameObject lightObjectPrefab;
    public GameObject mediumObjectPrefab;
    public GameObject heavyObjectPrefab;
    public Transform spawnHeight;

    [Header("Ground")]
    public GameObject ground; // Drag your ground object here

    private float startTime;
    private bool experimentRunning;
    private string logText = "";
    private List<GameObject> spawnedObjects;
    private List<float> objectMasses;
    private List<bool> objectsLanded;
    private List<float> landingTimes;

    public System.Action<ExperimentData> OnExperimentComplete;

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        objectMasses = new List<float>();
        objectsLanded = new List<bool>();
        landingTimes = new List<float>();
    }

    void Update()
    {
        ApplyGravity();

        if (experimentRunning)
        {
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

    void ApplyGravity()
    {
        Physics.gravity = new Vector3(0f, gravitySlider.value, 0f);
    }

    void InstantiateObjects()
    {
        objectsLanded.Clear();
        objectMasses.Clear();
        landingTimes.Clear();

        GameObject[] prefabs = { lightObjectPrefab, mediumObjectPrefab, heavyObjectPrefab };
        float[] masses = { 0.5f, 2f, 5f };
        string[] names = { "Light", "Medium", "Heavy" };
        
        for (int i = 0; i < 3; i++)
        {
            float dropHeight = heightSlider.value;
            Vector3 position = spawnHeight.position +
                Vector3.up * dropHeight +
                Vector3.right * (i - 1) * 2f;

            GameObject newObj = Instantiate(prefabs[i], position, Quaternion.identity);
            newObj.name = $"{names[i]}_{masses[i]}kg_{i}"; // Add index to name
            
            Rigidbody rb = newObj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = newObj.AddComponent<Rigidbody>();
            }
            
            rb.mass = masses[i];
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Add collider if missing
            if (newObj.GetComponent<Collider>() == null)
            {
                newObj.AddComponent<SphereCollider>();
            }

            // Add collision detection script inline
            CollisionDetector detector = newObj.AddComponent<CollisionDetector>();
            detector.controller = this;
            detector.ground = ground;

            spawnedObjects.Add(newObj);
            objectMasses.Add(masses[i]);
            objectsLanded.Add(false);
            landingTimes.Add(0f);
        }
    }

    // Called when an object hits the ground
    public void ObjectHitGround(GameObject obj)
    {
        // Find which object this is
        int index = -1;
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == obj)
            {
                index = i;
                break;
            }
        }

        if (index == -1 || objectsLanded[index]) return;

        // STOP TIMER for this object
        objectsLanded[index] = true;
        landingTimes[index] = Time.time - startTime;

        // Freeze the object
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"Object {index} ({objectMasses[index]}kg) LANDED at {landingTimes[index]:F3}s");
        
        UpdateLandingDisplay();
    }

    void DestroyAllObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }

    public void StartFall()
    {
        logText = "";
        DestroyAllObjects();
        InstantiateObjects();
        
        experimentRunning = true;
        startTime = Time.time;

        logText = "=== EXPERIMENT STARTED ===\n";
        logText += $"Gravity: {gravitySlider.value:F2} m/s²\n";
        logText += $"Height: {heightSlider.value:F2} m\n\n";
        logText += "Dropping 3 objects...\n";

        UpdatePanel();
        Debug.Log("Experiment started!");
    }

    void UpdateLandingDisplay()
    {
        string results = "\n--- RESULTS ---\n";
        
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (objectsLanded[i])
            {
                results += $"✓ Mass: {objectMasses[i]:F1} kg → Time: {landingTimes[i]:F3}s\n";
            }
            else
            {
                results += $"○ Mass: {objectMasses[i]:F1} kg → Falling...\n";
            }
        }
        
        logText = $"=== EXPERIMENT RUNNING ===\n";
        logText += $"Gravity: {gravitySlider.value:F2} m/s²\n";
        logText += $"Height: {heightSlider.value:F2} m\n";
        logText += results;
    }

    void EndExperiment()
    {
        experimentRunning = false;
        
        logText = "=== EXPERIMENT COMPLETE ===\n";
        logText += $"Gravity: {gravitySlider.value:F2} m/s²\n";
        logText += $"Height: {heightSlider.value:F2} m\n\n";
        
        logText += "--- FINAL RESULTS ---\n";
        
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            logText += $"Mass: {objectMasses[i]:F1} kg → {landingTimes[i]:F3}s\n";
        }

        float minTime = Mathf.Min(landingTimes.ToArray());
        float maxTime = Mathf.Max(landingTimes.ToArray());
        float difference = maxTime - minTime;
        
        logText += $"\nTime difference: {difference:F3}s";
        
        if (difference < 0.1f)
        {
            logText += "\n✓ All masses fell at same rate!\n";
            logText += "(Proving Galileo's theory)";
        }

        Debug.Log("Experiment complete!");

        ExperimentData data = new ExperimentData
        {
            gravity = gravitySlider.value,
            height = heightSlider.value,
            masses = new List<float>(objectMasses),
            fallTimes = new List<float>(landingTimes)
        };

        OnExperimentComplete?.Invoke(data);
    }

    void UpdatePanel()
    {
        string header = $"Gravity: {gravitySlider.value:F2} m/s²\n";
        header += $"Height: {heightSlider.value:F2} m\n\n";

        string status = experimentRunning ? 
            $"Elapsed: {(Time.time - startTime):F2}s\n" : 
            "Ready\n";

        dataText.text = header + status + logText;
    }

    public void ResetExperiment()
    {
        experimentRunning = false;
        logText = "";
        DestroyAllObjects();
    }

    void OnDestroy()
    {
        DestroyAllObjects();
    }
}

// Simple collision detector - part of the same file
public class CollisionDetector : MonoBehaviour
{
    [HideInInspector]
    public ExperimentController controller;
    [HideInInspector]
    public GameObject ground;
    
    private bool hasHit = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit && collision.gameObject == ground)
        {
            hasHit = true;
            controller.ObjectHitGround(gameObject);
            Debug.Log($"{gameObject.name} hit the ground!");
        }
    }
}

[System.Serializable]
public class ExperimentData
{
    public float gravity;
    public float height;
    public List<float> masses;
    public List<float> fallTimes;
}