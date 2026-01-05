using EvolveThisMatch.Save;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public sealed class UILevelUpModel
    {
        private readonly AgentSaveDataTemplate _agentData;
        private AgentSaveData.Agent _owned;

        private readonly int[] _eatFood = new int[4];
        private int _maxLevel;

        public bool IsEmpty => _owned == null;

        public UILevelUpModel()
        {
            _agentData = SaveManager.Instance.agentData;
        }

        public void Bind(AgentSaveData.Agent owned)
        {
            _owned = owned;
            _maxLevel = _agentData.GetMaxLevelByTier(_owned.tier);

            if (_owned == null)
            {
                _maxLevel = 0;
                return;
            }

            Clear();
        }

        public void Clear()
        {
            Array.Clear(_eatFood, 0, _eatFood.Length);
        }

        #region 음식 올리기·내리기
        public bool AddFood(int index)
        {
            if (!CanLevelUp()) return false;

            _eatFood[index]++;
            return true;
        }

        public bool RemoveFood(int index)
        {
            if (_eatFood[index] <= 0) return false;

            _eatFood[index]--;
            return true;
        }
        #endregion

        #region 음식 자동 선택
        public int[] AutoSelect(int[] stockCounts)
        {
            int[] selectedFood = new int[_eatFood.Length];

            if (!CanLevelUp()) return selectedFood;

            int currentLevel = _owned.level;
            int currentExp = _owned.exp + GetTotalExp();

            // 시작 레벨에서 현재까지 추가된 경험치를 적용하기
            while (currentLevel < _maxLevel)
            {
                int requiredExp = GetRequiredExpForLevel(currentLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                currentLevel++;
            }

            if (currentLevel >= _maxLevel) return selectedFood;

            // 다음 레벨까지 필요한 경험치 반환하기
            int nextNeedExp = GetRequiredExpForLevel(currentLevel);
            nextNeedExp -= currentExp;

            for (int i = stockCounts.Length - 1; i >= 0; i--)
            {
                if (nextNeedExp <= 0) break;

                int foodExp = AgentSaveDataTemplate.foodExp[i];
                int foodCount = stockCounts[i];

                if (foodCount <= 0) continue;

                // 필요한 음식의 개수
                int needCount = Mathf.CeilToInt((float)nextNeedExp / foodExp);
                // 실제로 사용할 음식의 개수
                int useCount = Mathf.Min(needCount, foodCount);

                if (useCount <= 0) continue;

                _eatFood[i] += useCount;
                selectedFood[i] = useCount;
                nextNeedExp -= useCount * foodExp;
            }

            return selectedFood;
        }
        #endregion

        #region ViewState 생성
        public LevelupViewState BuildViewState()
        {
            if (_owned == null) return default;

            int originLevel = _owned.level;
            int additionalExp = GetTotalExp();
            int currentExp = _owned.exp + additionalExp;

            int currentLevel = originLevel;

            // 시작 레벨에서 현재까지 추가된 경험치를 적용하기
            while (currentLevel < _maxLevel)
            {
                int requiredExp = GetRequiredExpForLevel(currentLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                currentLevel++;
            }

            // 다음 레벨까지 필요한 총 경험치
            int nextNeedExp = GetRequiredExpForLevel(currentLevel);
            float percent;
            string expText;

            // 최대 레벨이라면
            if (currentLevel >= _maxLevel)
            {
                percent = 1;
                if (additionalExp <= 0) expText = "Max";
                else expText = $"Max (+{additionalExp})";
            }
            else
            {
                percent = Mathf.Clamp01((float)currentExp / nextNeedExp);
                if (additionalExp <= 0) expText = $"{currentExp} / {nextNeedExp}";
                else expText = $"{currentExp} / {nextNeedExp} (+{additionalExp})";
            }

            return new LevelupViewState
            {
                originLevel = originLevel,
                targetLevel = currentLevel,
                expText = expText,
                expPercent = percent,
                showTarget = currentLevel > originLevel
            };
        }
        #endregion

        #region 레벨업
        public void LevelUp(UnityAction onComplete)
        {
            int totalExp = GetTotalExp();
            if (totalExp <= 0) return;

            _agentData.LevelUpAgent(_owned.id, _eatFood, onComplete);
        }

        private bool CanLevelUp()
        {
            if (_owned == null) return false;

            int currentLevel = _owned.level;
            int currentExp = _owned.exp + GetTotalExp();

            // 시작 레벨에서 현재까지 추가된 경험치를 적용하기
            while (currentLevel < _maxLevel)
            {
                int requiredExp = GetRequiredExpForLevel(currentLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                currentLevel++;
            }

            return currentLevel < _maxLevel;
        }
        #endregion

        #region 계산
        /// <summary>
        /// 제단 위에 올라가있는 총 경험치 반환
        /// </summary>
        private int GetTotalExp()
        {
            int total = 0;
            for (int i = 0; i < _eatFood.Length; i++)
                total += _eatFood[i] * AgentSaveDataTemplate.foodExp[i];
            return total;
        }

        /// <summary> 
        /// 현재 레벨 기준 필요한 경험치 반환
        /// </summary>
        private int GetRequiredExpForLevel(int level)
        {
            const double baseExp = 450.0;
            const double growthRate = 1.09;
            return (int)Math.Round(baseExp * Math.Pow(growthRate, level - 1));
        }
        #endregion
    }
}