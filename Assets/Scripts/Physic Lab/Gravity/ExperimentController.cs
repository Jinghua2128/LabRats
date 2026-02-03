using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ExperimentController : MonoBehaviour
{
    [Header("UI")]
    public Slider gravitySlider;
    public Slider massSlider;
    public Slider distanceSlider;

    [Header("Display")]
    public TextMeshProUGUI dataText;

    [Header("Objects")]
    public List<Rigidbody> objects;
    public Transform spawnHeight;

    float startTime;
    bool experimentRunning;

    void Update()
    {
        Physics.gravity = new Vector3(0, gravitySlider.value, 0);

        foreach (var obj in objects)
            obj.mass = massSlider.value;

        UpdatePanel();
    }

    public void StartFall()
    {
        experimentRunning = true;
        startTime = Time.time;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].linearVelocity = Vector3.zero;
            objects[i].transform.position =
                spawnHeight.position +
                transform.forward * distanceSlider.value +
                Vector3.right * i * 0.5f;
        }
    }

    void UpdatePanel()
    {
        dataText.text =
        $"Gravity: {gravitySlider.value:F2} m/s²\n" +
        $"Mass: {massSlider.value:F2} kg\n" +
        $"Distance: {distanceSlider.value:F2} m\n\n";
    }
}
