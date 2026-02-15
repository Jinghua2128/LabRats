/*
 * File: Terminal.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Connection point for circuit components. Represents positive/negative terminals
 *              that wires can connect to for building electrical circuits.
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
/// Terminal connection point for circuit components. Wires connect to terminals
/// to form electrical circuits. Tracks polarity and connected wires.
/// </summary>
public class Terminal : MonoBehaviour
{
    /// <summary>
    /// Indicates if this is a positive terminal (true) or negative terminal (false).
    /// Used for batteries and measurement devices.
    /// </summary>
    public bool isPositive;
    
    /// <summary>
    /// Reference to the circuit component that owns this terminal.
    /// </summary>
    public CircuitComponent owner;

    /// <summary>
    /// Reference to the wire end currently connected to this terminal.
    /// </summary>
    [HideInInspector]
    public WireEnd connectedWire;

    /// <summary>
    /// Indicates whether a wire is currently connected to this terminal.
    /// </summary>
    public bool IsConnected => connectedWire != null;
}
