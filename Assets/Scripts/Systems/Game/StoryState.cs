using System.Collections.Generic;
using UnityEngine;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "StoryState", menuName = "ScriptableObjects/StoryState", order = 2)]
    public class StoryState : ScriptableObject
    {
        [SerializeField] private List<string> _activeFlagsList = new List<string>();
        [SerializeField] private List<string> _firedTriggersList = new List<string>();

        private HashSet<string> _activeFlags;
        private HashSet<string> _firedTriggers;

        private void OnEnable()
        {
            InitializeHashSets();
        }

        private void InitializeHashSets()
        {
            _activeFlags = new HashSet<string>(_activeFlagsList);
            _firedTriggers = new HashSet<string>(_firedTriggersList);
        }

        // Flag management
        public bool HasFlag(string flagName)
        {
            if (_activeFlags == null)
                InitializeHashSets();

            return _activeFlags.Contains(flagName);
        }

        public void SetFlag(string flagName)
        {
            if (_activeFlags == null)
                InitializeHashSets();

            if (_activeFlags.Add(flagName))
            {
                _activeFlagsList.Add(flagName);
            }
        }

        public void ClearFlag(string flagName)
        {
            if (_activeFlags == null)
                InitializeHashSets();

            if (_activeFlags.Remove(flagName))
            {
                _activeFlagsList.Remove(flagName);
            }
        }

        public bool HasAllFlags(params string[] flagNames)
        {
            if (_activeFlags == null)
                InitializeHashSets();

            foreach (var flag in flagNames)
            {
                if (!_activeFlags.Contains(flag))
                    return false;
            }
            return true;
        }

        public bool HasAnyFlag(params string[] flagNames)
        {
            if (_activeFlags == null)
                InitializeHashSets();

            foreach (var flag in flagNames)
            {
                if (_activeFlags.Contains(flag))
                    return true;
            }
            return false;
        }

        // Trigger management
        public bool HasTriggered(string triggerID)
        {
            if (_firedTriggers == null)
                InitializeHashSets();

            return _firedTriggers.Contains(triggerID);
        }

        public void MarkTriggered(string triggerID)
        {
            if (_firedTriggers == null)
                InitializeHashSets();

            if (_firedTriggers.Add(triggerID))
            {
                _firedTriggersList.Add(triggerID);
            }
        }

        public void ResetTrigger(string triggerID)
        {
            if (_firedTriggers == null)
                InitializeHashSets();

            if (_firedTriggers.Remove(triggerID))
            {
                _firedTriggersList.Remove(triggerID);
            }
        }

        // Serialization support for SaveLoadManager
        public List<string> GetAllFlags()
        {
            if (_activeFlags == null)
                InitializeHashSets();

            return new List<string>(_activeFlags);
        }

        public List<string> GetAllTriggeredIDs()
        {
            if (_firedTriggers == null)
                InitializeHashSets();

            return new List<string>(_firedTriggers);
        }

        public void LoadFlags(List<string> flags)
        {
            _activeFlagsList = new List<string>(flags);
            _activeFlags = new HashSet<string>(flags);
        }

        public void LoadTriggers(List<string> triggers)
        {
            _firedTriggersList = new List<string>(triggers);
            _firedTriggers = new HashSet<string>(triggers);
        }

        // Debug utility
        public void ResetAll()
        {
            _activeFlagsList.Clear();
            _firedTriggersList.Clear();
            _activeFlags = new HashSet<string>();
            _firedTriggers = new HashSet<string>();

            Debug.Log("StoryState reset: All flags and triggers cleared");
        }
    }
}
