/*
 * File: Voltmeter.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Circuit measurement tool that measures voltage difference between two probe points.
 *              Singleton component for centralized voltage measurement access.
 * 
 * Author: Liu GuangXuan
 * Organization: G²KM Studio
 * Copyright: © 2026 G²KM Studio. All rights reserved.
 * 
 * Created: 2026
 * Last Modified: 2026-02-15
 */

using UnityEngine;
using TMPro;

/// <summary>
/// Voltmeter measurement tool that calculates and displays voltage difference
/// between two probe points in the circuit. Singleton for global access.
/// </summary>
public class Voltmeter : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Voltmeter.
    /// </summary>
    public static Voltmeter Instance;

    /// <summary>
    /// Positive probe reference.
    /// </summary>
    public VoltmeterProbe positiveProbe;
    
    /// <summary>
    /// Negative probe reference.
    /// </summary>
    public VoltmeterProbe negativeProbe;
    
    /// <summary>
    /// 3D text display for showing voltage readings.
    /// </summary>
    public TextMeshPro display;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Updates the voltage display based on current probe positions.
    /// </summary>
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
