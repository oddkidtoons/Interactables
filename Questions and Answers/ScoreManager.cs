using UnityEngine;

namespace QandAPuzzle
{
    public class ScoreManager : MonoBehaviour
    {
        private int totalCompletions;  // Total completions across levels

        // Key for storing the score in PlayerPrefs
        private const string ScoreKey = "TotalCompletions";

        void Start()
        {
            LoadScore();  // Load the stored score when the game starts
        }

        // Increases the score by 1 (called when a puzzle is completed)
        public void IncreaseScore()
        {
            totalCompletions++;
            SaveScore();  // Save the updated score after increase
        }

        // Saves the current score to PlayerPrefs
        private void SaveScore()
        {
            PlayerPrefs.SetInt(ScoreKey, totalCompletions);
            PlayerPrefs.Save();  // Ensure the score is saved to disk
        }

        // Loads the current score from PlayerPrefs
        private void LoadScore()
        {
            totalCompletions = PlayerPrefs.GetInt(ScoreKey, 0);  // Default to 0 if not found
        }

        // Returns the current total completions score
        public int GetScore()
        {
            return totalCompletions;
        }

        // Resets the score (if needed, e.g., for a new game or level reset)
        public void ResetScore()
        {
            totalCompletions = 0;
            SaveScore();  // Save the reset score
        }

        // Context menu option to reset score from the Unity Inspector
        [ContextMenu("Reset Score")]
        public void ResetScoreFromInspector()
        {
            ResetScore();
        }
    }
}
