using UnityEngine;

public class StealableItem : MonoBehaviour
{
    public string itemName = "Item";
    
    void Start()
    {
        // Add this item to shelf registry if needed
        gameObject.tag = "StealableItem";
    }
}