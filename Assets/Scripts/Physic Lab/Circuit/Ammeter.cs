using UnityEngine;
using TMPro;

public class Ammeter : CircuitComponent
{
    public TextMeshPro display;

    public void UpdateReading(float current)
    {
        display.text = current.ToString("F2") + " A";
    }

    public void Zero()
    {
        display.text = "0.00 A";
    }
}
