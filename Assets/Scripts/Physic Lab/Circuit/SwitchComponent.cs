using UnityEngine;

public class SwitchComponent : CircuitComponent
{
    public bool isOn;

    public void Toggle()
    {
        isOn = !isOn;
        CircuitManager.Instance.ValidateCircuit();
    }
}
