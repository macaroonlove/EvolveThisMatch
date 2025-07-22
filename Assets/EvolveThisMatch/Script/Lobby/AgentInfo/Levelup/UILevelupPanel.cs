using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
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

        private UIFoodItem[] _eats;
        private UIFoodItem[] _stocks;

        private ProfileSaveDataTemplate _profileData;
        private ProfileSaveData.Agent _owned;
        private UnityAction _reShow;

        private int _maxLevel;
        private int _expValue;
        private static readonly int[] _foodExp = { 30, 500, 2000, 15000 };

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

            _eats = GetObject((int)Objects.EatFood).GetComponentsInChildren<UIFoodItem>();
            _stocks = GetObject((int)Objects.StockFood).GetComponentsInChildren<UIFoodItem>();

            _autoSelectButton = GetComponentInChildren<UILevelupAutoSelectButton>();
            _autoSelectButton.Initialize(AutoSelect);

            for (int i = 0; i < _eats.Length; i++)
            {
                int index = i;
                _eats[i].InitializeItem(() => TransferEatToShock(index));
                _stocks[i].InitializeItem(() => TransferShockToEat(index));
            }

            GetButton((int)Buttons.ClearButton).onClick.AddListener(Clear);
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(LevelUp);
        }

        internal void Show(ProfileSaveData.Agent owned, UnityAction reShow)
        {
            _owned = owned;
            _reShow = reShow;
            if (_profileData == null) _profileData = GameDataManager.Instance.profileSaveData;

            Clear();
        }

        private void TransferShockToEat(int i)
        {
            int level = _owned.level;
            int targetLevel = level;
            int currentExp = _owned.exp + _expValue;

            // 목표 레벨 계산
            while (targetLevel < _maxLevel)
            {
                int requiredExp = _profileData.GetRequiredExpForLevel(targetLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                targetLevel++;
            }

            if (targetLevel >= _maxLevel) return;

            _stocks[i].Decrement();
            _eats[i].Increment();
            _expValue += _foodExp[i];

            RefreshSlider();
        }

        private void TransferEatToShock(int i)
        {
            _eats[i].Decrement();
            _stocks[i].Increment();
            _expValue -= _foodExp[i];

            RefreshSlider();
        }

        private void RefreshSlider()
        {
            if (_owned == null) return;

            int level = _owned.level;
            int currentExp = _owned.exp + _expValue;
            int targetLevel = level;

            // 목표 레벨 계산
            while (targetLevel < _maxLevel)
            {
                int requiredExp = _profileData.GetRequiredExpForLevel(targetLevel);

                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                targetLevel++;
            }

            // 현재 레벨 기준의 필요 경험치
            int nextLevelExp = _profileData.GetRequiredExpForLevel(targetLevel);
            float percent;
            if (targetLevel > level + 1)
            {
                percent = 1;
            }
            else
            {
                percent = Mathf.Clamp01((float)currentExp / nextLevelExp);
            }

            _expSlider.fillAmount = percent;

            // 텍스트
            if (_expValue > 0)
            {
                _expText.text = $"{currentExp} / {nextLevelExp} (+{_expValue})";
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
        }

        private void AutoSelect()
        {
            if (_owned == null) return;
            if (_owned.level >= _maxLevel) return;

            int level = _owned.level;
            int currentExp = _owned.exp;
            int targetLevel = level;

            for (int i = 0; i < 4; i++)
                currentExp += _eats[i].count * _foodExp[i];

            while (targetLevel < _maxLevel)
            {
                int requiredExp = _profileData.GetRequiredExpForLevel(targetLevel);
                if (currentExp < requiredExp) break;

                currentExp -= requiredExp;
                targetLevel++;
            }

            if (targetLevel >= _maxLevel) return;

            int expToNextLevel = _profileData.GetRequiredExpForLevel(targetLevel) - currentExp;
            int[] useCount = new int[4];

            bool isEat = false;

            for (int i = 3; i >= 0; i--)
            {
                while (_stocks[i].count > 0 && expToNextLevel > 0)
                {
                    expToNextLevel -= _foodExp[i];
                    _stocks[i].Decrement();
                    _eats[i].Increment();
                    _expValue += _foodExp[i];
                    useCount[i]++;
                    isEat = true;
                }

                if (expToNextLevel <= 0)
                    break;
            }

            if (!isEat)
            {
                for (int i = 3; i >= 0; i--)
                {
                    while (_stocks[i].count > 0)
                    {
                        _stocks[i].Decrement();
                        _eats[i].Increment();
                        _expValue += _foodExp[i];
                    }
                }
            }

            RefreshSlider();
        }

        private void LevelUp()
        {
            if (_owned == null) return;
            if (_expValue <= 0) return;

            _profileData.LevelUpAgent(_owned.id, _expValue);

            foreach (var eat in _eats)
            {
                eat.PayFood();
            }

            _reShow?.Invoke();
        }

        private void Clear()
        {
            if (_owned == null) return;

            _maxLevel = _profileData.GetMaxLevelByTier(_owned.tier);
            _targetLevel.gameObject.SetActive(false);
            _arrow.SetActive(false);

            int level = _owned.level;
            int maxExp = _profileData.GetRequiredExpForLevel(level);
            float percent = (float)_owned.exp / maxExp;

            _originLevel.text = $"Lv. {level}";
            _expText.text = $"{_owned.exp} / {maxExp}";
            _expSlider.fillAmount = percent;

            for (int i = 0; i < _eats.Length; i++)
            {
                _eats[i].Hide();
                _eats[i].ResetEat();
                _stocks[i].ResetStock();
            }
        }
    }
}