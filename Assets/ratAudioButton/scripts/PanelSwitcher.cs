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
