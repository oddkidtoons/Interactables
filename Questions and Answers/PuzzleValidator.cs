using UnityEngine;
using UnityEngine.Events;

namespace QandAPuzzle
{
    public class PuzzleValidator : MonoBehaviour
    {
        public PlatformSpawner platformSpawner; // Reference to PlatformSpawner
        public QuestionManager questionManager; // Reference to QuestionManager
        public UnityEvent CompletionEvent;      // Event triggered when the puzzle is complete
        private bool isPuzzleComplete = false;

        private void Start()
        {
            if (platformSpawner == null)
            {
                Debug.LogError("PlatformSpawner is not assigned in PuzzleValidator.");
                platformSpawner = FindObjectOfType<PlatformSpawner>();
                if (platformSpawner == null)
                {
                    Debug.LogError("PlatformSpawner not found in the scene.");
                }
            }

            if (questionManager == null)
            {
                Debug.LogError("QuestionManager is not assigned in PuzzleValidator.");
                questionManager = FindObjectOfType<QuestionManager>();
                if (questionManager == null)
                {
                    Debug.LogError("QuestionManager not found!");
                }
            }
        }

        public void ValidateAnswer()
        {
            if (isPuzzleComplete)
            {
                return; // Prevent validation if the puzzle is already complete
            }

            // Debug log for initial validation state
            Debug.Log("Starting answer validation...");
            bool isCorrect = true;

            foreach (Platform platform in platformSpawner.SpawnedPlatforms)
            {
                char expected = platform.GetExpectedLetter();
                char assigned = platform.GetAssignedLetter();

                Debug.Log($"Platform Validation - Expected: {expected}, Assigned: {assigned}");

                if (assigned != expected)
                {
                    isCorrect = false;
                    platform.MarkAsIncorrect();
                }
                else
                {
                    platform.MarkAsCorrect();
                }
            }

            // Provide feedback via QuestionManager
            questionManager.DisplayAnswerFeedback(isCorrect);

            if (isCorrect)
            {
                Debug.Log("Puzzle validated as correct! Triggering completion.");
                questionManager.CompletePuzzle();  // Use CompletePuzzle instead of DisplayCompletion
                CompletionEvent?.Invoke();        // Invoke the completion event if assigned
                isPuzzleComplete = true;          // Mark the puzzle as complete
            }
            else
            {
                Debug.Log("Puzzle validation failed. Not all platforms are correct.");
            }
        }
    }
}
