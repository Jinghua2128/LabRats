/*
 * File: WireEnd.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Endpoint of a wire that can snap to and connect with circuit terminals.
 *              Manages wire-to-terminal connections using proximity detection.
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
/// Wire endpoint that automatically snaps to nearby terminals.
/// Manages connection state between wires and circuit component terminals.
/// </summary>
public class WireEnd : MonoBehaviour
{
    /// <summary>
    /// Reference to the terminal this wire end is currently connected to.
    /// </summary>
    public Terminal connectedTerminal;
    
    /// <summary>
    /// Transform indicating the snap point for terminal connections.
    /// </summary>
    public Transform snapPoint;
    
    /// <summary>
    /// Maximum distance for automatic snapping to terminals.
    /// </summary>
    public float snapDistance = 0.05f;

    /// <summary>
    /// Called when this wire end stays within a terminal's trigger collider.
    /// Automatically snaps to nearby unconnected terminals.
    /// </summary>
    /// <param name="other">The collider being touched.</param>
    private void OnTriggerStay(Collider other)
    {
        Terminal terminal = other.GetComponent<Terminal>();
        if (terminal == null) return;

        if (connectedTerminal == null && !terminal.IsConnected)
        {
            float dist = Vector3.Distance(transform.position, terminal.transform.position);
            if (dist < snapDistance)
            {
                SnapToTerminal(terminal);
            }
        }
    }

    /// <summary>
    /// Snaps this wire end to the specified terminal and establishes the connection.
    /// </summary>
    /// <param name="terminal">The terminal to snap to.</param>
    void SnapToTerminal(Terminal terminal)
    {
        connectedTerminal = terminal;
        terminal.connectedWire = this;

        transform.position = terminal.transform.position;
        transform.rotation = terminal.transform.rotation;
    }

    /// <summary>
    /// Disconnects this wire end from its current terminal.
    /// </summary>
    public void Disconnect()
    {
        if (connectedTerminal != null)
        {
            connectedTerminal.connectedWire = null;
            connectedTerminal = null;
        }
    }
}
