using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class InputActionManager : MonoBehaviour
{
    public static InputActionManager Instance { get; private set; }
    public InputActionMap actionMap;
    public List<InteractableItem> interactableItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        var playerInput = GetComponent<PlayerInput>();
        actionMap = playerInput.currentActionMap;
        interactableItems = new List<InteractableItem>(FindObjectsOfType<InteractableItem>());
    }

    public void CallAction(InteractableItem.InputActionBinding binding, InteractableItem item)
    {
        var targetComponent = item.targetComponent;

        if (targetComponent == null)
        {
            Debug.LogError("Target Component is not set for binding: " + binding.actionName);
            return;
        }

        if (binding.cachedMethod == null)
        {
            Type type = targetComponent.GetType();
            binding.cachedMethod = type.GetMethod(binding.methodName);
            if (binding.cachedMethod == null)
            {
                Debug.LogError("Method not found: " + binding.methodName + " in Component " + type.Name);
                return;
            }
        }

        if (targetComponent != null && binding.cachedMethod != null)
        {
            binding.cachedMethod.Invoke(targetComponent, null);
            Debug.Log($"Action {binding.methodName} has been called directly on targetComponent.");
        }
        else
        {
            Debug.LogError($"Cannot call action {binding.methodName} as the targetComponent or method is not properly set.");
        }
    }

    public void BindAction(InteractableItem.InputActionBinding binding, InteractableItem item)
    {
        var targetComponent = item.targetComponent;

        if (targetComponent == null)
        {
            Debug.LogError("Target Component is not set for binding: " + binding.actionName);
            return;
        }

        if (binding.cachedMethod == null)
        {
            Type type = targetComponent.GetType();
            binding.cachedMethod = type.GetMethod(binding.methodName);
            if (binding.cachedMethod == null)
            {
                Debug.LogError("Method not found: " + binding.methodName + " in Component " + type.Name);
                return;
            }
        }

        if (binding.cachedCancelMethod == null && binding.callOtherFunctionWhenCanceled)
        {
            Type type = targetComponent.GetType();
            binding.cachedCancelMethod = type.GetMethod(binding.methodNameOnCancel);
            if (binding.cachedCancelMethod == null)
            {
                Debug.LogError("Method not found: " + binding.methodNameOnCancel + " in Component " + type.Name);
                return;
            }
        }

        InputAction inputAction = actionMap.FindAction(binding.actionName);

        if (inputAction == null)
        {
            Debug.LogError("Input action not found: " + binding.actionName);
            return;
        }

        Action<InputAction.CallbackContext> onCancel = null;
        Action<InputAction.CallbackContext> onAction = context =>
        {
            if (targetComponent != null && binding.cachedMethod != null)
            {
                binding.cachedMethod.Invoke(targetComponent, null);
                binding.ActionPerformed = true;

                if (binding.autoRemoveOnCancel || binding.callOtherFunctionWhenCanceled)
                {
                    inputAction.canceled += onCancel;
                }
            }
        };
        binding.actionDelegate = onAction;

        if (binding.callOtherFunctionWhenCanceled && !binding.autoRemoveOnCancel)
        {
            onCancel = context =>
            {
                if (targetComponent != null && binding.cachedCancelMethod != null)
                {
                    binding.cachedCancelMethod.Invoke(targetComponent, null);
                    binding.ActionPerformed = false;
                    Debug.Log("Input action binding cancel function called without unbinding");
                }
            };
            binding.cancelDelegate = onCancel;
        }
        else if (!binding.callOtherFunctionWhenCanceled && binding.autoRemoveOnCancel)
        {
            onCancel = context =>
            {
                if (binding.callOnStarted)
                {
                    inputAction.started -= onAction;
                }
                else
                {
                    inputAction.performed -= onAction;
                }
                binding.ActionPerformed = false;
                inputAction.canceled -= onCancel;
                Debug.Log("Input action binding canceled and not function called.");
            };
        }
        else if (binding.callOtherFunctionWhenCanceled && binding.autoRemoveOnCancel)
        {
            onCancel = context =>
            {
                if (targetComponent != null && binding.cachedCancelMethod != null)
                {
                    if (binding.callOnStarted)
                    {
                        inputAction.started -= onAction;
                    }
                    else
                    {
                        inputAction.performed -= onAction;
                    }
                    binding.cachedCancelMethod.Invoke(targetComponent, null);
                    binding.ActionPerformed = false;
                    Debug.Log("Input action binding cancel function called and binding removed automatically");
                    inputAction.canceled -= onCancel;
                }
            };
            binding.cancelDelegate = onCancel;
        }

        if (binding.callOnStarted)
        {
            inputAction.started += onAction;
        }
        else
        {
            inputAction.performed += onAction;
        }
    }

    public void UnbindAction(string bindingId)
    {
        var binding = FindBindingById(bindingId);
        if (binding == null)
        {
            Debug.LogError($"UnbindAction failed: {bindingId} not found.");
            return;
        }

        var inputAction = actionMap.FindAction(binding.actionName);
        if (inputAction == null)
        {
            Debug.LogError($"Input action not found: {binding.actionName}");
            return;
        }

        UnbindDelegateFromAction(binding, inputAction);
    }

    public InteractableItem.InputActionBinding FindBindingById(string bindingId)
    {
        foreach (var item in interactableItems)
        {
            foreach (var binding in item.inputActionBindings)
            {
                if (binding.bindingId == bindingId)
                {
                    return binding;
                }
            }
        }

        Debug.LogWarning($"InputActionBinding with id '{bindingId}' was not found.");
        return null;
    }

    private void UnbindDelegateFromAction(InteractableItem.InputActionBinding binding, InputAction inputAction)
    {
        if (binding.callOnStarted)
        {
            inputAction.started -= binding.actionDelegate;
        }
        else
        {
            inputAction.performed -= binding.actionDelegate;
        }

        if (!binding.callOtherFunctionWhenCanceled)
        {
            inputAction.canceled -= binding.actionDelegate;
            Debug.Log($"Binding '{binding.bindingId}' ({binding.actionName}) actionDelegate unassigned.");
        }
        else
        {
            inputAction.canceled -= binding.cancelDelegate;
            Debug.Log($"Binding '{binding.bindingId}' ({binding.actionName}) actionDelegate and cancelDelegate unassigned.");
        }
    }

    public void RegisterInteractableItem(InteractableItem item)
    {
        if (!interactableItems.Contains(item))
        {
            interactableItems.Add(item);
        }
    }

    public void UnregisterInteractableItem(InteractableItem item)
    {
        interactableItems.Remove(item);
    }
}
