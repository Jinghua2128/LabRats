using UnityEngine;
using UnityEngine.AI;

public class RobotPatrol : MonoBehaviour
{
    public Transform[] points; // assign in Inspector
    private int currentPointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (points.Length > 0)
        {
            agent.SetDestination(points[0].position);
        }
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
            agent.SetDestination(points[currentPointIndex].position);
        }
    }
}
