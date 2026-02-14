using UnityEngine;
using System.Collections.Generic;

public class Wire : MonoBehaviour
{
    public int particleCount;
    public List<WireParticle> _particles = new List<WireParticle>();
    public float particleResetLength;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float springStiffness = 100f;
    public float particleMass = 1f;
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
        public class WireParticle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 force;
        public WireParticle(Vector3 pos)
        {
            this.position = pos;
        }
    }
}
