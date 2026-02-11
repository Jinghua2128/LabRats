using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NormalShopperNPC : BaseNPC
{
    [Header("NormalShopper Settings")]
    public Transform[] shelfPoints;
    public Transform checkoutPoint;
    public Transform exitPoint;
    public float browseTime = 5f;

    private NavMeshAgent agent;
    private bool isAtCheckout = false;
    private int currentShelfIndex = 0;

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
        agent.speed = moveSpeed;

        firstInteractionText = "Just shopping, thanks.";
        secondInteractionText = "Why are you arresting me?!";

        StartShopping();
    }

    void StartShopping()
    {
        if (shelfPoints.Length > 0)
        {
            MoveToShelf();
        }
        else
        {
            GoToCheckout();
        }
    }

    void MoveToShelf()
    {
        if (currentShelfIndex < shelfPoints.Length)
        {
            agent.SetDestination(shelfPoints[currentShelfIndex].position);
            StartCoroutine(BrowseAtShelf());
        }
        else
        {
            GoToCheckout();
        }
    }

    System.Collections.IEnumerator BrowseAtShelf()
    {
        // Wait to reach shelf
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;

        // Browse for a while
        yield return new WaitForSeconds(browseTime);

        // Move to next shelf or checkout
        currentShelfIndex++;
        if (currentShelfIndex < shelfPoints.Length)
        {
            MoveToShelf();
        }
        else
        {
            GoToCheckout();
        }
    }

    void GoToCheckout()
    {
        if (checkoutPoint != null && !isAtCheckout)
        {
            isAtCheckout = true;
            agent.SetDestination(checkoutPoint.position);
            StartCoroutine(FinishShopping());
        }
    }

    IEnumerator FinishShopping()
    {
        // Wait to reach checkout
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;
        
        // Simulate checkout time
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        
        // Leave store
        LeaveStore();
    }
}
