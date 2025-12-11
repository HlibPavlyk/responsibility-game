using System;
using System.Collections.Generic;

namespace Systems.SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public string currentSceneName;
        public List<string> activeFlags = new List<string>();
        public List<string> firedTriggers = new List<string>();
    }
}
