using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject landingPanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signupPanel;
    [SerializeField] private GameObject errorPanel;
    
    [Header("Intro Fade")]
    [SerializeField] private CanvasGroup introCanvasGroup;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float fadeOutDuration = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ShowLanding();
    }

    // ---------- Core helpers ----------

    private void DisableAllPanels()
    {
        landingPanel?.SetActive(false);
        introPanel?.SetActive(false);
        choicePanel?.SetActive(false);
        loginPanel?.SetActive(false);
        signupPanel?.SetActive(false);

    }

    private void ClearInputs(GameObject panel)
    {
        if (panel == null) return;

        foreach (var input in panel.GetComponentsInChildren<TMP_InputField>(true))
            input.text = "";
    }

    // ---------- Scene start ----------

    public void ShowLanding()
    {
        DisableAllPanels();
        landingPanel.SetActive(true);
    }

    // ---------- Landing → Intro → Choice ----------

    public void PlayIntroAndShowChoice()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        introPanel.SetActive(true);
        introCanvasGroup.alpha = 0f;

        // Fade in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            introCanvasGroup.alpha = t / fadeInDuration;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);
        landingPanel?.SetActive(false);

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            introCanvasGroup.alpha = 1f - (t / fadeOutDuration);
            yield return null;
        }

        introPanel.SetActive(false);
        ShowChoice();
    }

    // ---------- Choice ----------

    public void ShowChoice()
    {
        DisableAllPanels();
        choicePanel.SetActive(true);
    }

    // ---------- Login / Signup ----------

    public void ShowLogin()
    {
        DisableAllPanels();
        ClearInputs(loginPanel);
        loginPanel.SetActive(true);
    }

    public void ShowSignup()
    {
        DisableAllPanels();
        ClearInputs(signupPanel);
        signupPanel.SetActive(true);
    }

    public void BackToChoice()
    {
        ShowChoice();
    }
}