using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InteractableItem : MonoBehaviour
{
    public GameObject activatable;
    public bool deactivateOnInteract = false;
    public bool destroyOnInteract = false;
    public Component targetComponent;

    [Serializable]
    public class InputActionBinding
    {
        public string bindingId;
        public string actionName;
        public string methodName;
        public bool callOnStarted = true;
        public bool callOtherFunctionWhenCanceled = false;
        public string methodNameOnCancel;
        public bool autoRemoveOnCancel = true;
        public bool callFunctionImmediately;

        [HideInInspector]
        [NonSerialized]
        public System.Reflection.MethodInfo cachedMethod;

        [HideInInspector]
        [NonSerialized]
        public System.Reflection.MethodInfo cachedCancelMethod;

        [HideInInspector]
        [NonSerialized]
        public Action<InputAction.CallbackContext> actionDelegate;

        [HideInInspector]
        [NonSerialized]
        public Action<InputAction.CallbackContext> cancelDelegate;

        [HideInInspector]
        public bool ActionPerformed { get; set; }
    }

    [HideInInspector]
    public List<InputActionBinding> inputActionBindings = new List<InputActionBinding>();

    private class Wrapper
    {
        public List<InputActionBinding> Bindings;
    }

    public void ExportBindingsToJson(string filePath)
    {
        try
        {
            string json = JsonUtility.ToJson(new Wrapper { Bindings = this.inputActionBindings }, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Export successful: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to export to JSON: " + ex.Message);
        }
    }

    public void ImportBindingsFromJson(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            Wrapper wrapped = JsonUtility.FromJson<Wrapper>(json);
            this.inputActionBindings = wrapped.Bindings;
            Debug.Log("Import successful: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to import from JSON: " + ex.Message);
        }
    }

    private void OnEnable()
    {
        if (InputActionManager.Instance != null)
        {
            InputActionManager.Instance.RegisterInteractableItem(this);
        }
    }

    private void OnDestroy()
    {
        if (InputActionManager.Instance != null)
        {
            InputActionManager.Instance.UnregisterInteractableItem(this);
        }
    }

    public void Interacted()
    {
        foreach (var binding in inputActionBindings)
        {
            if (!binding.callFunctionImmediately)
            {
                InputActionManager.Instance.BindAction(binding, this);
            }
            else
            {
                InputActionManager.Instance.CallAction(binding, this);
            }
        }

        if (activatable != null)
        {
            activatable.SetActive(true);
        }

        if (deactivateOnInteract)
        {
            gameObject.SetActive(false);
        }

        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }
}
