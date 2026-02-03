using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    public static CircuitManager Instance;

    public float batteryVoltage = 3.0f;
    public Ammeter ammeter;

    private void Awake()
    {
        Instance = this;
    }

    public void ValidateCircuit()
    {
        if (!IsClosedLoop())
        {
            ammeter.Zero();
            DisableCurrent();
            return;
        }

        float current = batteryVoltage / 10f; // simple R = 10Ω
        ammeter.UpdateReading(current);
        EnableCurrent(current);
    }

    bool IsClosedLoop()
    {
        // VERY SIMPLE VERSION (expand later)
        return true;
    }

    public float GetVoltageBetween(Terminal a, Terminal b)
    {
        if (!IsClosedLoop()) return 0f;

        if (a.isPositive && !b.isPositive)
            return batteryVoltage;

        if (!a.isPositive && b.isPositive)
            return -batteryVoltage;

        return 0f;
    }

    void EnableCurrent(float current)
    {
        // Activate wire shader animation
    }

    void DisableCurrent()
    {
        // Stop wire shader animation
    }
}
