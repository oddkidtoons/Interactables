using System.Collections;
using UnityEngine;

public class PlatformBridgePiece : MonoBehaviour
{
    public bool IsSafe; // Current state of the platform
    public float WarningDuration = 0.5f; // Duration of the blinking effect
    public Color SafeColor = Color.green; // Color when platform is safe
    public Color EarlyWarningColor = Color.yellow; // Color during early warning (<50% of timer)
    public Color LateWarningColor = Color.red; // Color during late warning (>50% of timer)
    public Color UnsafeColor = Color.gray; // Color when platform is unsafe
    private Renderer platformRenderer;
    private Collider platformCollider;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();
    }

    // Sets the platform's state and visual appearance
    public void SetState(bool safe)
    {
        IsSafe = safe;
        platformCollider.enabled = safe; // Enable or disable collider
        platformRenderer.material.color = safe ? SafeColor : UnsafeColor; // Safe is green, unsafe is gray
    }

    // Blinks the platform as a warning before it becomes unsafe
    public IEnumerator WarnBeforeUnsafe(float timeLeft)
    {
        float totalWarningTime = WarningDuration;
        while (timeLeft > 0)
        {
            float blinkSpeed = Mathf.Lerp(1f, 5f, 1f - (timeLeft / totalWarningTime));
            if (timeLeft / totalWarningTime > 0.5f)
            {
                // Early warning phase (<50% of timer)
                platformRenderer.material.color = (timeLeft % (1 / blinkSpeed) > (1 / blinkSpeed) / 2) ? 
                    EarlyWarningColor : SafeColor;
            }
            else
            {
                // Late warning phase (>50% of timer)
                platformRenderer.material.color = (timeLeft % (1 / blinkSpeed) > (1 / blinkSpeed) / 2) ? 
                    LateWarningColor : UnsafeColor;
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    // Trigger safe state externally
    public void TriggerSafeState()
    {
        SetState(true);
    }

    // Trigger unsafe state externally
    public void TriggerUnsafeState()
    {
        SetState(false);
    }
}