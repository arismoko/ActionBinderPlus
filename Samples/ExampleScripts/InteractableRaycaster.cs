using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableRaycaster : MonoBehaviour
{
    public Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                InteractableItem interactableItem = hit.collider.GetComponent<InteractableItem>();
                if (interactableItem != null)
                {
                    interactableItem.Interacted();
                }
            }
        }
    }
}
