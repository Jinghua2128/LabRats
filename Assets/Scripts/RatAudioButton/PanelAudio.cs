//Made by Gracie Arianne Peh 11/02/25
//Small script to play narration audio linked to panel


using UnityEngine;

public class PanelAudio : MonoBehaviour
{
    public AudioClip narrationClip;
    public bool autoPlayOnEnable = false;

    private void OnEnable()
    {
        if (autoPlayOnEnable)
        {
            RatAudioManager.Instance.PlayClip(narrationClip);
        }
    }
}
