using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ShoplifterNPC : BaseNPC
{
    [Header("Shoplifter Settings")]
    public Transform[] itemShelves;
    public GameObject stolenItemPrefab; // Visual representation of stolen item
    public Transform handTransform; // Where to show stolen item
    public Transform exitPoint;
    public float stealTime = 2f;
    
    private NavMeshAgent agent;
    private bool hasStolen = false;
    private GameObject currentStolenItem;
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

        firstInteractionText = "I'm good bro. What u doing?";
        secondInteractionText = "Alright, you got me!";

        StartStealing();
    }
    
    void StartStealing()
    {
        if (itemShelves.Length > 0 && !hasStolen)
        {
            Transform targetShelf = itemShelves[Random.Range(0, itemShelves.Length)];
            agent.SetDestination(targetShelf.position);
            StartCoroutine(StealFromShelf(targetShelf));
        }
    }
    
    IEnumerator StealFromShelf(Transform shelf)
    {
        // Wait to reach shelf
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;
        
        // Steal animation time
        yield return new WaitForSeconds(stealTime);
        
        // Check if shelf has stealable items
        StealableItem[] items = shelf.GetComponentsInChildren<StealableItem>();
        if (items.Length > 0)
        {
            StealableItem itemToSteal = items[Random.Range(0, items.Length)];
            StealItem(itemToSteal);
        }
        
        hasStolen = true;
    }

    void StealItem(StealableItem item)
    {
        // Notify game manager
        if (GameManager.instance != null)
            GameManager.instance.ItemStolen(item.itemName);

        // Show stolen item in hand
        if (stolenItemPrefab != null && handTransform != null)
        {
            currentStolenItem = Instantiate(stolenItemPrefab, handTransform.position, handTransform.rotation, handTransform);
        }

        // Hide original item
        item.gameObject.SetActive(false);

        // Activate distractors
        ActivateDistractors();
        LeaveStore();
    }
    
    void ActivateDistractors()
    {
        DistractorNPC[] distractors = FindObjectsByType<DistractorNPC>(FindObjectsSortMode.None);
        foreach (var distractor in distractors)
        {
            distractor.ActivateDistraction();
        }
    }
}