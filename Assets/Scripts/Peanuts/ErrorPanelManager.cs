using UnityEngine;
using TMPro;

public class ErrorPanelManager : MonoBehaviour
{
    public static ErrorPanelManager Instance;

    // ---------- UI References ----------

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_Text errorText;

    // ---------- Optional Settings ----------

    [SerializeField] private bool listenToGlobalErrors = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensures error UI starts hidden
        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Listens for Unity console errors and shows them in UI
        if (listenToGlobalErrors)
            Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        if (listenToGlobalErrors)
            Application.logMessageReceived -= HandleLog;
    }

    // ---------- Public API ----------

    public void ShowError(string message)
    {
        if (errorText != null)
            errorText.text = message;

        if (errorPanel != null)
            errorPanel.SetActive(true);
    }

    public void HideError()
    {
        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    // ---------- Global Catcher ----------

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Catches errors from anywhere in the app and shows them in the error panel
        if (type == LogType.Error || type == LogType.Exception)
            ShowError(logString);
    }
}