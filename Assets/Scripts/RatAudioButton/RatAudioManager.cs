//Made by Gracie Arianne Peh 11/02/25
//Script to handle audio

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
        if (narrationSource.isPlaying && currentClip == clip) //to stop audio when button is pressed again
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
