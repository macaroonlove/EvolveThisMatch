using EvolveThisMatch.Core;
using FrameWork.UIPopup;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UITypeEngraveButton : UIEngraveButton
    {
        [SerializeField] private ElementalTemplate _elementalTemplate;

        private CrystalSystem _crystalSystem;
        private ElementalSystem _elemantalSystem;

        internal override void InitializeBattle(UIEngraveCanvas engraveCanvas)
        {
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
            _elemantalSystem = BattleManager.Instance.GetSubSystem<ElementalSystem>();
            _crystalSystem.onChangedCrystal += OnChangedCrystal;

            OnChangedCrystal(_crystalSystem.currentCrystal);
            Refrash();
        }

        internal override void DeinitializeBattle()
        {
            if (_crystalSystem != null)
            {
                _crystalSystem.onChangedCrystal -= OnChangedCrystal;
            }
        }

        private void OnChangedCrystal(int value)
        {
            int needCrystal = _elemantalSystem.GetNeedCrystal(_elementalTemplate);

            if (needCrystal > value)
            {
                _payText.color = Color.red;
            }
            else
            {
                _payText.color = _originTextColor;
            }
        }

        protected override void Engrave()
        {
            if (_elemantalSystem == null || _elementalTemplate == null) return;

            int result = _elemantalSystem.RequestIncreaseElemental(_elementalTemplate);

            switch (result)
            {
                case -2:
                    UIPopupManager.Instance.ShowNotificationPopup("더 이상 각인 레벨을 상승시킬 수 없습니다.");
                    return;
                case -1:
                    UIPopupManager.Instance.ShowNotificationPopup("비정상적인 각인 레벨업을 시도하고 있습니다.");
                    return;
                case 1:
                    UIPopupManager.Instance.ShowNotificationPopup("성공적으로 각인 레벨을 상승시켰습니다.");
                    break;
                default:
                    return;
            }

            int needCrystal = _elemantalSystem.GetNeedCrystal(_elementalTemplate);
            if (!_crystalSystem.PayCrystal(needCrystal))
            {
                UIPopupManager.Instance.ShowNotificationPopup("크리스탈이 부족합니다.");
                return;
            }

            Refrash();
        }

        internal override void ForceEngrave()
        {
            if (_elemantalSystem == null || _elementalTemplate == null) return;

            int result = _elemantalSystem.RequestIncreaseElemental(_elementalTemplate);

            switch (result)
            {
                case -1:
                    UIPopupManager.Instance.ShowNotificationPopup("비정상적인 각인 레벨업을 시도하고 있습니다.");
                    return;
            }

            Refrash();
        }

        private void Refrash()
        {
            var level = _elemantalSystem.GetLevel(_elementalTemplate);

            if (level == 5)
            {
                _payText.text = "Max";
            }
            else
            {
                var needCrystal = _elemantalSystem.GetNeedCrystal(_elementalTemplate);
                _payText.text = $"<sprite name=\"Crystal\"> {needCrystal}";
            }

            _levelText.text = $"LV. {level}";
        }
    }
}