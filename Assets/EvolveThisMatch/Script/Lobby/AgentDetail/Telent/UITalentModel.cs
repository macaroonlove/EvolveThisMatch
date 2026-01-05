using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using System.Collections.Generic;
using Random = FrameWork.PlayFabExtensions.Random;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITalentModel
    {
        private AgentSaveData.Agent _owned;
        private TalentSaveData _saveData;

        private readonly float[] _rarityProbabilities;
        private readonly Dictionary<int, Random> _rngs = new();

        private int _cachedCost;

        public AgentSaveData.Agent owned => _owned;
        public TalentSaveData saveData => _saveData;

        public UITalentModel(float[] rarityProbabilities)
        {
            _rarityProbabilities = rarityProbabilities;
        }

        public void Bind(AgentSaveData.Agent owned)
        {
            _owned = owned;
            _saveData = SaveManager.Instance.agentData.GetTalentSaveData(owned.id);
        }

        #region 재능 돌리기
        public bool TryRollOnce()
        {
            if (!CanPay()) return false;

            foreach (var slot in _saveData.finalTalent)
            {
                if (slot.isLock) continue;
                RollSlot(slot);
            }

            ApplyRollCount();
            Save();
            return true;
        }

        public bool TryRollFiltered(int rarity, List<int> targets)
        {
            if (!CanPay())
            {
                Save();
                return false;
            }

            bool matched = false;
            var talentData = AgentSaveDataTemplate.talentTitleData.talentData;

            foreach (var slot in _saveData.finalTalent)
            {
                if (slot.isLock) continue;

                RollSlot(slot);

                var newRarity = GetRarity(talentData[slot.id], slot.value);
                if (targets.Contains(slot.id) && newRarity.rarity == (EAgentRarity)rarity)
                {
                    matched = true;
                }
            }

            ApplyRollCount();
            if (matched) Save();
            return matched;
        }

        private void RollSlot(TalentSaveData.Talent slot)
        {
            var rng = GetRng();
            var talentData = AgentSaveDataTemplate.talentTitleData.talentData;

            int id = rng.Next(0, talentData.Count - 1);
            var config = talentData[id];

            var rarity = GetRandomRarity(rng);
            int value = GetRandomValue(rng, rarity.rarity, config);

            slot.id = id;
            slot.value = value;
        }
        #endregion

        #region 비용 관리
        public int CalculateCost()
        {
            int cost = 5;
            foreach (var slot in _saveData.finalTalent)
            {
                if (slot.isLock) cost += 5;
            }
            _cachedCost = cost;
            return cost;
        }

        public bool CanPay()
        {
            var powder = SaveManager.Instance.profileData.GetVariable(EVariableType.Powder).Value;
            return powder >= _cachedCost;
        }

        private void Pay()
        {
            var powder = SaveManager.Instance.profileData.GetVariable(EVariableType.Powder);
            powder.AddValue(-_cachedCost);
        }
        #endregion

        #region 잠금 관리
        public void SetLock(int index, bool isLock)
        {
            _saveData.finalTalent[index].isLock = isLock;

            var lockEntry = new TalentSaveData.LockHistory
            {
                order = _saveData.rollCount,
                index = index,
                isLock = isLock
            };
            _saveData.lockHistory.Add(lockEntry);

            Save();
        }
        #endregion

        #region 저장
        private void ApplyRollCount()
        {
            Pay();

            _saveData.rollCount++;
            if (_saveData.rollCount % 50 == 0)
            {
                Save();
            }
        }

        private void Save()
        {
            SaveManager.Instance.agentData.SaveTalentLocalData();
        }
        #endregion

        #region 랜덤
        #region RNG 관리
        private Random GetRng()
        {
            if (!_rngs.TryGetValue(_owned.id, out var rng))
            {
                rng = new Random(_owned.seed);
                _rngs[_owned.id] = rng;
            }
            return rng;
        }

        public void ClearRNG() => _rngs.Clear();
        #endregion

        #region 랜덤 값 불러오기
        private AgentRarityTemplate GetRandomRarity(Random rng)
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            int rand = rng.Next(0, 100);
            float cumulative = 0;

            for (int i = 0; i < _rarityProbabilities.Length; i++)
            {
                cumulative += _rarityProbabilities[i];
                if (rand <= cumulative)
                    return raritys[i];
            }

            return raritys[^1];
        }

        private int GetRandomValue(Random rng, EAgentRarity rarity, TalentConfig data)
        {
            return rarity switch
            {
                EAgentRarity.Myth => rng.Next(data.mythLimit, data.maxValue),
                EAgentRarity.Legend => rng.Next(data.legendLimit, data.mythLimit),
                EAgentRarity.Epic => rng.Next(data.epicLimit, data.legendLimit),
                EAgentRarity.Rare => rng.Next(data.rareLimit, data.epicLimit),
                _ => rng.Next(data.minValue, data.rareLimit),
            };
        }
        #endregion
        #endregion

        #region 유틸리티
        public AgentRarityTemplate GetRarity(TalentConfig data, int value)
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            if (value > data.mythLimit) return raritys[0];
            if (value > data.legendLimit) return raritys[1];
            if (value > data.epicLimit) return raritys[2];
            if (value > data.rareLimit) return raritys[3];
            return raritys[4];
        }
        #endregion
    }
}