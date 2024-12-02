using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

namespace QandAPuzzle
{
    public class QuestionManager : MonoBehaviour
    {
        [SerializeField] private string[] questions = { "What is 2 + 2?", "How many continents?", "What is 10 divided by 2?" };
        [SerializeField] private string[] answers = { "4", "7", "5" };

        public TextMeshProUGUI questionText;
        public TextMeshProUGUI answerText;
        public TextMeshProUGUI completionText;
        public TextMeshProUGUI scoreText;  // Reference to the Score UI Text
        private string currentAnswer;

        public UnityEvent OnCompletionEvent;
        private bool isPuzzleComplete = false;

        // Public delays to adjust in the Inspector
        public float correctFeedbackDelay = 1f; // Delay before showing the "Correct!" UI feedback
        public float correctFeedbackHideDelay = 2f; // Delay for clearing the answer feedback (Correct/Incorrect message)
       
        public float completionUIShowDelay = 1f; // Delay before showing the completion UI
        public float completionUIHideDelay = 2f; // Delay for hiding the completion UI

        public PlatformSpawner platformSpawner; // Reference to PlatformSpawner
        public ScoreManager scoreManager;  // Reference to ScoreManager

        public void DisplayRandomQuestion()
        {
            var (question, _) = GetRandomQuestion();

            if (questionText != null)
            {
                questionText.text = question;
            }

            if (answerText != null)
            {
                answerText.text = "";
            }

            if (completionText != null)
            {
                completionText.gameObject.SetActive(false);  // Hide completion UI
            }

            isPuzzleComplete = false; // Reset the completion status when a new question is displayed
        }

        public void DisplayAnswerFeedback(bool isCorrect)
        {
            if (isPuzzleComplete) 
            {
                return; // If the puzzle is complete, do not show feedback again
            }

            Debug.Log($"DisplayAnswerFeedback: {isCorrect}");
            if (answerText != null)
            {
                StartCoroutine(ShowAnswerFeedback(isCorrect));  // Start a coroutine to show the answer feedback with delay
            }
        }

        private IEnumerator ShowAnswerFeedback(bool isCorrect)
        {
            // Delay before showing the feedback
            yield return new WaitForSeconds(correctFeedbackHideDelay);

            if (isCorrect)
            {
                questionText.gameObject.SetActive(false);
                yield return new WaitForSeconds(correctFeedbackDelay); // Delay before showing the "Correct!" message
                answerText.text = "Correct!";
                StartCoroutine(ClearAnswerFeedback(correctFeedbackHideDelay));  // Hide "Correct!" after another delay
            }
            else
            {
                answerText.text = "Wrong answer. Try again!";
            }
        }

        private IEnumerator ClearAnswerFeedback(float delay)
        {
            yield return new WaitForSeconds(delay);
            answerText.text = "";  // Clear the feedback
        }

        public void DisplayCompletion()
        {
            // If the puzzle is already complete, prevent triggering completion UI again
            if (isPuzzleComplete)
            {
                return;
            }

            Debug.Log("DisplayCompletion triggered!");
            if (completionText != null)
            {
                StartCoroutine(ShowCompletionUI());
            }

            isPuzzleComplete = true;  // Mark the puzzle as complete

            // Increase score and update the UI
            scoreManager.IncreaseScore();
            UpdateScoreUI();
        }

        private IEnumerator ShowCompletionUI()
        {
            // Delay before showing the completion UI
            yield return new WaitForSeconds(completionUIShowDelay);

            if (completionText != null)
            {
                completionText.gameObject.SetActive(true);  // Show the completion UI
                completionText.text = "Complete!";
                Debug.Log($"Completion UI Active: {completionText.gameObject.activeSelf}");
            }

            // Delay before hiding the completion UI
            yield return new WaitForSeconds(completionUIHideDelay);
            
            if (completionText != null)
            {
                completionText.gameObject.SetActive(false);  // Hide after the delay
                Debug.Log("Completion UI Hidden.");
            }

            OnCompletionEvent.Invoke();  // Trigger the completion event
        }

        public void CheckPuzzleCompletion()
        {
            // Check if all platforms are occupied and if any are incorrect
            bool allPlatformsOccupied = true;
            bool anyIncorrect = false;

            foreach (var platform in platformSpawner.SpawnedPlatforms)
            {
                if (!platform.GetAssignedLetter().Equals(platform.GetExpectedLetter()))
                {
                    anyIncorrect = true;
                }
                if (platform.GetAssignedLetter() == '\0') // Empty letter means platform not filled
                {
                    allPlatformsOccupied = false;
                }
            }

            // Show "Wrong Answer" UI if all platforms are occupied and any are incorrect
            if (allPlatformsOccupied && anyIncorrect)
            {
                DisplayAnswerFeedback(false);  // Display the "Wrong Answer" message
            }
            else if (allPlatformsOccupied && !anyIncorrect)
            {
                DisplayCompletion();  // All letters are correct, complete the puzzle
            }
        }

        public (string, string) GetRandomQuestion()
        {
            int index = Random.Range(0, questions.Length);
            string question = questions[index];
            currentAnswer = answers[index];  // Store the current answer
            return (question, currentAnswer);
        }

        public string GetCurrentAnswer()
        {
            return currentAnswer;
        }

        // Method to update the score UI
        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + scoreManager.GetScore();  // Update the score display
            }
        }
    }
}