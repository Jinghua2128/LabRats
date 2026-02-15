//Made by Gracie Arianne Peh 11/02/25
//Script to find panel and play connected audio when rat button is pressed
using UnityEngine;

public class RatButton : MonoBehaviour
{
    public void OnPress()
    {
        PanelAudio activePanel = FindActivePanel(); 

        if (activePanel != null)
        {
            RatAudioManager.Instance.PlayClip(activePanel.narrationClip); //play connected audio
        }
    }

    PanelAudio FindActivePanel() //find the active panel in scene
    {
        PanelAudio[] panels = Object.FindObjectsByType<PanelAudio>(FindObjectsSortMode.None);
        foreach (PanelAudio panel in panels)
        {
            if (panel.gameObject.activeInHierarchy)
                return panel;
        }

        return null;
    }
}
