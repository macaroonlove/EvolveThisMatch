using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UILevelupPanel : UIBase
    {
        #region 바인딩
        enum Texts
        {
            OriginLevel,
            TargetLevel,
            ExpText,
        }
        enum Images
        {
            ExpSlider,
        }
        enum Buttons
        {
            ClearButton,
            LevelUpButton,
        }
        enum Objects
        {
            Arrow,
            EatFood,
            StockFood,
        }
        #endregion

        private TextMeshProUGUI _originLevel;
        private TextMeshProUGUI _targetLevel;
        private TextMeshProUGUI _expText;
        private Image _expSlider;
        private GameObject _arrow;
        private UILevelupAutoSelectButton _autoSelectButton;

        private UILevelUpFoodItem[] _eats;
        private UILevelUpFoodItem[] _stocks;

        private AgentSaveDataTemplate _agentData;
        private AgentSaveData.Agent _owned;
        private UnityAction _reShow;

        private int _maxLevel;
        private int[] _eatFood = new int[4];

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _originLevel = GetText((int)Texts.OriginLevel);
            _targetLevel = GetText((int)Texts.TargetLevel);
            _expText = GetText((int)Texts.ExpText);
            _expSlider = GetImage((int)Images.ExpSlider);
            _arrow = GetObject((int)Objects.Arrow);

            _eats = GetObject((int)Objects.EatFood).GetComponentsInChildren<UILevelUpFoodItem>();
            _stocks = GetObject((int)Objects.StockFood).GetComponentsInChildren<UILevelUpFoodItem>();

            _autoSelectButton = GetComponentInChildren<UILevelupAutoSelectButton>();
            _autoSelectButton.Initialize(AutoSelect);

            for (int i = 0; i < _eats.Length; i++)
            {
                int index = i;
                _eats[i].InitializeItem(() => RemoveFood(index));
                _stocks[i].InitializeItem(() => AddFood(index));
            }

            GetButton((int)Buttons.ClearButton).onClick.AddListener(Clear);
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(LevelUp);
        }

        internal void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _owned = owned;
            _reShow = reShow;

            if (_agentData == null)
                _agentData = SaveManager.Instance.agentData;

            Clear();
        }

        #region 음식 올리기·내리기
        private void AddFood(int i)
        {
            int targetLevel = _owned.level;
            int currentExp = _owned.exp + GetTotalExp();

            // 목표 레벨 계산
            GetTargetLevel(ref targetLevel, ref currentExp);

            if (targetLevel >= _maxLevel) return;

            _stocks[i].Decrement();
            _eats[i].Increment();
            _eatFood[i]++;

            UpdateUI();
        }

        private void RemoveFood(int i)
        {
            if (_eatFood[i] <= 0) return;

            _eats[i].Decrement();
            _stocks[i].Increment();
            _eatFood[i]--;

            UpdateUI();
        }
        #endregion

        #region 음식 자동 선택
        private void AutoSelect()
        {
            if (_owned == null) return;
            if (_owned.level >= _maxLevel) return;

            int targetLevel = _owned.level;
            int currentExp = _owned.exp + GetTotalExp();

            // 목표 레벨 계산
            GetTargetLevel(ref targetLevel, ref currentExp);

            if (targetLevel >= _maxLevel) return;

            int expToNextLevel = GetRequiredExpForLevel(targetLevel) - currentExp;
            bool isEat = false;

            for (int i = 3; i >= 0; i--)
            {
                while (_stocks[i].count > 0 && expToNextLevel > 0)
                {
                    expToNextLevel -= AgentSaveDataTemplate.foodExp[i];
                    _stocks[i].Decrement();
                    _eats[i].Increment();
                    _eatFood[i]++;
                    isEat = true;
                }

                if (expToNextLevel <= 0) break;
            }

            if (!isEat)
            {
                for (int i = 3; i >= 0; i--)
                {
                    while (_stocks[i].count > 0)
                    {
                        _stocks[i].Decrement();
                        _eats[i].Increment();
                        _eatFood[i]++;
                    }
                }
            }

            UpdateUI();
        }
        #endregion

        #region 비우기
        private void Clear()
        {
            if (_owned == null) return;

            _maxLevel = _agentData.GetMaxLevelByTier(_owned.tier);
            _targetLevel.gameObject.SetActive(false);
            _arrow.SetActive(false);

            int level = _owned.level;
            int maxExp = GetRequiredExpForLevel(level);
            float percent = (float)_owned.exp / maxExp;

            _originLevel.text = $"Lv. {level}";
            _expText.text = $"{_owned.exp} / {maxExp}";
            _expSlider.fillAmount = percent;

            for (int i = 0; i < _eats.Length; i++)
            {
                _eatFood[i] = 0;
                _eats[i].Hide();
                _eats[i].ResetEat();
                _stocks[i].ResetStock();
            }
        }
        #endregion

        #region UI 업데이트
        private void UpdateUI()
        {
            if (_owned == null) return;

            int level = _owned.level;
            int targetLevel = level;
            int additionalExp = GetTotalExp();
            int currentExp = _owned.exp + additionalExp;

            // 목표 레벨 계산
            GetTargetLevel(ref targetLevel, ref currentExp);

            // 다음 레벨로 가는데 필요한 총 경험치
            int nextLevelExp = GetRequiredExpForLevel(targetLevel);
            float percent;
            if (targetLevel > level + 1)
            {
                percent = 1;
            }
            else
            {
                percent = Mathf.Clamp01((float)currentExp / nextLevelExp);
            }

            // UI최신화
            if (additionalExp > 0)
            {
                _expText.text = $"{currentExp} / {nextLevelExp} (+{additionalExp})";
                _targetLevel.text = $"Lv. {targetLevel}";
                _targetLevel.gameObject.SetActive(true);
                _arrow.SetActive(true);
            }
            else
            {
                _expText.text = $"{currentExp} / {nextLevelExp}";
                _targetLevel.gameObject.SetActive(false);
                _arrow.SetActive(false);
            }

            _expSlider.fillAmount = percent;
        }
        #endregion

        #region 유틸리티
        private void GetTargetLevel(ref int targetLevel, ref int currentExp)
        {
            while (targetLevel < _maxLevel)
            {
                int requiredExp = GetRequiredExpForLevel(targetLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                targetLevel++;
            }
        }

        /// <summary>
        /// 현재 레벨 기준 필요한 경험치 반환
        /// </summary>
        public int GetRequiredExpForLevel(int level)
        {
            if (level <= 0) return 0;

            const double baseExp = 450.0;
            const double growthRate = 1.09;

            return (int)Math.Round(baseExp * Math.Pow(growthRate, level - 1));
        }

        private int GetTotalExp()
        {
            int total = 0;

            for (int i = 0; i < _eatFood.Length; i++)
            {
                total += _eatFood[i] * AgentSaveDataTemplate.foodExp[i];
            }

            return total;
        }
        #endregion

        private void LevelUp()
        {
            if (_owned == null) return;

            int additionalExp = GetTotalExp();
            if (additionalExp <= 0) return;

            _agentData.LevelUpAgent(_owned.id, _eatFood, () =>
            {
                // 레벨업에 성공했다면
                foreach (var eat in _eats)
                {
                    eat.PayFood();
                }

                _reShow?.Invoke();
            });
        }
    }
}