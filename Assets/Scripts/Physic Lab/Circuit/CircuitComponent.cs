/*
 * File: CircuitComponent.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Base class for all circuit components (batteries, bulbs, switches, meters).
 *              Provides common terminal connection points for circuit building.
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
/// Base class for all circuit components. Provides terminal connection points
/// for building electrical circuits in the physics lab simulation.
/// </summary>
public class CircuitComponent : MonoBehaviour
{
    /// <summary>
    /// Array of terminal connection points for this component.
    /// </summary>
    public Terminal[] terminals;
}
