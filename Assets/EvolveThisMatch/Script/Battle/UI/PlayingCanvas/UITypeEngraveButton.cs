using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UITypeEngraveButton : UIEngraveButton
    {
        [SerializeField] private SkillTypeTemplate _skillTypeTemplate;

        private CrystalSystem _crystalSystem;

        internal override void InitializeBattle(UIEngraveCanvas engraveCanvas)
        {
            _skillTypeTemplate.Initialize();

            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
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
            var needCrystal = _skillTypeTemplate.GetEngraveData().needCrystal;

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
            var needCrystal = _skillTypeTemplate.GetEngraveData().needCrystal;

            if (!_crystalSystem.CheckCrystal(needCrystal)) return;

            if (_skillTypeTemplate.UpgradeEngraveLevel())
            {
                _crystalSystem.PayCrystal(needCrystal);

                Refrash();
            }
        }

        private void Refrash()
        {
            var data = _skillTypeTemplate.GetEngraveData();

            if (data.needCrystal == -1)
            {
                _payText.text = "Max";
            }
            else
            {
                _payText.text = $"<sprite name=\"Crystal\"> {data.needCrystal}";
            }

            _levelText.text = $"LV. {_skillTypeTemplate.engraveLevel + 1}";
        }
    }
}