using UnityEngine;
using TMPro;
using UnityEditor; // For Editor functionality
using UnityEngine.Events;

public class SimpleEventTriggerWithUI : MonoBehaviour
{
    public UnityEvent eventA; // Event A (e.g., "Light On")
    public UnityEvent eventB; // Event B (e.g., "Light Off")
    public float timerDuration = 5f; // Duration of the timer

    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI component

    private float timer; 

    void Start()
    {
        // Initially trigger Event A
        eventA.Invoke();
        UpdateTimerText(); // Update the timer text initially
    }

    public void TriggerEvent()
    {
        // Trigger Event B
        eventB.Invoke();
        timer = timerDuration; // Initialize timer with duration when triggered
        UpdateTimerText(); 
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            // Reset timer and trigger Event A
            timer = 0f; 
            eventA.Invoke();
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timer).ToString(); 
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SimpleEventTriggerWithUI))]
    public class SimpleEventTriggerWithUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SimpleEventTriggerWithUI script = (SimpleEventTriggerWithUI)target;

            if (GUILayout.Button("Trigger Event"))
            {
                script.TriggerEvent();
            }
        }
    }
#endif
}