using UnityEngine;
using TMPro;

public class Voltmeter : MonoBehaviour
{
    public static Voltmeter Instance;

    public VoltmeterProbe positiveProbe;
    public VoltmeterProbe negativeProbe;
    public TextMeshPro display;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateVoltage()
    {
        if (positiveProbe.touchingTerminal == null || negativeProbe.touchingTerminal == null)
        {
            display.text = "0.00 V";
            return;
        }

        float voltage = CircuitManager.Instance.GetVoltageBetween(
            positiveProbe.touchingTerminal,
            negativeProbe.touchingTerminal
        );

        display.text = voltage.ToString("F2") + " V";
    }
}
