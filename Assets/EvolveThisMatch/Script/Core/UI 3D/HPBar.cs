using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class HPBar : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            HP_Fill,
            Shield_Fill,
        }
        #endregion

        private Unit _unit;
        private HealthAbility _healthAbility;

        private Image _hp;
        private Image _shield;
        private Vector2 _shieldPos = new Vector2(0, -0.5f);

        protected override void Awake()
        {
            base.Awake();

            BindImage(typeof(Images));
            _hp = GetImage((int)Images.HP_Fill);
            _shield = GetImage((int)Images.Shield_Fill);

            _unit = GetComponentInParent<Unit>();
            
            _unit.onAbilityInitialize += OnAbilityInitialize;
            _unit.onAbilityDeinitialize += OnAbilityDeinitialize;

            Hide(true);
        }

        private void OnDestroy()
        {
            _unit.onAbilityInitialize -= OnAbilityInitialize;
            _unit.onAbilityDeinitialize -= OnAbilityDeinitialize;
        }

        private void OnAbilityInitialize()
        {
            _healthAbility = _unit.healthAbility;

            OnChangedHealth(_healthAbility.currentHP);
            OnChangedShield(0);

            _healthAbility.onChangedHealth += OnChangedHealth;
            _healthAbility.onChangedShield += OnChangedShield;
        }

        private void OnAbilityDeinitialize()
        {
            _healthAbility.onChangedHealth -= OnChangedHealth;
            _healthAbility.onChangedShield -= OnChangedShield;
        }

        private void ShowOrHide()
        {
            if (_healthAbility.shieldCount > 0 || _healthAbility.currentHP != _healthAbility.finalMaxHP)
            {
                Show(true);
            }
            else
            {
                Hide(true);
            }
        }

        private void OnChangedHealth(int hp)
        {
            var maxHp = _healthAbility.finalMaxHP;
            var per = hp / (float)maxHp;
            _hp.fillAmount = per;

            ShowOrHide();
        }

        private void OnChangedShield(int shield)
        {
            var maxHp = _healthAbility.finalMaxHP;
            var hp = _healthAbility.currentHP;
            var per = shield / (float)maxHp;
            var per2 = hp / (float)maxHp;
            _shield.fillAmount = per;

            if ((maxHp - hp) >= shield)
            {
                _shieldPos.x = per2 * 96f;
                _shield.rectTransform.anchoredPosition = _shieldPos;
                _shield.fillOrigin = 0;
            }
            else
            {
                _shieldPos.x = 0;
                _shield.rectTransform.anchoredPosition = _shieldPos;
                _shield.fillOrigin = 1;
            }

            ShowOrHide();
        }
    }
}