using UnityEngine;

public class StoreExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Handle NPCs leaving the store
        BaseNPC npc = other.GetComponent<BaseNPC>();
        if (npc != null)
        {
            Debug.Log($"{npc.npcName} has left the store");
            
            // Notify GameManager
            if (GameManager.instance != null)
                GameManager.instance.OnNPCDestroyed();

            Destroy(other.gameObject);
        }
    }
}