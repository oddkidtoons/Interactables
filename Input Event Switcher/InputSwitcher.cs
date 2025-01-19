using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputSwitcher : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;

    [SerializeField] private InputActionReference selectActionReference; 

    [SerializeField] private UnityEvent onEventA;
    [SerializeField] private UnityEvent onEventB;

    private bool isEventAActive = true;

    private void OnEnable()
    {
        if (selectActionReference == null || selectActionReference.action == null)
        {
            Debug.LogError("SelectActionReference or its underlying InputAction is not assigned!");
            return;
        }

        // Enable the action
        selectActionReference.action.Enable();

        // Subscribe to performed event
        selectActionReference.action.performed += OnSelectActionPerformed;
    }

    private void OnDisable()
    {
        if (selectActionReference != null && selectActionReference.action != null)
        {
            selectActionReference.action.performed -= OnSelectActionPerformed;
        }
    }

    private void OnSelectActionPerformed(InputAction.CallbackContext context)
    {
        isEventAActive = !isEventAActive;
        Debug.Log($"Button Pressed. isEventAActive: {isEventAActive}");

        if (isEventAActive)
        {
            Debug.Log("Invoking Event A");
            onEventA.Invoke();
        }
        else
        {
            Debug.Log("Invoking Event B");
            onEventB.Invoke();
        }
    }
}
