/*
 * File: Wire.cs
 * Project: LabRats - Physics Lab (Circuit)
 * Description: Physics-based wire simulation using spring particle system.
 *              Creates flexible wire behavior with gravity and damping for realistic connections.
 * 
 * Author: Liu GuangXuan
 * Organization: G²KM Studio
 * Copyright: © 2026 G²KM Studio. All rights reserved.
 * 
 * Created: 2026
 * Last Modified: 2026-02-15
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Physics-based wire simulation using a spring particle system.
/// Creates flexible wire connections with realistic motion and gravity.
/// </summary>
public class Wire : MonoBehaviour
{
    /// <summary>
    /// Number of particles in the wire simulation.
    /// </summary>
    public int particleCount;
    
    /// <summary>
    /// List of particles that make up the wire.
    /// </summary>
    public List<WireParticle> _particles = new List<WireParticle>();
    
    /// <summary>
    /// Rest length between individual particles.
    /// </summary>
    public float particleResetLength;
    
    /// <summary>
    /// Gravity force applied to wire particles.
    /// </summary>
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    
    /// <summary>
    /// Spring stiffness for connections between particles.
    /// </summary>
    public float springStiffness = 100f;
    
    /// <summary>
    /// Mass of each particle in the wire.
    /// </summary>
    public float particleMass = 1f;
    
    /// <summary>
    /// Damping factor to stabilize particle movement.
    /// </summary>
    public float damping = 0.2f;

    void Start()
    {
        var offset = new Vector3(x:particleResetLength, y:0, z:0);
        for (var i = 0; i < particleCount; i++)
        {
            if (i == 0)
            {
                _particles.Add(new WireParticle(transform.position));
            }
            else
            {
                _particles.Add(new WireParticle(transform.position + offset * i));
            }
        }
    }
    private void FixedUpdate()
    {
        for (var i = 1; i < _particles.Count; i++)
        {
            Vector3 displacement = _particles[i].position - _particles[i - 1].position;
            float offset = displacement.magnitude - particleResetLength;
            Vector3 springForce =springStiffness * offset * displacement.normalized;
            _particles[i].force += springForce;
            _particles[i - 1].force -= springForce;
            _particles[i].force += gravity * particleMass;
            _particles[i].force -= damping * _particles[i].velocity;
        }
        _particles[0].position = transform.position;
        for (var i = 0; i < _particles.Count; i++)
        {
            Vector3 acceleration = _particles[i].force / particleMass;
            _particles[i].velocity += acceleration * Time.fixedDeltaTime;
            _particles[i].position += _particles[i].velocity * Time.fixedDeltaTime;
            _particles[i].force = Vector3.zero;
        }
    }
    private void OnDrawGizmos()
    {        
        Gizmos.color = Color.red;
        for(int i =0; i < _particles.Count; i++)
        {
            if (i > 0)
            {
                Gizmos.DrawSphere(_particles[i - 1].position, 0.5f);
                if (i == 0)
                {
                    continue;
                }
                Gizmos.DrawLine(_particles[i - 1].position, _particles[i].position);
            }
        }
    }
    
    /// <summary>
    /// Individual particle in the wire simulation system.
    /// Stores position, velocity, and force data for physics calculations.
    /// </summary>
    public class WireParticle
    {
        /// <summary>
        /// Current position of the particle in world space.
        /// </summary>
        public Vector3 position;
        
        /// <summary>
        /// Current velocity of the particle.
        /// </summary>
        public Vector3 velocity;
        
        /// <summary>
        /// Accumulated force on the particle for this physics step.
        /// </summary>
        public Vector3 force;
        
        /// <summary>
        /// Creates a new wire particle at the specified position.
        /// </summary>
        /// <param name="pos">Initial position of the particle.</param>
        public WireParticle(Vector3 pos)
        {
            this.position = pos;
        }
    }
}
