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
