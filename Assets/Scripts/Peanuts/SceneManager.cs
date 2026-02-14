using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    [Header("Game Scene")]
    [SerializeField] private string MainArea = "01_MainArea";
    [SerializeField] private string PhysLab = "02_PhysLab";
    [SerializeField] private string ChemLab = "03_ChemLab";
    public void LoadPhysLab(string PhysLab)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(PhysLab);
    }
    public void LoadChemLab(string ChemLab)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ChemLab);
    }
    public void LoadMainArea(string MainArea)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainArea);
    }
    
}
