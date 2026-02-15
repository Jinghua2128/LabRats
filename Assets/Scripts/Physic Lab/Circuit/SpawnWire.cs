/*
 * File: SpawnWire.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Utility component that spawns wire prefabs at a specified location.
 *              Used for creating new wires during circuit building.
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
/// Spawns wire prefabs at a designated spawn point.
/// Used for dynamically creating new wires during circuit construction.
/// </summary>
public class SpawnWire : MonoBehaviour
{
    /// <summary>
    /// Prefab of the wire to spawn.
    /// </summary>
    public GameObject wirePrefab;
    
    /// <summary>
    /// Transform indicating where to spawn new wires.
    /// </summary>
    public Transform spawnPoint;
    
    /// <summary>
    /// Instantiates a new wire at the spawn point.
    /// </summary>
    public void SpawnNewWire()
    {
        Instantiate(wirePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
