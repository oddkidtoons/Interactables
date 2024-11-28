using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class OddKidEventTimer : MonoBehaviour
{
    public float timerDuration = 10f; // Duration in seconds for the timer
    private float timer = 0f;
    private bool timerRunning = false;
    
    public TextMeshProUGUI timerText;  // Reference to the UI text to show the timer countdown
    public UnityEvent onTimerComplete;  // Event to trigger when the timer completes

    public UnityEvent firstEvent;  // First event to trigger (set in inspector)
    public UnityEvent secondEvent;  // Second event to trigger (set in inspector)

    // Completion UI
    public GameObject completionUI; // UI to display upon completion
    public float completionUIDuration = 3f; // Duration to display the completion UI

    // Start is called before the first frame update
    void Start()
    {
        if (onTimerComplete == null)
        {
            onTimerComplete = new UnityEvent(); // Ensure there's an event to trigger
        }

        if (completionUI != null)
        {
            completionUI.SetActive(false); // Hide the completion UI at the start
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;  // Decrease timer by time passed each frame
            if (timer <= 0)
            {
                timer = 0;
                TriggerSecondEvent();  // Call the event when the timer completes
            }
            UpdateTimerUI();  // Update the UI text with the current timer
        }
    }

    // Method to start the timer
    public void StartTimer()
    {
        timer = timerDuration;  // Reset the timer to the set duration
        timerRunning = true;  // Start the timer countdown
        firstEvent.Invoke();  // Trigger the first event when the timer starts
    }

    // Method to trigger the second event
    private void TriggerSecondEvent()
    {
        timerRunning = false;  // Stop the timer when it's done
        secondEvent.Invoke();  // Trigger the second event
        onTimerComplete.Invoke();  // Trigger any additional actions after the timer completes

        ShowCompletionUI(); // Activate the completion UI
    }

    // Update the UI with the current timer countdown
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timer).ToString();  // Show the countdown in the UI
        }
    }

    // Display the completion UI for a set duration
    private void ShowCompletionUI()
    {
        if (completionUI != null)
        {
            completionUI.SetActive(true); // Show the completion UI
            Invoke(nameof(HideCompletionUI), completionUIDuration); // Schedule its deactivation
        }
    }

    // Hide the completion UI
    private void HideCompletionUI()
    {
        if (completionUI != null)
        {
            completionUI.SetActive(false); // Hide the completion UI
        }
    }
}
