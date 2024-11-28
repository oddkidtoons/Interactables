using UnityEngine;

public class OddKidTrigger : MonoBehaviour
{
    public OddKidEventTimer eventTimer;  // Reference to the OddKidEventTimer script
    public string triggerTag = "Player";  // Tag to specify which object triggers the timer

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger zone has the correct tag
        if (other.CompareTag(triggerTag))
        {
            if (eventTimer != null)
            {
                eventTimer.StartTimer();  // Start the timer
                Debug.Log("Timer triggered by: " + other.name);
            }
            else
            {
                Debug.LogWarning("OddKidEventTimer not assigned.");
            }
        }
    }
}

