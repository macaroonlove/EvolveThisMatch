using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class ProfileSaveData
    {
        public string displayName;

        [Tooltip("튜토리얼 클리어 여부")]
        public bool isClearTutorial;

        [Tooltip("골드")]
        public ObscuredInt gold;

        [Tooltip("보유하고 있는 아군 유닛들")]
        public List<Agent> ownedAgents = new List<Agent>();

        [Tooltip("보유하고 있는 패시브 아이템들의 아이디")]
        public List<int> ownedPassiveItemIds = new List<int>();

        [Tooltip("보유하고 있는 액티브 아이템들의 아이디")]
        public List<int> ownedActiveItemIds = new List<int>();

        #region 데이터 모델
        [Serializable]
        public class Agent
        {
            public int id;
            public int unitCount;
            public int tier;
            public int exp;
            public int level;

            public int selectedSkinId;
            public List<int> ownedSkinIds = new List<int>() { 0 };

            public Agent(int id)
            {
                this.id = id;
                this.unitCount = 0;
                this.tier = 0;
                this.exp = 0;
                this.level = 0;
                this.selectedSkinId = 0;
            }
        }
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ProfileSaveData", fileName = "ProfileSaveData", order = 0)]
    public class ProfileSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ProfileSaveData _data;

        [Header("Variable 연동")]
        [SerializeField] private ObscuredIntVariable _goldVariable;

        public bool isLoaded { get; private set; }

        public string displayName { get => _data.displayName; set => _data.displayName = value; }
        public bool isClearTutorial { get => _data.isClearTutorial; set => _data.isClearTutorial = value; }

        public List<ProfileSaveData.Agent> ownedAgents => _data.ownedAgents;
        public List<int> ownedPassiveItemIds => _data.ownedPassiveItemIds;
        public List<int> ownedActiveItemIds => _data.ownedActiveItemIds;

        public override void SetDefaultValues()
        {
            _data = new ProfileSaveData();

            // 초기 캐릭터 추가
            AddAgent(0);
            AddAgent(1);
            AddAgent(2);
            AddAgent(3);
            AddAgent(4);
            AddAgent(5);

            // 초기 골드 추가
            _data.gold = 100;

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ProfileSaveData>(json);

            if (_data != null)
            {
                isLoaded = _data.ownedAgents.Count > 0;

                _goldVariable.Value = _data.gold;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            _data.gold = _goldVariable.Value;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }

        #region 유닛
        /// <summary>
        /// 유닛 추가
        /// </summary>
        public void AddAgent(int id)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, id);

            // 유닛이 없었다면 유닛 추가
            if (modifyUnit == null)
            {
                _data.ownedAgents.Add(new ProfileSaveData.Agent(id));
            }
            // 유닛이 있었다면 유닛의 개수 추가
            else
            {
                modifyUnit.unitCount++;
            }
        }

        #region 스킨
        /// <summary>
        /// 스킨 추가 (보유 중인 유닛만 스킨 획득 가능)
        /// </summary>
        public void AddAgentSkin(int agentId, int skinId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            // 유닛이 있다면 스킨 추가
            if (modifyUnit != null)
            {
                if (modifyUnit.ownedSkinIds.Contains(skinId))
                {
                    modifyUnit.ownedSkinIds.Add(skinId);
                }
            }
        }

        /// <summary>
        /// 스킨을 보유하고 있는지
        /// </summary>
        public bool IsOwnedAgentSkin(int agentId, int skinId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                if (skinId == 0) return true;

                return modifyUnit.ownedSkinIds.Contains(skinId);
            }

            return false;
        }

        /// <summary>
        /// 현재 유닛의 모든 스킨ID 받아오기
        /// </summary>
        public List<int> GetAllAgentSkinId(int agentId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                return modifyUnit.ownedSkinIds;
            }

            return null;
        }

        /// <summary>
        /// 현재 유닛의 선택된 스킨ID 받아오기
        /// </summary>
        public int GetSelectedAgentSkinId(int agentId)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, agentId);

            if (modifyUnit != null)
            {
                return modifyUnit.selectedSkinId;
            }

            return -1;
        }
        #endregion

        #region 유닛 티어 상승
        /// <summary>
        /// 유닛의 티어를 올리는데 요구하는 개수
        /// </summary>
        private static readonly ObscuredInt[] _agentTierUpRequirements = { 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };

        /// <summary>
        /// 유닛 티어 상승
        /// </summary>
        public bool TierUpAgent(int id)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, id);

            // 유닛이 있다면 && 최대 티어가 아니라면
            if (modifyUnit != null && modifyUnit.tier < _agentTierUpRequirements.Length - 1)
            {
                int requiredCount = _agentTierUpRequirements[modifyUnit.tier];

                if (modifyUnit.unitCount >= requiredCount)
                {
                    modifyUnit.unitCount -= requiredCount;
                    modifyUnit.tier++;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 티어 상승 가능한 유닛인지 판별
        /// </summary>
        public bool GetTierUpAbleUnit(int id)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, id);

            return modifyUnit != null
                && modifyUnit.tier < _agentTierUpRequirements.Length - 1
                && modifyUnit.unitCount >= _agentTierUpRequirements[modifyUnit.tier];
        }
        
        /// <summary>
        /// 유닛의 티어 별 최대 유닛 개수 반환
        /// </summary>
        public int GetMaxUnitCountByTier(int tier)
        {
            return _agentTierUpRequirements[tier];
        }
        #endregion

        #region 유닛 레벨업
        /// <summary>
        /// 티어에 따라 제한되는 최대 레벨
        /// </summary>
        private static readonly int[] _agentMaxLevelPerTier = { 50, 70, 100, 120, 150, 180, 210, 240, 270, 300, 330 };

        /// <summary>
        /// 유닛 레벨업
        /// </summary>
        public bool LevelUpAgent(int id, int gainedExp)
        {
            var unit = FindAgent(_data.ownedAgents, id);
            if (unit == null)
                return false;

            int maxLevel = _agentMaxLevelPerTier[unit.tier];
            bool leveledUp = false;

            unit.exp += gainedExp;

            while (unit.level < maxLevel)
            {
                int requiredExp = GetRequiredExpForLevel(unit.level);
                if (unit.exp >= requiredExp)
                {
                    unit.exp -= requiredExp;
                    unit.level++;
                    leveledUp = true;
                }
                else
                {
                    break;
                }
            }

            return leveledUp;
        }

        /// <summary>
        /// 현재 레벨 기준 필요한 경험치 반환
        /// </summary>
        private int GetRequiredExpForLevel(int level)
        {
            if (level <= 0) return 0;

            const double baseExp = 450.0;
            const double growthRate = 1.09;

            return (int)Math.Round(baseExp * Math.Pow(growthRate, level - 1));
        }

        /// <summary>
        /// 레벨업이 가능한지 여부
        /// </summary>
        public bool CanLevelUp(int id)
        {
            var modifyUnit = FindAgent(_data.ownedAgents, id);

            if (modifyUnit == null)
                return false;

            int maxLevel = GetMaxLevelByTier(modifyUnit.tier);

            if (modifyUnit.level >= maxLevel)
                return false;

            int requiredExp = GetRequiredExpForLevel(modifyUnit.level);

            return modifyUnit.exp >= requiredExp;
        }

        /// <summary>
        /// 해당 티어에서의 최대 레벨 반환
        /// </summary>
        public int GetMaxLevelByTier(int tier)
        {
            if (tier < 0 || tier >= _agentMaxLevelPerTier.Length)
                return _agentMaxLevelPerTier[_agentMaxLevelPerTier.Length - 1];

            return _agentMaxLevelPerTier[tier];
        }
        #endregion

        #region 유틸리티
        private ProfileSaveData.Agent FindAgent(List<ProfileSaveData.Agent> agents, int agentId)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].id == agentId)
                {
                    return agents[i];
                }
            }
            return null;
        }
        #endregion
        #endregion

        #region 아이템
        /// <summary>
        /// 패시브 아이템 추가
        /// </summary>
        public void AddPassiveItem(int id)
        {
            if (_data.ownedPassiveItemIds.Contains(id) == false)
            {
                _data.ownedPassiveItemIds.Add(id);
            }
        }

        /// <summary>
        /// 액티브 아이템 추가
        /// </summary>
        public void AddActiveItem(int id)
        {
            if (_data.ownedActiveItemIds.Contains(id) == false)
            {
                _data.ownedActiveItemIds.Add(id);
            }
        }
        #endregion
    }
}