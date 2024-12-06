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
        public TextMeshProUGUI timerText; // Reference to the Timer UI Text

        private string currentAnswer;

        public UnityEvent OnCompletionEvent;
        public UnityEvent OnTimerEnd;  // Event triggered when the timer ends

        private bool isPuzzleComplete = false;  // Centralized completion state

        // Public delays to adjust in the Inspector
        public float correctFeedbackDelay = 1f;
        public float correctFeedbackHideDelay = 2f;
        public float completionUIShowDelay = 1f;
        public float completionUIHideDelay = 2f;

        public PlatformSpawner platformSpawner;
        public ScoreManager scoreManager;

        // Timer variables
        public bool countDown = true;           // Choose between counting down or counting up
        public float timerDuration = 120f;     // Timer duration in seconds
        private float timer;                   // Current timer value
        private bool timerRunning = false;     // Tracks whether the timer is running

        void Start()
        {
            questionText.gameObject.SetActive(false);


            // Initialize timer
            timer = countDown ? timerDuration : 0;
            if (timerText != null)
            {
                UpdateTimerUI(); // Display initial timer value
            }

            //DisplayRandomQuestion();
        }

        void Update()
        {
            if (timerRunning)
            {
                UpdateTimer();
            }
        }

        // Timer logic
        private void UpdateTimer()
        {
            if (countDown)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = 0;
                    TimerEnded();
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= timerDuration)
                {
                    timer = timerDuration;
                    TimerEnded();
                }
            }

            UpdateTimerUI();
        }

        // Updates the timer display
        private void UpdateTimerUI()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timer / 60);
                int seconds = Mathf.FloorToInt(timer % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        // Event triggered when the timer ends
        private void TimerEnded()
        {
            timerRunning = false;
            Debug.Log("Timer ended!");
            if (OnTimerEnd != null)
            {
                Debug.Log("Triggering OnTimerEnd event.");
                OnTimerEnd.Invoke(); // Trigger the timer end event
            }
        }

        // Start the timer
        public void StartTimer()
        {
            timerRunning = true;
        }

        // Stop the timer
        public void StopTimer()
        {
            timerRunning = false;
        }

        // Reset the timer
        public void ResetTimer()
        {
            timer = countDown ? timerDuration : 0;
            timerRunning = false;
            UpdateTimerUI();
        }

        public void DisplayRandomQuestion()
        {
            var (question, _) = GetRandomQuestion();

            if (questionText != null)
            {
                questionText.gameObject.SetActive(true);
                questionText.text = question;
            }

            if (answerText != null)
            {
                answerText.gameObject.SetActive(true);
                answerText.text = "";
            }

            if (completionText != null)
            {
                completionText.gameObject.SetActive(false); // Hide completion UI
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
                StartCoroutine(ShowAnswerFeedback(isCorrect));
            }
        }

        private IEnumerator ShowAnswerFeedback(bool isCorrect)
        {
            yield return new WaitForSeconds(correctFeedbackHideDelay);

            if (isCorrect)
            {
                questionText.gameObject.SetActive(false);
                yield return new WaitForSeconds(correctFeedbackDelay);
                answerText.text = "Correct!";
                StartCoroutine(ClearAnswerFeedback(correctFeedbackHideDelay));
            }
            else
            {
                answerText.text = "Wrong answer. Try again!";
            }
        }

        private IEnumerator ClearAnswerFeedback(float delay)
        {
            yield return new WaitForSeconds(delay);
            answerText.text = ""; // Clear the feedback
        }

        public void CompletePuzzle()
        {
            if (isPuzzleComplete)
            {
                Debug.Log("Puzzle already complete. Skipping completion logic.");
                return;
            }

            Debug.Log("Completing puzzle...");
            isPuzzleComplete = true;

            // Display completion UI and trigger completion events
            StartCoroutine(ShowCompletionUI());

            scoreManager.IncreaseScore();
            UpdateScoreUI();

            if (OnCompletionEvent != null)
            {
                Debug.Log("Triggering OnCompletionEvent.");
                OnCompletionEvent.Invoke(); // Trigger external completion event
            }
        }

        private IEnumerator ShowCompletionUI()
        {
            yield return new WaitForSeconds(completionUIShowDelay);

            if (completionText != null)
            {
                completionText.gameObject.SetActive(true);
                completionText.text = "Complete!";
                Debug.Log($"Completion UI Active: {completionText.gameObject.activeSelf}");
            }

            yield return new WaitForSeconds(completionUIHideDelay);

            if (completionText != null)
            {
                completionText.gameObject.SetActive(false);
                Debug.Log("Completion UI Hidden.");
            }
        }

        public void CheckPuzzleCompletion()
        {
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

            if (allPlatformsOccupied && anyIncorrect)
            {
                DisplayAnswerFeedback(false);
            }
            else if (allPlatformsOccupied && !anyIncorrect)
            {
                CompletePuzzle(); // Centralized completion logic
            }
        }

        public (string, string) GetRandomQuestion()
        {
    
            int index = Random.Range(0, questions.Length);
            string question = questions[index];
            currentAnswer = answers[index];
            return (question, currentAnswer);
        }

        public string GetCurrentAnswer()
        {
            return currentAnswer;
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + scoreManager.GetScore();
            }
        }
    }
}
