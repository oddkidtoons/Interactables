using UnityEngine;
using System.Collections.Generic;

namespace QandAPuzzle
{
    public class PlatformSpawner : MonoBehaviour
    {
        public GameObject platformPrefab;  // Reference to the platform prefab
        public Transform spawnArea;  // The area in which to spawn the platforms
        public List<Platform> SpawnedPlatforms { get; private set; }  // Store the spawned platforms

        // Public spacing variable for setting the platform distance in the Inspector
        public float platformSpacing = 2f;  // Distance between platforms

        public PuzzleValidator puzzleValidator;  // Reference to the PuzzleValidator

        public void SpawnPlatforms(int numberOfPlatforms, string answer)
        {
            SpawnedPlatforms = new List<Platform>();

            // Clear any previously spawned platforms
            foreach (Transform child in spawnArea)
            {
                Destroy(child.gameObject);
            }

            // Spawn the platforms based on the number of letters in the answer
            for (int i = 0; i < numberOfPlatforms; i++)
            {
                GameObject platformObj = Instantiate(platformPrefab, spawnArea.position + new Vector3(i * platformSpacing, 0, 0), Quaternion.identity);
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
    }
}
