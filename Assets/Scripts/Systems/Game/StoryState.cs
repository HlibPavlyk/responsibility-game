using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "StoryState", menuName = "ScriptableObjects/StoryState", order = 2)]
    public class StoryState : ScriptableObject
    {
        [SerializeField] private List<string> activeFlagsList = new();
        [SerializeField] private List<string> firedTriggersList = new();

        private void OnValidate()
        {
            // Remove duplicates and null/empty entries when edited in Inspector
            RemoveDuplicatesAndEmpty(activeFlagsList);
            RemoveDuplicatesAndEmpty(firedTriggersList);
        }

        private void RemoveDuplicatesAndEmpty(List<string> list)
        {
            if (list == null) return;

            // Remove null or empty strings
            list.RemoveAll(string.IsNullOrEmpty);

            // Remove duplicates by converting to HashSet and back
            var uniqueItems = new HashSet<string>(list);
            list.Clear();
            list.AddRange(uniqueItems);
        }

        // Flag management
        public bool HasFlag(string flagName)
        {
            return activeFlagsList.Contains(flagName);
        }

        public void SetFlag(string flagName)
        {
            if (!activeFlagsList.Contains(flagName))
            {
                activeFlagsList.Add(flagName);
            }
        }

        public void ClearFlag(string flagName)
        {
            activeFlagsList.Remove(flagName);
        }

        public bool HasAllFlags(params string[] flagNames)
        {
            foreach (var flag in flagNames)
            {
                if (!activeFlagsList.Contains(flag))
                    return false;
            }
            return true;
        }

        public bool HasAnyFlag(params string[] flagNames)
        {
            foreach (var flag in flagNames)
            {
                if (activeFlagsList.Contains(flag))
                    return true;
            }
            return false;
        }

        // Trigger management
        public bool HasTriggered(string triggerID)
        {
            return firedTriggersList.Contains(triggerID);
        }

        public void MarkTriggered(string triggerID)
        {
            if (!firedTriggersList.Contains(triggerID))
            {
                firedTriggersList.Add(triggerID);
            }
        }

        public void ResetTrigger(string triggerID)
        {
            firedTriggersList.Remove(triggerID);
        }

        // Serialization support for SaveLoadManager
        public List<string> GetAllFlags()
        {
            return new List<string>(activeFlagsList);
        }

        public List<string> GetAllTriggeredIDs()
        {
            return new List<string>(firedTriggersList);
        }

        public void LoadFlags(List<string> flags)
        {
            activeFlagsList = new List<string>(flags);
            RemoveDuplicatesAndEmpty(activeFlagsList);
        }

        public void LoadTriggers(List<string> triggers)
        {
            firedTriggersList = new List<string>(triggers);
            RemoveDuplicatesAndEmpty(firedTriggersList);
        }

        // Debug utility
        public void ResetAll()
        {
            activeFlagsList.Clear();
            firedTriggersList.Clear();

            Debug.Log("StoryState reset: All flags and triggers cleared");
        }

        // Inspector utilities for manual management
#if UNITY_EDITOR
        [ContextMenu("Remove Duplicates")]
        private void RemoveDuplicatesManual()
        {
            RemoveDuplicatesAndEmpty(activeFlagsList);
            RemoveDuplicatesAndEmpty(firedTriggersList);
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("StoryState: Removed duplicates and empty entries");
        }

        [ContextMenu("Clear All Flags")]
        private void ClearAllFlags()
        {
            activeFlagsList.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("StoryState: All flags cleared");
        }

        [ContextMenu("Clear All Triggers")]
        private void ClearAllTriggers()
        {
            firedTriggersList.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("StoryState: All fired triggers cleared");
        }
#endif
    }
}
