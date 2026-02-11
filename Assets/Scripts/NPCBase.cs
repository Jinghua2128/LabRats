using TMPro;
using UnityEngine;
using System.Collections;

public enum NPCType
{
    NormalShopper,
    Shoplifter,
    Distractor
}

public abstract class BaseNPC : MonoBehaviour
{
    [Header("NPC Base Settings")]
    public NPCType npcType;
    public float moveSpeed = 2f;
    public string npcName = "NPC";
    
    [Header("Interaction")]
    public TextMeshProUGUI interactionPrompt;
    public string firstInteractionText = "Hello there.";
    public string secondInteractionText = "Okay, you got me.";
    
    protected bool playerInRange = false;
    protected bool firstInteraction = false;
    protected bool arrested = false;
    protected Transform player;
    
    [Header("Audio")]
    public AudioClip[] voiceClips;
    protected AudioSource audioSource;
    
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
        
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }
    
    protected virtual void Update()
    {
        if (arrested) return;
        
        // Check for interaction
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Interacting with {npcName}, first interaction: {firstInteraction}"); // Debug log
            HandleInteraction();
        }
    }
    
    protected virtual void HandleInteraction()
    {
        if (!firstInteraction)
        {
            firstInteraction = true;
            ShowInteractionText(firstInteractionText);
            
            // Update interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.text = "Press E to arrest";
            }
        }
        else
        {
            ArrestNPC();
        }
    }
    
    protected virtual void ArrestNPC()
    {
        arrested = true;
        
        if (GameManager.instance != null)
            GameManager.instance.ArrestNPC(npcType, npcName);
        
        StartCoroutine(ArrestSequence());
    }
    
    protected virtual IEnumerator ArrestSequence()
    {
        // Show arrest text
        ShowInteractionText(secondInteractionText);
        
        // Hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
        
        // Spawn police car and play cutscene
        yield return StartCoroutine(PoliceCutscene());
        
        // Cleanup
        if (GameManager.instance != null)
            GameManager.instance.OnNPCDestroyed();
        
        Destroy(gameObject);
    }
    
    protected virtual IEnumerator PoliceCutscene()
    {
        // Find and activate police car
        PoliceCarController policeCar = FindFirstObjectByType<PoliceCarController>();
        if (policeCar != null)
        {
            yield return StartCoroutine(policeCar.ArrestSequence(transform.position));
        }
        else
        {
            // Fallback if no police car found
            Debug.Log("Police car arrives for arrest");
            yield return new WaitForSeconds(2f);
        }
    }
    
    protected virtual void ShowInteractionText(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = text;
            interactionPrompt.gameObject.SetActive(true);
            
            // Auto-hide after 3 seconds
            StartCoroutine(HideTextAfterDelay(3f));
        }
    }
    
    protected virtual IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (interactionPrompt != null && playerInRange && firstInteraction && !arrested)
        {
            interactionPrompt.text = "Press E to arrest";
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !arrested)
        {
            playerInRange = true;
            if (interactionPrompt != null)
            {
                if (!firstInteraction)
                {
                    interactionPrompt.text = "Press E to talk";
                }
                else
                {
                    interactionPrompt.text = "Press E to arrest";
                }
                interactionPrompt.gameObject.SetActive(true);
            }
            Debug.Log($"Player entered range of {npcName}"); // Debug log
        }
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.gameObject.SetActive(false);
            Debug.Log($"Player left range of {npcName}"); // Debug log
        }
    }
}