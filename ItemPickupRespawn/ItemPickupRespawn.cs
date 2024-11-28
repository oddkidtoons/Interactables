using UnityEngine;
using UnityEngine.Events;

public class ItemPickupRespawn : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player"; // Tag for objects that can pick up the item
    public float respawnTime = 5f; // Time in seconds before the item respawns

    [Header("References")]
    public GameObject itemPrefab; // The item prefab to spawn
    public Transform spawnPoint; // Where the item should spawn

    [Header("Events")]
    public UnityEvent onItemPickedUp; // Event triggered when the item is picked up

    private GameObject currentItem; // Keeps track of the current active item
    private bool canTriggerEvent = true; // Ensures the event can only be triggered when the item is present

    private void Start()
    {
        SpawnItem(); // Spawn the initial item
    }

    private void SpawnItem()
    {
        if (itemPrefab != null && spawnPoint != null)
        {
            currentItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
            canTriggerEvent = true; // Allow the event to trigger again
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and the item exists
        if (other.CompareTag(playerTag) && currentItem != null && canTriggerEvent)
        {
            // Trigger the event
            onItemPickedUp.Invoke();

            // Destroy the current item to simulate pickup
            Destroy(currentItem);
            currentItem = null; // Reset the current item reference
            canTriggerEvent = false; // Disable the event until the item respawns

            // Start the respawn coroutine
            StartCoroutine(RespawnItem());
        }
    }

    private System.Collections.IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime); // Wait for the respawn time

        // Respawn the item
        SpawnItem();
    }
}
