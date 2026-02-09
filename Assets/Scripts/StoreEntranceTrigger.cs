using UnityEngine;

public class StoreEntranceTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
                GameManager.instance.OnStoreEntered();
        }
    }
}
