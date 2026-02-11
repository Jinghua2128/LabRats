using UnityEngine;

public class RatAudioManager : MonoBehaviour
{
    public static RatAudioManager Instance;

    public AudioSource narrationSource;

    private AudioClip currentClip;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayClip(AudioClip clip)
    {
        if (narrationSource.isPlaying && currentClip == clip)
        {
            narrationSource.Stop();
            return;
        }

        narrationSource.Stop();
        narrationSource.clip = clip;
        currentClip = clip;
        narrationSource.Play();
    }
}
