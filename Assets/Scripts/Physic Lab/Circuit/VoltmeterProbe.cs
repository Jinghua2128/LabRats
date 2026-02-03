using UnityEngine;

public class VoltmeterProbe : MonoBehaviour
{
    public bool isPositive;
    public Terminal touchingTerminal;

    private void OnTriggerEnter(Collider other)
    {
        touchingTerminal = other.GetComponent<Terminal>();
        Voltmeter.Instance.UpdateVoltage();
    }

    private void OnTriggerExit(Collider other)
    {
        touchingTerminal = null;
        Voltmeter.Instance.UpdateVoltage();
    }
}
