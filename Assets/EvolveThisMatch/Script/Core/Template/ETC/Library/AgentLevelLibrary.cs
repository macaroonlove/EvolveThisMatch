using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/AgentLevel", fileName = "AgentLevelLibrary", order = 0)]
    public class AgentLevelLibrary : ScriptableObject
    {
        [SerializeField] private List<AgentLevelData> _levelData = new List<AgentLevelData>(15);

        public AgentLevelData GetLevelData(int level)
        {
            if (level < 0 || level >= _levelData.Count)
            {
                return null;
            }

            return _levelData[level - 1];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_levelData.Count > 15)
                _levelData.RemoveRange(15, _levelData.Count - 15);

            while (_levelData.Count < 15)
                _levelData.Add(new AgentLevelData());
        }
#endif
    }

    [System.Serializable]
    public class AgentLevelData
    {
        public int needCoin;

        public List<AlwaysEffect> levelEffects = new List<AlwaysEffect>();
    }
}