using UnityEngine;

public class FallTimer : MonoBehaviour
{
    float startTime;
    bool falling;

    void OnEnable()
    {
        startTime = Time.time;
        falling = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!falling) return;

        float time = Time.time - startTime;
        Debug.Log(gameObject.name + " Time: " + time);
        falling = false;
    }
}
