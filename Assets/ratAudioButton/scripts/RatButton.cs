using UnityEngine;

public class RatButton : MonoBehaviour
{
    public void OnPress()
    {
        PanelAudio activePanel = FindActivePanel();

        if (activePanel != null)
        {
            RatAudioManager.Instance.PlayClip(activePanel.narrationClip);
        }
    }

    PanelAudio FindActivePanel()
    {
        PanelAudio[] panels = FindObjectsOfType<PanelAudio>();

        foreach (PanelAudio panel in panels)
        {
            if (panel.gameObject.activeInHierarchy)
                return panel;
        }

        return null;
    }
}
