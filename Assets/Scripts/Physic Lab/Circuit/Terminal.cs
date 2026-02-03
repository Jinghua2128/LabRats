using UnityEngine;

public class Terminal : MonoBehaviour
{
    public bool isPositive;        // For battery / meters
    public CircuitComponent owner; // Battery, Bulb, Switch, etc.

    [HideInInspector]
    public WireEnd connectedWire;

    public bool IsConnected => connectedWire != null;
}
