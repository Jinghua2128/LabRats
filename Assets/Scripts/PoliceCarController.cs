using UnityEngine;
using System.Collections;

public class PoliceCarController : MonoBehaviour
{
    [Header("Police Car Settings")]
    public Transform shopEntrance; // Where the car should drive to
    public float driveSpeed = 10f;
    public float waitTime = 10f;
    public AudioClip sirenSound;
    
    private Vector3 startPosition;
    private AudioSource audioSource;
    private bool isActive = false;
    
    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        
        // Hide car initially
        gameObject.SetActive(false);
    }
    
    public IEnumerator ArrestSequence(Vector3 arrestLocation)
    {
        if (isActive) yield break;
        
        isActive = true;
        gameObject.SetActive(true);
        
        // Play siren sound
        if (sirenSound != null && audioSource != null)
        {
            audioSource.clip = sirenSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Drive to shop entrance
        Vector3 targetPosition = shopEntrance != null ? shopEntrance.position : arrestLocation;
        yield return StartCoroutine(DriveToPosition(targetPosition));
        
        // Wait at location
        yield return new WaitForSeconds(waitTime);
        
        // Drive back to start position
        yield return StartCoroutine(DriveToPosition(startPosition));
        
        // Stop siren and hide car
        if (audioSource != null)
            audioSource.Stop();
        
        gameObject.SetActive(false);
        isActive = false;
    }
    
    IEnumerator DriveToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPos, driveSpeed * Time.deltaTime);
            
            // Rotate to face movement direction
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            
            yield return null;
        }
    }
}