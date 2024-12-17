using UnityEngine;
using System.Collections.Generic;

namespace QandAPuzzle
{
    public class PlatformSpawner : MonoBehaviour
    {
        public GameObject platformPrefab;  // Reference to the platform prefab
        public Transform spawnArea;  // The area in which to spawn the platforms
        public List<Platform> SpawnedPlatforms { get; private set; }  // Store the spawned platforms

        public float platformSpacing = 2f;  // Distance between platforms

        public PuzzleValidator puzzleValidator;  // Reference to the PuzzleValidator

        public void SpawnPlatforms(int numberOfPlatforms, string answer)
        {
            SpawnedPlatforms = new List<Platform>();

            // Clear any previously spawned platforms
            DespawnPlatforms();

            // Calculate the starting offset to center the platforms
            float totalWidth = (numberOfPlatforms - 1) * platformSpacing;  // Total width of the platforms
            float startX = -totalWidth / 2;  // Center the platforms around the spawnArea

            // Spawn the platforms
            for (int i = 0; i < numberOfPlatforms; i++)
            {
                float xOffset = startX + (i * platformSpacing);  // Calculate the x-position for each platform

                // Calculate the spawn position and apply the rotation to face the Z-axis of the spawn area
                Vector3 spawnPosition = spawnArea.position + spawnArea.right * xOffset;
                Quaternion spawnRotation = Quaternion.LookRotation(spawnArea.forward, Vector3.up);

                GameObject platformObj = Instantiate(platformPrefab, spawnPosition, spawnRotation);
                platformObj.transform.parent = spawnArea;  // Set the platform as a child of the spawn area
                Platform platform = platformObj.GetComponent<Platform>();

                // Assign the expected letter to the platform
                platform.AssignExpectedLetter(answer[i]);  // Assigns the correct letter for validation
                platform.platformSpawner = this;  // Assign reference to the PlatformSpawner

                SpawnedPlatforms.Add(platform);
            }
        }

        // Method to return all spawned platforms
        public List<Platform> GetSpawnedPlatforms()
        {
            return SpawnedPlatforms;
        }

        // Method to check if all platforms have the correct letter
        public bool AllPlatformsCorrect()
        {
            foreach (Platform platform in SpawnedPlatforms)
            {
                if (platform.GetAssignedLetter() != platform.GetExpectedLetter())
                {
                    return false;
                }
            }
            return true;
        }

        // Method to check if all platforms are correct and trigger the completion
        public void CheckIfAllPlatformsCorrect()
        {
            if (AllPlatformsCorrect())
            {
                puzzleValidator.ValidateAnswer();  // Trigger the validation after all platforms are correct
            }
        }

        // New Method to Despawn Platforms
        public void DespawnPlatforms()
        {
            if (spawnArea == null) return;

            foreach (Transform child in spawnArea)
            {
                Destroy(child.gameObject);
            }

            SpawnedPlatforms?.Clear();
        }
    }
}
