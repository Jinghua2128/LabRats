/*
 * File: ExperimentController.cs
 * Project: LabRats - Physics Lab (Gravity)
 * Description: Controls gravity experiment simulation with falling objects of different masses.
 *              Manages experiment execution, timing, and data collection to demonstrate Galileo's principle.
 * 
 * Author: Liu GuangXuan
 * Organization: G²KM Studio
 * Copyright: © 2026 G²KM Studio. All rights reserved.
 * 
 * Created: 2026
 * Last Modified: 2026-02-15
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controls gravity experiments by managing falling objects and measuring their fall times.
/// Demonstrates that objects of different masses fall at the same rate in a vacuum (Galileo's principle).
/// </summary>
public class ExperimentController : MonoBehaviour
{
    /// <summary>
    /// UI Controls for experiment parameters.
    /// </summary>
    [Header("UI")]
    /// <summary>
    /// Slider to adjust gravity force (m/s²).
    /// </summary>
    public Slider gravitySlider;
    
    /// <summary>
    /// Slider to adjust drop height (meters).
    /// </summary>
    public Slider heightSlider;

    /// <summary>
    /// Display components for experiment data.
    /// </summary>
    [Header("Display")]
    /// <summary>
    /// Text display for experiment data and results.
    /// </summary>
    public TextMeshProUGUI dataText;

    /// <summary>
    /// Prefabs for falling objects with different masses.
    /// </summary>
    [Header("Object Prefabs")]
    /// <summary>
    /// Prefab for light object (0.5 kg).
    /// </summary>
    public GameObject lightObjectPrefab;
    
    /// <summary>
    /// Prefab for medium object (2 kg).
    /// </summary>
    public GameObject mediumObjectPrefab;
    
    /// <summary>
    /// Prefab for heavy object (5 kg).
    /// </summary>
    public GameObject heavyObjectPrefab;
    
    /// <summary>
    /// Transform marking the base spawn position for objects.
    /// </summary>
    public Transform spawnHeight;

    /// <summary>
    /// Ground reference for collision detection.
    /// </summary>
    [Header("Ground")]
    /// <summary>
    /// The ground GameObject that objects will collide with.
    /// </summary>
    public GameObject ground;

    private float startTime;
    private bool experimentRunning;
    private string logText = "";
    private List<GameObject> spawnedObjects;
    private List<float> objectMasses;
    private List<bool> objectsLanded;
    private List<float> landingTimes;

    /// <summary>
    /// Callback invoked when an experiment completes with all objects landed.
    /// </summary>
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

    /// <summary>
    /// Called when a falling object hits the ground. Records landing time and freezes the object.
    /// </summary>
    /// <param name="obj">The GameObject that hit the ground.</param>
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

    /// <summary>
    /// Starts a new gravity experiment by spawning objects and beginning the timer.
    /// </summary>
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

    /// <summary>
    /// Resets the experiment by stopping the current run and destroying all spawned objects.
    /// </summary>
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

/// <summary>
/// Detects collisions between falling objects and the ground.
/// Notifies the ExperimentController when an object lands.
/// </summary>
public class CollisionDetector : MonoBehaviour
{
    /// <summary>
    /// Reference to the ExperimentController managing this experiment.
    /// </summary>
    [HideInInspector]
    public ExperimentController controller;
    
    /// <summary>
    /// Reference to the ground GameObject to detect collisions with.
    /// </summary>
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

/// <summary>
/// Data structure containing results from a single gravity experiment.
/// </summary>
[System.Serializable]
public class ExperimentData
{
    /// <summary>
    /// The gravity value used in the experiment (m/s²).
    /// </summary>
    public float gravity;
    
    /// <summary>
    /// The drop height used in the experiment (meters).
    /// </summary>
    public float height;
    
    /// <summary>
    /// List of object masses used in the experiment (kg).
    /// </summary>
    public List<float> masses;
    
    /// <summary>
    /// List of fall times for each object (seconds).
    /// </summary>
    public List<float> fallTimes;
}