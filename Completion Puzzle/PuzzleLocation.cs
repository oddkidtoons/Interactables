using UnityEngine;

public class PuzzleLocation : MonoBehaviour
{
    [Header("Puzzle Object Settings")]
    public string requiredObjectTag; // Tag of the correct object for this location
    private bool isCorrect = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the correct tag
        if (other.CompareTag(requiredObjectTag))
        {
            isCorrect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset if the object leaves the trigger
        if (other.CompareTag(requiredObjectTag))
        {
            isCorrect = false;
        }
    }

    public bool IsCorrectlyPlaced()
    {
        return isCorrect;
    }
}
