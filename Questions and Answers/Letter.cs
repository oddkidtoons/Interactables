using UnityEngine;

namespace QandAPuzzle
{
    public class Letter : MonoBehaviour
    {
        // Make this public or use SerializeField to expose it in the inspector
        [SerializeField] private char letter;

        // Set the letter this object represents (you can still use this method)
        public void SetLetter(char letter)
        {
            this.letter = letter;
        }

        // Get the letter this object represents
        public char GetLetter()
        {
            return letter;
        }

        // You can implement logic to handle when the letter is placed on a platform
        public void PlaceOnPlatform(Platform platform)
        {
            platform.AssignLetter(letter);  // Assign the letter to the platform
        }
    }
}

