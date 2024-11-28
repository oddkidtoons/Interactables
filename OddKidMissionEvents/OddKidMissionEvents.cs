using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OddKidMissionEvents : MonoBehaviour
{
    [System.Serializable]
    public class GameEvent
    {
        public string eventName;
        public bool isTriggered = false;
        public UnityEvent onTrigger;
    }

    public List<GameEvent> events; // List of sequential game events
    private int currentEventIndex = 0;
    public UnityEvent onAllEventsCompleted; // Final event when all events are completed
    public GameObject completionUI; // UI element to display upon completion
    public float completionDelay = 0f; // Delay before the completed event is invoked
    public float uiAutoHideTime = 5f; // Time to hide the completion UI automatically

    public int GetCurrentEventIndex()
    {
        return currentEventIndex;
    }

    public void TriggerEvent(string eventName)
    {
        if (currentEventIndex < events.Count && events[currentEventIndex].eventName == eventName)
        {
            events[currentEventIndex].onTrigger.Invoke();
            events[currentEventIndex].isTriggered = true;
            currentEventIndex++;

            // Check if all events are completed
            if (currentEventIndex >= events.Count)
            {
                if (completionDelay > 0)
                {
                    Invoke(nameof(InvokeCompletionEvent), completionDelay);
                }
                else
                {
                    InvokeCompletionEvent();
                }
            }
        }
        else
        {
            Debug.LogWarning("Incorrect event triggered or event already completed: " + eventName);
        }
    }

    private void InvokeCompletionEvent()
    {
        onAllEventsCompleted.Invoke();
        ShowCompletionUI();
    }

    private void ShowCompletionUI()
    {
        if (completionUI != null)
        {
            completionUI.SetActive(true);

            // Start the timer to hide the UI
            if (uiAutoHideTime > 0)
            {
                Invoke(nameof(HideCompletionUI), uiAutoHideTime);
            }
        }
    }

    private void HideCompletionUI()
    {
        if (completionUI != null)
        {
            completionUI.SetActive(false);
        }
    }
}
