using UnityEngine;

namespace QandAPuzzle
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private char assignedLetter;  // Letter currently placed on the platform
        private char expectedLetter;  // The correct letter for this platform
        private bool isOccupied = false;  // Tracks if the platform has a letter on it
        public PlatformSpawner platformSpawner; // Reference to PlatformSpawner

        // Public colors for correct and incorrect letters
        public Color correctColor = Color.green;  // Default color for correct letters
        public Color incorrectColor = Color.red;  // Default color for incorrect letters
        public Color defaultColor = Color.white;  // Default color when no letter is assigned

        // Method to assign the expected letter (correct answer)
        public void AssignExpectedLetter(char letter)
        {
            expectedLetter = letter;
        }

        // Method to get the expected letter
        public char GetExpectedLetter()
        {
            return expectedLetter;
        }

        // Method to assign the currently placed letter
        public void AssignLetter(char letter)
        {
            assignedLetter = letter;
            isOccupied = true;

            // Update platform state (color) after a new letter is placed
            UpdatePlatformState();
            
            // Check if all platforms are correct after a new letter is placed
            platformSpawner.CheckIfAllPlatformsCorrect();
        }

        // Method to remove the currently placed letter
        public void RemoveLetter()
        {
            assignedLetter = '\0';  // Clear the assigned letter
            isOccupied = false;     // Mark platform as unoccupied
            ResetPlatformState();   // Reset the platform's state to its default (no color)
        }

        // Method to get the assigned letter
        public char GetAssignedLetter()
        {
            return assignedLetter;
        }

        // Update platform state based on the assigned letter
        private void UpdatePlatformState()
        {
            if (assignedLetter == expectedLetter)
            {
                MarkAsCorrect();
            }
            else
            {
                MarkAsIncorrect();
            }
        }

        // Reset platform's state (no letter, no color)
        private void ResetPlatformState()
        {
            GetComponent<Renderer>().material.color = defaultColor;  // Reset to default color
        }

        // Mark platform as correct (e.g., change color to green)
        public void MarkAsCorrect()
        {
            GetComponent<Renderer>().material.color = correctColor;
        }

        // Mark platform as incorrect (e.g., change color to red)
        public void MarkAsIncorrect()
        {
            GetComponent<Renderer>().material.color = incorrectColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Letter") && !isOccupied)
            {
                Letter letterScript = other.GetComponent<Letter>();
                if (letterScript != null)
                {
                    char placedLetter = letterScript.GetLetter();
                    AssignLetter(placedLetter);  // Assign the new letter and update platform state
                }
            }
        }

        // Triggered when a letter is removed from the platform
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Letter") && isOccupied)
            {
                // Optionally, you can add a behavior to handle when a letter leaves the platform
                // For example: Remove the letter and reset state
                RemoveLetter();
            }
        }
    }
}
