/*
 * File: SwitchComponent.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Toggle switch circuit component that controls circuit flow.
 *              Can be opened or closed to break or complete the circuit.
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
/// Toggle switch circuit component. Controls whether the circuit is open or closed.
/// Triggers circuit validation when toggled.
/// </summary>
public class SwitchComponent : CircuitComponent
{
    /// <summary>
    /// Current state of the switch (true = closed/on, false = open/off).
    /// </summary>
    public bool isOn;

    /// <summary>
    /// Toggles the switch state and validates the circuit.
    /// </summary>
    public void Toggle()
    {
        isOn = !isOn;
        CircuitManager.Instance.ValidateCircuit();
    }
}
