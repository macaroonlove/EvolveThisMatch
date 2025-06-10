using FrameWork.UIBinding;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class ManaBar : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Mana_Fill,
        }
        #endregion

        private Unit _unit;
        private ManaAbility _manaAbility;

        private Image _mana;

        protected override void Awake()
        {
            base.Awake();

            BindImage(typeof(Images));
            _mana = GetImage((int)Images.Mana_Fill);

            _unit = GetComponentInParent<Unit>();
            _unit.onAbilityInitialize += OnAbilityInitialize;
            _unit.onAbilityDeinitialize += OnAbilityDeinitialize;
        }

        private void OnDestroy()
        {
            _unit.onAbilityInitialize -= OnAbilityInitialize;
            _unit.onAbilityDeinitialize -= OnAbilityDeinitialize;
        }

        private void OnAbilityInitialize()
        {
            _manaAbility = _unit.GetAbility<ManaAbility>();
            
            if (_manaAbility == null || _manaAbility.finalMaxMana <= 0)
            {
                Hide();
            }
            else
            {
                _manaAbility.onChangedMana += OnChangedMana;
                Show();
            }
        }

        private void OnAbilityDeinitialize()
        {
            Hide();

            if (_manaAbility.finalMaxMana > 0)
            {
                _manaAbility.onChangedMana -= OnChangedMana;
            }
        }

        private void OnChangedMana(int mana)
        {
            var maxHp = _manaAbility.finalMaxMana;
            var per = mana / (float)maxHp;
            _mana.fillAmount = per;
        }
    }
}