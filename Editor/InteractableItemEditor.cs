using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableItem))]
public class InteractableItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InteractableItem script = (InteractableItem)target;

        // Draw the default inspector, which includes the targetComponent field
        DrawDefaultInspector();

        // Header for Input Action Bindings
        GUILayout.Label("Input Action Bindings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        if (script.inputActionBindings == null)
        {
            script.inputActionBindings = new System.Collections.Generic.List<InteractableItem.InputActionBinding>();
        }
        for (int i = 0; i < script.inputActionBindings.Count; i++)
        {
            var binding = script.inputActionBindings[i];
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                // Remove this element
                script.inputActionBindings.RemoveAt(i);
                break; // Exit the loop as the list has been modified
            }
            EditorGUILayout.EndHorizontal();

            // Toggle for calling function immediately
            binding.callFunctionImmediately = EditorGUILayout.Toggle("Call Function Immediately", binding.callFunctionImmediately);

            // Only show bindingId and actionName if "call function immediately" is not checked
            if (!binding.callFunctionImmediately)
            {
                binding.bindingId = EditorGUILayout.TextField("Unique Binding ID", binding.bindingId);
                binding.actionName = EditorGUILayout.TextField("Action Name", binding.actionName);
            }

            binding.methodName = EditorGUILayout.TextField("Method Name", binding.methodName);

            // Conditional fields based on the "call function immediately" setting
            if (!binding.callFunctionImmediately)
            {
                binding.callOnStarted = EditorGUILayout.Toggle("Call On Started?", binding.callOnStarted);
                binding.callOtherFunctionWhenCanceled = EditorGUILayout.Toggle("Call Other Function When Canceled?", binding.callOtherFunctionWhenCanceled);
                if (binding.callOtherFunctionWhenCanceled)
                {
                    binding.methodNameOnCancel = EditorGUILayout.TextField("Method Name On Cancel", binding.methodNameOnCancel);
                }
                binding.autoRemoveOnCancel = EditorGUILayout.Toggle("Auto Remove On Cancel?", binding.autoRemoveOnCancel);
            }

            EditorGUILayout.EndVertical();
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties(); // Apply changes to serialized properties
        }

        // Button to add a new binding
        if (GUILayout.Button("Add New Binding"))
        {
            script.inputActionBindings.Add(new InteractableItem.InputActionBinding());
        }

        if (GUILayout.Button("Export to JSON"))
        {
            string defaultName = "ActionBindings.json";
            string path = EditorUtility.SaveFilePanel("Save Action Bindings", "", defaultName, "json");
            if (!string.IsNullOrEmpty(path))
            {
                script.ExportBindingsToJson(path);
            }
        }
        if (GUILayout.Button("Import from JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Load Action Bindings", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                script.ImportBindingsFromJson(path);
                EditorUtility.SetDirty(target);
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target); // Mark the target as dirty if changes have been made
        }
    }
}
