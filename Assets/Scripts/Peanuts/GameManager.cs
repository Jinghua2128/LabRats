using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;  // child: InputField
    [SerializeField] private TMP_Text secondsText;       // child: Seconds/number

    private float startTime;
    private DatabaseReference dbRef;

    // Simple data object to send to Firebase
    [System.Serializable]
    private class UpdateEntry
    {
        public string text;
        public int seconds;
    }

    private void OnEnable()
    {
        // Reset timer whenever this object becomes active
        startTime = Time.time;
    }

    private void Start()
    {
        // Subscribe to the input field end-edit event
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnInputSubmitted);

        // Initialise Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase ready for UpdateTest.");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + status);
            }
        });
    }

    private void Update()
    {
        float elapsed = Time.time - startTime;
        int seconds = Mathf.FloorToInt(elapsed);

        if (secondsText != null)
            secondsText.text = seconds.ToString();
    }

    private void OnInputSubmitted(string currentText)
    {
        // When player finishes typing, save to Firebase
        if (string.IsNullOrEmpty(currentText))
            return;

        float elapsed = Time.time - startTime;
        SaveToFirebase(currentText, Mathf.FloorToInt(elapsed));
    }

    private void SaveToFirebase(string text, int seconds)
    {
        if (dbRef == null)
        {
            Debug.LogWarning("Firebase not ready yet, could not save.");
            return;
        }

        // /updateTests/<autoKey>
        string key = dbRef.Child("updateTests").Push().Key;

        UpdateEntry entry = new UpdateEntry
        {
            text = text,
            seconds = seconds
        };

        string json = JsonUtility.ToJson(entry);
        dbRef.Child("updateTests").Child(key).SetRawJsonValueAsync(json);

        Debug.Log($"Saved to Firebase: \"{text}\" after {seconds} seconds.");
    }
}
