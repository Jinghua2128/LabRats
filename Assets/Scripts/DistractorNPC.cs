using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DistractorNPC : BaseNPC
{
    [Header("Distractor Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float pushForce = 10f;
    public float activationDelay = 1f;
    public float runDuration = 15f; // How long to run around after activation
    public Transform exitPoint;
    private NavMeshAgent agent;
    private bool isActivated = false;
    private bool isWalking = true;
    private Rigidbody playerRb;
    public virtual void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
    }
    
    protected virtual void LeaveStore()
    {
        if (exitPoint != null && !arrested)
        {
            StartCoroutine(WalkToExit());
        }
        else
        {
            // Fallback - destroy after delay
            StartCoroutine(DestroyAfterDelay(2f));
        }
    }
    
    protected virtual IEnumerator WalkToExit()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(exitPoint.position);
            
            // Wait until close to exit
            while (Vector3.Distance(transform.position, exitPoint.position) > 1f)
            {
                yield return null;
            }
        }
        else
        {
            // Move without NavMesh if no agent
            while (Vector3.Distance(transform.position, exitPoint.position) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, exitPoint.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        
        // Notify GameManager and destroy
        if (GameManager.instance != null)
            GameManager.instance.OnNPCDestroyed();
        
        Destroy(gameObject);
    }
    protected virtual IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.instance != null)
            GameManager.instance.OnNPCDestroyed();
        
        Destroy(gameObject);
    }
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        
        firstInteractionText = "Trying to get something!";
        secondInteractionText = "Fine, you got me!";
        
        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();
        
        // Start walking around randomly from spawn
        StartCoroutine(WalkRandomly());
    }
    
    // Initial random walking behavior
    IEnumerator WalkRandomly()
    {
        while (isWalking && !isActivated && !arrested)
        {
            // Pick random point to walk to
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 8f;
            randomPoint.y = transform.position.y;
            
            agent.SetDestination(randomPoint);
            
            // Wait longer for casual walking
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
    
    public void ActivateDistraction()
    {
        if (!isActivated)
        {
            StartCoroutine(StartDistractionAfterDelay());
        }
    }
    
    IEnumerator StartDistractionAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        
        // Stop casual walking
        isWalking = false;
        isActivated = true;
        
        // Switch to running speed
        agent.speed = runSpeed;
        
        // Start running around frantically
        StartCoroutine(RunAroundFrantically());
    }
    
    IEnumerator RunAroundFrantically()
    {
        float runTimer = 0f;
        
        while (runTimer < runDuration && !arrested)
        {
            // Pick random point to run to (larger radius for frantic movement)
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 12f;
            randomPoint.y = transform.position.y;
            
            agent.SetDestination(randomPoint);
            
            // Shorter intervals for frantic running
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            runTimer += Random.Range(1f, 3f);
        }
        
        // After running around, head to exit
        if (!arrested)
        {
            LeaveStore();
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActivated && playerRb != null)
        {
            // Push player
            Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
            pushDirection.y = 0; // Keep push horizontal
            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}