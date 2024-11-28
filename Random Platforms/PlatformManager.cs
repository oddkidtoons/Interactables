using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Management")]
    public List<RandomSwitchingPlatform> platforms; // List of linked platforms
    public float minSwitchTime = 1f;                // Minimum time between state switches
    public float maxSwitchTime = 5f;                // Maximum time between state switches
    public bool alwaysEnsureSafePath = true;        // Ensure there's always a safe platform close to the player
    public float safePlatformDistance = 10f;        // Distance from the player to prioritize safe platforms
    public float safePlatformActivationDelay = 1f;  // Time before a platform becomes safe near the player
    public float safePlatformDuration = 3f;         // Time a platform remains safe after activation
    public bool useRhythm = true;                   // Whether to enable rhythmic platform switching
    public float rhythmInterval = 2f;               // Interval for rhythm-based switching

    [Header("Player Settings")]
    public string playerTag = "Player";            // Tag assigned to the player

    private Transform playerTransform;

    private void Start()
    {
        // Locate the player by tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the player has the correct tag.");
            return;
        }

        playerTransform = player.transform;

        if (platforms == null || platforms.Count == 0)
        {
            Debug.LogError("No platforms assigned to the PlatformManager!");
            return;
        }

        // Start managing platform behavior
        if (useRhythm)
        {
            StartCoroutine(RhythmBasedSwitching());
        }
        else
        {
            StartCoroutine(ManagePlatforms());
        }
    }

    /// <summary>
    /// Rhythm-based platform switching logic.
    /// Platforms become safe in a fixed sequence or predictable pattern.
    /// </summary>
    private IEnumerator RhythmBasedSwitching()
    {
        while (true)
        {
            foreach (var platform in platforms)
            {
                yield return new WaitForSeconds(rhythmInterval);

                // Check distance to player and activate if within range
                float distanceToPlayer = Vector3.Distance(playerTransform.position, platform.transform.position);
                if (distanceToPlayer <= safePlatformDistance)
                {
                    StartCoroutine(ActivatePlatformTemporarily(platform, safePlatformDuration));
                }
                else
                {
                    // Randomize the platform if out of range
                    platform.SetPlatformState(Random.value > 0.5f);
                }
            }
        }
    }

    /// <summary>
    /// Manages platforms based on distance and timing.
    /// </summary>
    private IEnumerator ManagePlatforms()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSwitchTime, maxSwitchTime));

            foreach (var platform in platforms)
            {
                float distanceToPlayer = Vector3.Distance(playerTransform.position, platform.transform.position);
                if (distanceToPlayer <= safePlatformDistance)
                {
                    StartCoroutine(ActivatePlatformTemporarily(platform, safePlatformDuration));
                }
                else
                {
                    // Randomize the platform if the player is far away
                    platform.SetPlatformState(Random.value > 0.5f);
                }
            }
        }
    }

    /// <summary>
    /// Temporarily activates a platform as safe and then deactivates it.
    /// </summary>
    private IEnumerator ActivatePlatformTemporarily(RandomSwitchingPlatform platform, float duration)
    {
        yield return new WaitForSeconds(safePlatformActivationDelay); // Delay before becoming safe
        platform.SetPlatformState(true); // Make the platform safe

        yield return new WaitForSeconds(duration); // Wait for the safe duration
        platform.SetPlatformState(false); // Make the platform unsafe again
    }
}
