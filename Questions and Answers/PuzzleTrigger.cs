using UnityEngine;

namespace QandAPuzzle
{
    public class PuzzleTrigger : MonoBehaviour
    {
        public QuestionManager questionManager;  // Reference to the QuestionManager
        public PlatformSpawner platformSpawner;  // Reference to the PlatformSpawner
        private bool puzzleTriggered = false;  // Flag to check if the puzzle has already been triggered

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player is the one entering the trigger area and if the puzzle hasn't been triggered yet
            if (other.CompareTag("Player") && !puzzleTriggered)
            {
                Debug.Log("Player has entered the trigger area!");

                // Trigger a random question when the player enters
                questionManager.DisplayRandomQuestion();

                // Spawn platforms based on the answer length
                string currentAnswer = questionManager.GetCurrentAnswer();
                platformSpawner.SpawnPlatforms(currentAnswer.Length, currentAnswer);

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
