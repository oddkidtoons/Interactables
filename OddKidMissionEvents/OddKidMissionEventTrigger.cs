using UnityEngine;
using TMPro;

public class OddKidMissionEventTrigger : MonoBehaviour
{
    public string eventName;
    public OddKidMissionEvents eventManager; // Reference to the OddKidMissionEvents script
    public TextMeshProUGUI dialogueText;
    public float messageDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (eventManager != null)
            {
                if (eventManager.GetCurrentEventIndex() < eventManager.events.Count &&
                    eventManager.events[eventManager.GetCurrentEventIndex()].eventName == eventName)
                {
                    eventManager.TriggerEvent(eventName);
                    DisplayMessage("Event collected: " + eventName);
                }
                else
                {
                    DisplayMessage("Incorrect event order. Complete previous events first.");
                }
            }
        }
    }

    private void DisplayMessage(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = message;
            dialogueText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideMessage));
            Invoke(nameof(HideMessage), messageDuration);
        }
    }

    private void HideMessage()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }
}
