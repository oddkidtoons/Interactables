using UnityEngine;

public class RandomSwitchingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public Material safeMaterial;         // Material for the "safe" state
    public Material unsafeMaterial;       // Material for the "unsafe" state

    [Header("Platform Components")]
    private MeshRenderer platformRenderer; // The platform's visual component
    private Collider platformCollider;     // The platform's collider

    private void Start()
    {
        // Get platform components
        platformRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();

        if (platformRenderer == null || platformCollider == null)
        {
            Debug.LogError("Platform is missing a MeshRenderer or Collider component.");
            return;
        }

        // Initialize platform state
        SetPlatformState(false);
    }

    /// <summary>
    /// Updates the platform's material and collider based on its state.
    /// </summary>
    /// <param name="isSafe">Whether the platform is safe or not.</param>
    public void SetPlatformState(bool isSafe)
    {
        if (isSafe)
        {
            platformRenderer.material = safeMaterial;
            platformCollider.enabled = true; // Enable collision
        }
        else
        {
            platformRenderer.material = unsafeMaterial;
            platformCollider.enabled = false; // Disable collision
        }
    }
}
