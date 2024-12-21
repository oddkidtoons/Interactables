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
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;

        private string currentAnswer;

        public UnityEvent OnCompletionEvent;
        public UnityEvent OnTimerEnd;
        public UnityEvent OnFailureEvent;

        public string failedText = "You Failed!";
        public float correctFeedbackDelay = 1f;
        public float correctFeedbackHideDelay = 2f;
        public float completionUIShowDelay = 1f;
        public float completionUIHideDelay = 2f;
        public float scoreUIHideDelay = 2f;
        public float timerUIHideDelay = 2f;
        public float failureUIHideDelay = 2f;
        public float resetDelay = 2f; // Delay before resetting the puzzle

        public PlatformSpawner platformSpawner;
        public LetterSpawner letterSpawner; // Reference to the LetterSpawner
        public ScoreManager scoreManager;
        public PuzzleTrigger puzzleTrigger; // Reference to reset the trigger

        public bool countDown = true;
        public float timerDuration = 120f;

        private float timer;
        private bool timerRunning = false;
        private bool isPuzzleComplete = false;

        void Start()
        {
            InitializeUI();
            ResetTimer();
        }

        void Update()
        {
            if (timerRunning)
            {
                UpdateTimer();
            }
        }

        private void InitializeUI()
        {
            questionText?.gameObject.SetActive(false);
            answerText?.gameObject.SetActive(false);
            completionText?.gameObject.SetActive(false);
            scoreText?.gameObject.SetActive(false);
            timerText?.gameObject.SetActive(false);
        }

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

        private void UpdateTimerUI()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timer / 60);
                int seconds = Mathf.FloorToInt(timer % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                if (countDown)
                {
                    if (timer <= timerDuration * 0.1f)
                        timerText.color = Color.red;
                    else if (timer <= timerDuration * 0.8f)
                        timerText.color = Color.yellow;
                    else
                        timerText.color = Color.white;
                }
            }
        }

        private void TimerEnded()
        {
            timerRunning = false;
            Debug.Log("Timer ended!");
            if (isPuzzleComplete)
                return;

            if (OnFailureEvent != null)
            {
                OnFailureEvent.Invoke();
                ShowFailureUI();
                StartCoroutine(ResetPuzzleAfterDelay());
            }
        }

        private IEnumerator ResetPuzzleAfterDelay()
        {
            yield return new WaitForSeconds(resetDelay);

            Debug.Log("Resetting the puzzle...");
            platformSpawner.SpawnedPlatforms.Clear(); // Clear platforms
            letterSpawner?.DespawnLetters(); // Clear letters
            InitializeUI();
            ResetTimer();

            // Reset the trigger so the player can interact with it again
            if (puzzleTrigger != null)
            {
                puzzleTrigger.ResetPuzzleTrigger();
            }
        }

        public void StartTimer()
        {
            timerRunning = true;
            timerText?.gameObject.SetActive(true);
        }

        public void StopTimer()
        {
            timerRunning = false;
        }

        public void ResetTimer()
        {
            timer = countDown ? timerDuration : 0;
            timerRunning = false;
            UpdateTimerUI();
        }

        public void DisplayRandomQuestion()
        {
            var (question, answer) = GetRandomQuestion();

            questionText?.gameObject.SetActive(true);
            questionText.text = question;

            answerText?.gameObject.SetActive(true);
            answerText.text = "";

            completionText?.gameObject.SetActive(false);
            isPuzzleComplete = false;

            // Trigger the platform spawner
            platformSpawner.SpawnPlatforms(answer.Length, answer);

            // Trigger the letter spawner
            letterSpawner?.SpawnLetters(answer);
        }

        public void DisplayAnswerFeedback(bool isCorrect)
        {
            if (isPuzzleComplete)
                return;

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
                questionText?.gameObject.SetActive(false);
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
            answerText.text = "";
        }

        public void CompletePuzzle()
        {
            if (isPuzzleComplete)
                return;

            Debug.Log("Completing puzzle...");
            isPuzzleComplete = true;

            StopTimer();
            StartCoroutine(ShowCompletionUI());
            StartCoroutine(FlashTimerGreen());
            StartCoroutine(HideScoreAfterDelay());

            scoreManager.IncreaseScore();
            UpdateScoreUI();

            if (OnCompletionEvent != null)
            {
                OnCompletionEvent.Invoke();
            }
        }

        private IEnumerator ShowCompletionUI()
        {
            yield return new WaitForSeconds(completionUIShowDelay);
            completionText?.gameObject.SetActive(true);
            completionText.text = "Complete!";
            yield return new WaitForSeconds(completionUIHideDelay);
            completionText?.gameObject.SetActive(false);
        }

        private IEnumerator FlashTimerGreen()
        {
            if (timerText == null)
                yield break;

            for (int i = 0; i < 6; i++)
            {
                timerText.color = i % 2 == 0 ? Color.green : Color.white;
                yield return new WaitForSeconds(0.2f);
            }

            timerText.color = Color.white;
            yield return new WaitForSeconds(timerUIHideDelay);
            timerText?.gameObject.SetActive(false);
        }

        private IEnumerator HideScoreAfterDelay()
        {
            yield return new WaitForSeconds(scoreUIHideDelay);
            scoreText?.gameObject.SetActive(false);
        }

        private void ShowFailureUI()
        {
            if (questionText != null)
            {
                questionText.gameObject.SetActive(true);
                questionText.text = failedText;
                StartCoroutine(HideFailureUI());
            }
        }

        private IEnumerator HideFailureUI()
        {
            yield return new WaitForSeconds(failureUIHideDelay);
            questionText?.gameObject.SetActive(false);
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
                scoreText.gameObject.SetActive(true);
            }
        }
    }
}
