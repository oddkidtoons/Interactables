using UnityEngine;
using UnityEngine.Events;

namespace QandAPuzzle
{
    public class PuzzleTrigger : MonoBehaviour
    {
        public QuestionManager questionManager;  // Reference to the QuestionManager
        public PlatformSpawner platformSpawner;  // Reference to the PlatformSpawner
        public UnityEvent OnPuzzleTriggered;     // Event triggered when the puzzle starts
        public UnityEvent OnTriggerReset;        // Event triggered when the trigger is reset

        public bool startTimerOnTrigger = true;  // Option to start the timer when triggered
        public string triggerTag;                // Tag to identify the player

        private bool puzzleTriggered = false;    // Flag to check if the puzzle has already been triggered

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player enters the trigger area and the puzzle hasn't been triggered yet
            if (other.CompareTag(triggerTag) && !puzzleTriggered)
            {
                Debug.Log("Player has entered the trigger area!");

                // Trigger a random question when the player enters
                questionManager.DisplayRandomQuestion();

                // Spawn platforms based on the answer length
                string currentAnswer = questionManager.GetCurrentAnswer();
                platformSpawner.SpawnPlatforms(currentAnswer.Length, currentAnswer);

                // Start the timer if enabled
                if (startTimerOnTrigger)
                {
                    questionManager.StartTimer();
                }

                // Trigger additional events
                OnPuzzleTriggered?.Invoke();

                // Set the flag to true to prevent further triggering
                puzzleTriggered = true;
            }
        }

        // Reset the trigger and allow the puzzle to be restarted
        public void ResetPuzzleTrigger()
        {
            Debug.Log("Resetting Puzzle Trigger...");
            puzzleTriggered = false; // Allow the trigger to be activated again

            // Invoke the reset visual/event for player feedback
            OnTriggerReset?.Invoke();
        }
    }
}
