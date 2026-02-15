/*
 * File: VoltmeterProbe.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Probe component for the voltmeter that detects contact with circuit terminals.
 *              Tracks which terminal the probe is touching for voltage measurement.
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
/// Voltmeter probe that detects contact with circuit terminals.
/// Triggers voltage update when touching or leaving terminals.
/// </summary>
public class VoltmeterProbe : MonoBehaviour
{
    /// <summary>
    /// Indicates if this is the positive probe (true) or negative probe (false).
    /// </summary>
    public bool isPositive;
    
    /// <summary>
    /// Reference to the terminal currently being touched by this probe.
    /// </summary>
    public Terminal touchingTerminal;

    /// <summary>
    /// Called when probe enters contact with a terminal collider.
    /// </summary>
    /// <param name="other">The collider that was entered.</param>
    private void OnTriggerEnter(Collider other)
    {
        touchingTerminal = other.GetComponent<Terminal>();
        Voltmeter.Instance.UpdateVoltage();
    }

    /// <summary>
    /// Called when probe exits contact with a terminal collider.
    /// </summary>
    /// <param name="other">The collider that was exited.</param>
    private void OnTriggerExit(Collider other)
    {
        touchingTerminal = null;
        Voltmeter.Instance.UpdateVoltage();
    }
}
