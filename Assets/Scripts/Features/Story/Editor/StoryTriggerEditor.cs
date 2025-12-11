using UnityEditor;
using UnityEngine;
using Features.Story.Conditions;
using Features.Story.Actions;

namespace Features.Story.Editor
{
    [CustomEditor(typeof(StoryTrigger))]
    public class StoryTriggerEditor : UnityEditor.Editor
    {
        private SerializedProperty _triggerIDProperty;
        private SerializedProperty _oneTimeOnlyProperty;
        private SerializedProperty _startEnabledProperty;
        private SerializedProperty _visualCueProperty;
        private SerializedProperty _conditionsProperty;
        private SerializedProperty _actionsProperty;

        private void OnEnable()
        {
            // Find serialized properties
            _triggerIDProperty = serializedObject.FindProperty("triggerID");
            _oneTimeOnlyProperty = serializedObject.FindProperty("oneTimeOnly");
            _startEnabledProperty = serializedObject.FindProperty("startEnabled");
            _visualCueProperty = serializedObject.FindProperty("visualCue");
            _conditionsProperty = serializedObject.FindProperty("conditions");
            _actionsProperty = serializedObject.FindProperty("actions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw standard fields
            EditorGUILayout.LabelField("Trigger Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_triggerIDProperty);
            EditorGUILayout.PropertyField(_oneTimeOnlyProperty);
            EditorGUILayout.PropertyField(_startEnabledProperty);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Visual Cue", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_visualCueProperty);

            EditorGUILayout.Space(10);

            // Custom display for Conditions
            DrawConditionsArray();

            EditorGUILayout.Space(10);

            // Custom display for Actions
            DrawActionsArray();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionsArray()
        {
            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);

            // Show all existing conditions
            for (int i = 0; i < _conditionsProperty.arraySize; i++)
            {
                SerializedProperty element = _conditionsProperty.GetArrayElementAtIndex(i);

                // Get the type name for display
                string typeName = "Unknown";
                if (element.managedReferenceValue != null)
                {
                    typeName = GetFriendlyTypeName(element.managedReferenceValue.GetType().Name);
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Condition {i}: {typeName}", EditorStyles.boldLabel);

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    _conditionsProperty.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Draw the properties of the condition
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            // Add Condition button
            if (GUILayout.Button("+ Add Condition"))
            {
                ShowConditionTypeMenu();
            }
        }

        private void DrawActionsArray()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            // Show all existing actions
            for (int i = 0; i < _actionsProperty.arraySize; i++)
            {
                SerializedProperty element = _actionsProperty.GetArrayElementAtIndex(i);

                // Get the type name for display
                string typeName = "Unknown";
                if (element.managedReferenceValue != null)
                {
                    typeName = GetFriendlyTypeName(element.managedReferenceValue.GetType().Name);
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Action {i}: {typeName}", EditorStyles.boldLabel);

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    _actionsProperty.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Draw the properties of the action
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            // Add Action button
            if (GUILayout.Button("+ Add Action"))
            {
                ShowActionTypeMenu();
            }
        }

        private void ShowConditionTypeMenu()
        {
            GenericMenu menu = new GenericMenu();

            // Add all condition types
            menu.AddItem(new GUIContent("Require Flag"), false, AddCondition<RequireFlagCondition>);
            menu.AddItem(new GUIContent("Require Not Flag"), false, AddCondition<RequireNotFlagCondition>);
            menu.AddItem(new GUIContent("Require All Flags"), false, AddCondition<RequireAllFlagsCondition>);
            menu.AddItem(new GUIContent("Require Any Flag"), false, AddCondition<RequireAnyFlagCondition>);

            menu.ShowAsContext();
        }

        private void ShowActionTypeMenu()
        {
            GenericMenu menu = new GenericMenu();

            // Add all action types
            menu.AddItem(new GUIContent("Start Dialogue"), false, AddAction<StartDialogueAction>);
            menu.AddItem(new GUIContent("Set Flag"), false, AddAction<SetFlagAction>);
            menu.AddItem(new GUIContent("Transition Scene"), false, AddAction<TransitionSceneAction>);
            menu.AddItem(new GUIContent("Enable Trigger"), false, AddAction<EnableTriggerAction>);
            menu.AddItem(new GUIContent("Disable Trigger"), false, AddAction<DisableTriggerAction>);

            menu.ShowAsContext();
        }

        private void AddCondition<T>() where T : StoryCondition, new()
        {
            int index = _conditionsProperty.arraySize;
            _conditionsProperty.InsertArrayElementAtIndex(index);

            SerializedProperty element = _conditionsProperty.GetArrayElementAtIndex(index);
            element.managedReferenceValue = new T();

            serializedObject.ApplyModifiedProperties();
        }

        private void AddAction<T>() where T : StoryAction, new()
        {
            int index = _actionsProperty.arraySize;
            _actionsProperty.InsertArrayElementAtIndex(index);

            SerializedProperty element = _actionsProperty.GetArrayElementAtIndex(index);
            element.managedReferenceValue = new T();

            serializedObject.ApplyModifiedProperties();
        }

        private string GetFriendlyTypeName(string typeName)
        {
            // Convert type names to friendly display names
            switch (typeName)
            {
                // Conditions
                case "RequireFlagCondition":
                    return "Require Flag";
                case "RequireNotFlagCondition":
                    return "Require Not Flag";
                case "RequireAllFlagsCondition":
                    return "Require All Flags";
                case "RequireAnyFlagCondition":
                    return "Require Any Flag";

                // Actions
                case "StartDialogueAction":
                    return "Start Dialogue";
                case "SetFlagAction":
                    return "Set Flag";
                case "TransitionSceneAction":
                    return "Transition Scene";
                case "EnableTriggerAction":
                    return "Enable Trigger";
                case "DisableTriggerAction":
                    return "Disable Trigger";

                default:
                    return typeName;
            }
        }
    }
}
