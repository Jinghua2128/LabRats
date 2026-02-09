using UnityEngine;
using System.Collections;

public class RandomRainController : MonoBehaviour
{
    [Header("Rain Settings")]
    public ParticleSystem rainSystem;
    public float minRainDuration = 20f;
    public float maxRainDuration = 60f;
    public float minBreakDuration = 30f;
    public float maxBreakDuration = 120f;

    [Header("Environment Response")]
    public Light directionalLight;
    public Color rainyLightColor = new Color(0.4f, 0.4f, 0.5f);
    private Color originalLightColor;

    [Header("Audio")]
    public AudioSource rainAudioSource;
    public AudioClip rainLoopSound;

    private bool isRaining = false;
    private Coroutine rainCoroutine;

    void Start()
    {
        // Cache original light settings
        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
        }

        // Initialize rain system
        rainSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        // Start rain cycle
        rainCoroutine = StartCoroutine(RainCycle());
    }

    IEnumerator RainCycle()
    {
        while (true)
        {
            // Random break between rains
            float breakDuration = Random.Range(minBreakDuration, maxBreakDuration);
            yield return new WaitForSeconds(breakDuration);
            // Start rain sequence
            yield return StartCoroutine(Raining());
            // Rain duration
            float rainDuration = Random.Range(minRainDuration, maxRainDuration);
            yield return new WaitForSeconds(rainDuration);
        }
    }
    IEnumerator Raining()
    {
        isRaining = true;

        // Start loop sound
        if (rainAudioSource != null && rainLoopSound != null)
        {
            rainAudioSource.clip = rainLoopSound;
            rainAudioSource.loop = true;
            rainAudioSource.Play();
        }

        // Start particle system
        rainSystem.Play();

        // Yield return null to satisfy IEnumerator
        yield return null;
    }
    void OnDestroy()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
        }
    }

    // For debugging rain state
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUI.Label(new Rect(10, 10, 200, 30), $"Rain State: {(isRaining ? "RAINING" : "DRY")}");
        }
    }
}