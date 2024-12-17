using UnityEngine;
using UnityEngine.Events;

namespace QandAPuzzle
{
    public class Letter : MonoBehaviour
    {
        [SerializeField] private char letter;

        // Event triggered before the object is destroyed
        public UnityEvent OnBeforeDestroy;

        // Delay before destroying the object (in seconds)
        public float destroyDelay = 0f;

        // Set the letter this object represents
        public void SetLetter(char letter)
        {
            this.letter = letter;
        }

        // Get the letter this object represents
        public char GetLetter()
        {
            return letter;
        }

        // Handle when the letter is placed on a platform
        public void PlaceOnPlatform(Platform platform)
        {
            platform.AssignLetter(letter);
        }

        // Destroy the game object with optional delay and event
        public void DestroyLetter()
        {
            // Trigger the event if any listeners are attached
            OnBeforeDestroy?.Invoke();

            // Destroy the object with a delay
            if (destroyDelay > 0f)
            {
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
