using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public List<PuzzleLocation> puzzleLocations; // List of all locations
    public UnityEvent onPuzzleComplete;          // Event triggered when puzzle is solved

    [Header("UI Settings")]
    public GameObject completionUI;             // UI to display when puzzle is complete
    public float completionUIDuration = 3f;     // Time before hiding the completion UI

    private bool puzzleSolved = false;

    private void Start()
    {
        // Ensure the completion UI is hidden initially
        if (completionUI != null)
        {
            completionUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Check if all locations are solved and trigger puzzle complete
        if (!puzzleSolved && AreAllLocationsSolved())
        {
            puzzleSolved = true;
            PuzzleComplete();
        }
    }

    private bool AreAllLocationsSolved()
    {
        // Return true only if all puzzle locations are solved
        foreach (PuzzleLocation location in puzzleLocations)
        {
            if (!location.IsCorrectlyPlaced())
                return false;
        }
        return true;
    }

    private void PuzzleComplete()
    {
        // Trigger completion event and show UI
        onPuzzleComplete.Invoke();

        if (completionUI != null)
        {
            completionUI.SetActive(true);
            Invoke(nameof(HideCompletionUI), completionUIDuration);
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
