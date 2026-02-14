using UnityEngine;
using TMPro;

public class ErrorPanelManager : MonoBehaviour
{
    public static ErrorPanelManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_Text errorText;

    [Header("Optional Settings")]
    [SerializeField] private bool listenToGlobalErrors = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (listenToGlobalErrors)
            Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        if (listenToGlobalErrors)
            Application.logMessageReceived -= HandleLog;
    }

    // -------- Public API --------

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

    // -------- Optional Global Catcher --------

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            ShowError(logString);
        }
    }
}