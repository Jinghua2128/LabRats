/*
 * File: Ammeter.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Circuit component that measures and displays electrical current in Amperes.
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
/// Ammeter circuit component that measures and displays current flow in the circuit.
/// Inherits from CircuitComponent for terminal connections.
/// </summary>
public class Ammeter : CircuitComponent
{
    /// <summary>
    /// 3D text display for showing current readings.
    /// </summary>
    public TextMeshPro display;

    /// <summary>
    /// Updates the ammeter display with the current reading.
    /// </summary>
    /// <param name="current">Current value in Amperes.</param>
    public void UpdateReading(float current)
    {
        display.text = current.ToString("F2") + " A";
    }

    /// <summary>
    /// Resets the ammeter display to zero.
    /// </summary>
    public void Zero()
    {
        display.text = "0.00 A";
    }
}
