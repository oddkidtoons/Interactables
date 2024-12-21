using UnityEngine;
using System.Collections.Generic;

namespace QandAPuzzle
{
    public class LetterSpawner : MonoBehaviour
    {
        [Tooltip("List of letter prefabs. Each prefab should have the Letter script attached.")]
        public List<GameObject> letterPrefabs;

        [Tooltip("The area in which letters will be spawned.")]
        public Transform spawnArea;

        [Tooltip("How many random letters to spawn in addition to the required letters.")]
        public int extraRandomLetters = 0;

        [Tooltip("The bounds for randomizing letter positions (relative to the spawn area).")]
        public Vector3 spawnBounds = new Vector3(10f, 0f, 10f);

        [Tooltip("The minimum distance between spawned letters.")]
        public float minimumLetterDistance = 2f;

        private List<Letter> spawnedLetters = new List<Letter>();

        // Spawn letters based on the required answer
        public void SpawnLetters(string answer)
        {
            // Clear any existing letters
            DespawnLetters();

            List<char> allLetters = new List<char>(answer.ToCharArray());

            // Generate unique extra random letters
            HashSet<char> extraLetters = GenerateUniqueRandomLetters(answer);

            // Add the extra letters to the list
            allLetters.AddRange(extraLetters);

            // Shuffle the letters for randomized order
            ShuffleList(allLetters);

            // Spawn the letters
            foreach (char letterChar in allLetters)
            {
                Vector3 spawnPosition = GetRandomValidSpawnPosition();
                GameObject letterPrefab = GetLetterPrefab(letterChar);

                if (letterPrefab != null)
                {
                    GameObject letterObj = Instantiate(letterPrefab, spawnPosition, Quaternion.identity, spawnArea);
                    Letter letter = letterObj.GetComponent<Letter>();

                    // Assign the letter character
                    letter.SetLetter(letterChar);

                    // Add to the list of spawned letters
                    spawnedLetters.Add(letter);
                }
                else
                {
                    Debug.LogError($"No prefab found for letter '{letterChar}'. Ensure a matching prefab exists.");
                }
            }
        }

        // Remove all spawned letters
        public void DespawnLetters()
        {
            foreach (Letter letter in spawnedLetters)
            {
                if (letter != null)
                {
                    Destroy(letter.gameObject);
                }
            }

            spawnedLetters.Clear();
        }

        // Get the prefab for a specific letter
        private GameObject GetLetterPrefab(char letterChar)
        {
            foreach (GameObject prefab in letterPrefabs)
            {
                Letter letter = prefab.GetComponent<Letter>();
                if (letter != null && letter.GetLetter() == letterChar)
                {
                    return prefab;
                }
            }

            return null; // No matching prefab found
        }

        // Get a random valid position within the spawn area bounds, ensuring minimum distance between letters
        private Vector3 GetRandomValidSpawnPosition()
        {
            Vector3 position;
            int attempts = 0;
            do
            {
                position = GetRandomSpawnPosition();
                attempts++;
            }
            while (!IsPositionValid(position) && attempts < 100); // Prevent infinite loops

            return position;
        }

        // Check if a position is valid by ensuring it's far enough from all previously spawned letters
        private bool IsPositionValid(Vector3 position)
        {
            foreach (Letter letter in spawnedLetters)
            {
                if (letter != null && Vector3.Distance(letter.transform.position, position) < minimumLetterDistance)
                {
                    return false;
                }
            }

            return true;
        }

        // Get a random position within the spawn area bounds
        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 offset = new Vector3(
                Random.Range(-spawnBounds.x / 2, spawnBounds.x / 2),
                0f, // Assuming letters are spawned on the same Y level
                Random.Range(-spawnBounds.z / 2, spawnBounds.z / 2)
            );

            return spawnArea.position + offset;
        }

        // Generate unique random letters that do not overlap with the answer
        private HashSet<char> GenerateUniqueRandomLetters(string answer)
        {
            HashSet<char> uniqueLetters = new HashSet<char>(answer); // Start with letters in the answer
            HashSet<char> extraLetters = new HashSet<char>();

            while (extraLetters.Count < extraRandomLetters)
            {
                char randomLetter = GetRandomLetter();

                // Add only if it's not in the answer or already in extra letters
                if (!uniqueLetters.Contains(randomLetter))
                {
                    extraLetters.Add(randomLetter);
                }
            }

            return extraLetters;
        }

        // Get a random letter (A-Z)
        private char GetRandomLetter()
        {
            return (char)Random.Range(65, 91); // ASCII values for A-Z
        }

        // Shuffle a list using Fisher-Yates algorithm
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
