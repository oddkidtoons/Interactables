using System.Collections;
using UnityEngine;

public class PlatformBridgeManager : MonoBehaviour
{
    public PlatformBridgePiece[] Row1Platforms;
    public PlatformBridgePiece[] Row2Platforms;
    public float Interval = 2f; // Time interval for switching states
    public bool UseInterval = true; // Determines if platforms toggle at intervals
    public bool TriggerByEvent = false; // Allows toggling via external events
    private float timer;
    private bool isSafeState = true; // Tracks the current state

    public void TriggerSafeState() // External method to trigger safe state
    {
        isSafeState = true;
        TogglePlatforms(Row1Platforms, isSafeState);
        TogglePlatforms(Row2Platforms, !isSafeState);
        timer = 0; // Reset timer to ensure synchronization
    }

    public void TriggerUnsafeState() // External method to trigger unsafe state
    {
        isSafeState = false;
        TogglePlatforms(Row1Platforms, isSafeState);
        TogglePlatforms(Row2Platforms, !isSafeState);
        timer = 0; // Reset timer to ensure synchronization
    }

    void Start()
    {
        // Ensure platforms are set to initial state
        if (!TriggerByEvent)
        {
            TogglePlatforms(Row1Platforms, isSafeState);
            TogglePlatforms(Row2Platforms, !isSafeState);
        }
    }

    void Update()
    {
        if (UseInterval && !TriggerByEvent)
        {
            timer += Time.deltaTime;

            if (timer >= Interval)
            {
                isSafeState = !isSafeState;
                TogglePlatforms(Row1Platforms, isSafeState);
                TogglePlatforms(Row2Platforms, !isSafeState);
                timer = 0;
            }
            else if (timer >= Interval - Row1Platforms[0].WarningDuration)
            {
                StartWarning(Row1Platforms, isSafeState, Interval - timer);
                StartWarning(Row2Platforms, !isSafeState, Interval - timer);
            }
        }
    }

    void TogglePlatforms(PlatformBridgePiece[] platforms, bool safeState)
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].SetState(i % 2 == 0 ? safeState : !safeState);
        }
    }

    void StartWarning(PlatformBridgePiece[] platforms, bool safeState, float timeLeft)
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if ((i % 2 == 0) == safeState)
            {
                StartCoroutine(platforms[i].WarnBeforeUnsafe(timeLeft));
            }
        }
    }
}
