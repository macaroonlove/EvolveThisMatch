using UnityEngine;

namespace EvolveThisMatch.Save
{
    public abstract class SaveDataTemplate : ScriptableObject
    {
        public bool isLoaded { get; protected set; }

        public abstract void SetDefaultValues();

        public abstract bool Load(string json);

        public abstract string ToJson();

        public abstract void Clear();
    }
}