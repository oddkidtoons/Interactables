using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CollectibleCounter : MonoBehaviour
{
    public string targetTag = "Collectible"; // Tag of the objects to collect
    public int requiredCount = 3; // Number of objects required to unlock

    public TextMeshProUGUI uiText; // Reference to the TextMeshProUGUI component
    public AudioClip collectSound; // Sound effect for collecting an item
    public AudioClip completeSound; // Sound effect for collecting all items
    public UnityEvent onCollectionComplete; // Event to trigger when all objects are collected
    public UnityEvent onItemCollected; // Event to trigger when an item is collected

    private int collectedCount;

    void Start()
    {
        collectedCount = 0;
        UpdateUIText(); // Initialize UI text
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            collectedCount++;
            Destroy(other.gameObject); 

            // Play collect sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position); 
            }

            // Trigger onItemCollected event
            onItemCollected.Invoke();

            if (collectedCount >= requiredCount)
            {
                onCollectionComplete.Invoke();
                Debug.Log("All collectibles collected!");

                // Play complete sound
                if (completeSound != null)
                {
                    AudioSource.PlayClipAtPoint(completeSound, transform.position); 
                }
            }

            UpdateUIText(); // Update UI text after each collection
        }
    }

    void UpdateUIText()
    {
        if (uiText != null)
        {
            uiText.text = "Collected: " + collectedCount + " / " + requiredCount;
        }
    }
}