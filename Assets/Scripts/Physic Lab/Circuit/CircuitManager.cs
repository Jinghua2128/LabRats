/*
 * File: CircuitManager.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Singleton manager that validates circuit connections, calculates current flow,
 *              and manages voltage between terminals. Central controller for circuit simulation.
 * 
 * Author: Liu GuangXuan
 * Organization: G²KM Studio
 * Copyright: © 2026 G²KM Studio. All rights reserved.
 * 
 * Created: 2026
 * Last Modified: 2026-02-15
 */

using UnityEngine;

/// <summary>
/// Singleton manager for circuit simulation. Validates circuit topology,
/// calculates current flow, and determines voltage between terminals.
/// </summary>
public class CircuitManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the CircuitManager.
    /// </summary>
    public static CircuitManager Instance;

    /// <summary>
    /// Battery voltage in the circuit (Volts).
    /// </summary>
    public float batteryVoltage = 3.0f;
    
    /// <summary>
    /// Reference to the ammeter for displaying current readings.
    /// </summary>
    public Ammeter ammeter;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Validates the circuit topology and updates current flow if circuit is closed.
    /// </summary>
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

    /// <summary>
    /// Calculates the voltage difference between two terminals.
    /// </summary>
    /// <param name="a">First terminal.</param>
    /// <param name="b">Second terminal.</param>
    /// <returns>Voltage difference in Volts.</returns>
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
