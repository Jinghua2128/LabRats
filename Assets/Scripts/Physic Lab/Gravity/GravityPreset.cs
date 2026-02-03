using UnityEngine;
using UnityEngine.UI;

public class GravityPreset : MonoBehaviour
{
    public float gravityValue;
    public Slider gravitySlider;

    public void SetGravity()
    {
        gravitySlider.value = gravityValue;
    }
}
