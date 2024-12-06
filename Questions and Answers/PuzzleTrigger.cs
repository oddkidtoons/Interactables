using UnityEngine;
using UnityEngine.Events;

namespace QandAPuzzle
{
    public class PuzzleTrigger : MonoBehaviour
    {
        public QuestionManager questionManager;  // Reference to the QuestionManager
        public PlatformSpawner platformSpawner;  // Reference to the PlatformSpawner
        public UnityEvent OnPuzzleTriggered;     // Event triggered when the puzzle starts
        public bool startTimerOnTrigger = true;  // Option to start the timer when triggered
public string triggerTag;
        private bool puzzleTriggered = false;    // Flag to check if the puzzle has already been triggered

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player is the one entering the trigger area and if the puzzle hasn't been triggered yet
            if (other.CompareTag(triggerTag) && !puzzleTriggered)
            {
                Debug.Log("Player has entered the trigger area!");

                // Trigger a random question when the player enters
                questionManager.DisplayRandomQuestion();

                // Spawn platforms based on the answer length
                string currentAnswer = questionManager.GetCurrentAnswer();
                platformSpawner.SpawnPlatforms(currentAnswer.Length, currentAnswer);

                // Start the timer if the option is enabled
                if (startTimerOnTrigger)
                {
                    questionManager.StartTimer();
                }

                // Trigger any additional events
                OnPuzzleTriggered?.Invoke();

                // Set the flag to true to prevent further triggering
                puzzleTriggered = true;
            }
        }

        // Optionally, you can reset the puzzleTriggered flag in case you want to re-enable the trigger (e.g., between levels)
        public void ResetPuzzleTrigger()
        {
            puzzleTriggered = false;
        }
    }
}

