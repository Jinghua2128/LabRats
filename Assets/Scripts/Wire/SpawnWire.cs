using UnityEngine;

public class SpawnWire : MonoBehaviour
{
    public GameObject wirePrefab;
    public Transform spawnPoint;
    public void SpawnNewWire()
    {
        Instantiate(wirePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
