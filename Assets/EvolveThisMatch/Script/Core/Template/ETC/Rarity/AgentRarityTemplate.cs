using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Etc/Rarity/AgentRarity", fileName = "AgentRarity", order = 0)]
    public class AgentRarityTemplate : RarityTemplate
    {
        [SerializeField] private EAgentRarity _rarity;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;
        [SerializeField] private Sprite _agentInfoSprite;

        [Header("동기화")]
        [SerializeField] private AgentSyncData _agentSyncData;

        [Header("격 임시 돌파")]
        [SerializeField] private AgentLimitData _agentLimitData;

        #region 프로퍼티
        public EAgentRarity rarity => _rarity;
        public Color backgroundColor => _backgroundColor;
        public Color textColor => _textColor;
        public Sprite agentInfoSprite => _agentInfoSprite;

        public AgentSyncData agentSyncData => _agentSyncData;
        public AgentLimitData agentLimitData => _agentLimitData;
        #endregion
    }

    [Serializable]
    public class AgentSyncData
    {
        [SerializeField] private ObscuredInt _baseCoin = 0;
        [SerializeField] private ObscuredFloat _growthFactor = 1.1f;

        public int GetNeedCoin(int syncLevel)
        {
            if (syncLevel <= 0) return -1;

            var coin = _baseCoin * Mathf.Pow(_growthFactor, syncLevel - 1);
            return Mathf.RoundToInt(coin);
        }

        public int GetUnlockedSkillCount(int syncLevel)
        {
            return syncLevel / 5;
        }
    }

    [Serializable]
    public class AgentLimitData
    {
        [Serializable]
        private struct UpgradeEntry
        {
            public ObscuredInt probability;
            public AgentRarityTemplate resultRarity;
        }

        [SerializeField] private List<UpgradeEntry> _upgradeTable;

        public string GetUpgradeLimitProbability()
        {
            string result = "";

            foreach (var entry in _upgradeTable)
            {
                result += $"{entry.resultRarity.displayName}: {entry.probability}%\n";
            }

            return result;
        }

        public AgentRarityTemplate GetUpgradeLimitResult()
        {
            if (_upgradeTable.Count == 0) return null;

            int rand = Random.Range(0, 100);
            int cumulative = 0;

            foreach (var entry in _upgradeTable)
            {
                cumulative += entry.probability;
                if (rand < cumulative)
                {
                    return entry.resultRarity;
                }
            }

            return null;
        }
    }
}