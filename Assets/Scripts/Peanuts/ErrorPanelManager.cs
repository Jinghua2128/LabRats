using UnityEngine;
using TMPro;

public class ErrorPanelManager : MonoBehaviour
{
    public static ErrorPanelManager Instance;

    [Header("Error UI")]
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TMP_Text errorText;

    [Header("Behaviour")]
    [SerializeField] private bool listenToGlobalLogs = true;
    [SerializeField] private bool showExceptions = true;
    [SerializeField] private bool showErrors = true;
    [SerializeField] private bool showWarnings = false;
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
        if (listenToGlobalLogs)
        {
            Application.logMessageReceived += HandleLog;
        }
    }

    private void OnDisable()
    {
        if (listenToGlobalLogs)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        bool shouldShow = false;

        switch (type)
        {
            case LogType.Exception:
                shouldShow = showExceptions;
                break;
            case LogType.Error:
                shouldShow = showErrors;
                break;
            case LogType.Warning:
                shouldShow = showWarnings;
                break;
        }

        if (!shouldShow)
            return;

        string message = logString;

        ShowError(message);
    }

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
}