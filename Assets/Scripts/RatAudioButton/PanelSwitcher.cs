//Made by Gracie Arianne Peh 11/02/25
//Small script to change panels with a button

using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject panelToShow;
    public GameObject panelToHide;

    public void SwitchPanel()
    {
        if (panelToHide != null)
            panelToHide.SetActive(false);

        if (panelToShow != null)
            panelToShow.SetActive(true);
    }
}
