using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class HealthTestCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            HealButton,
            ShieldInfinityButton,
            ShieldDurationButton,
        }
        #endregion

        private Unit _unit;

        protected override void Awake()
        {
            base.Awake();

            BindButton(typeof(Buttons));
            GetButton((int)Buttons.HealButton).onClick.AddListener(Heal);
            GetButton((int)Buttons.ShieldInfinityButton).onClick.AddListener(InfinityShield);
            GetButton((int)Buttons.ShieldDurationButton).onClick.AddListener(DurationShield);
        }

        internal void Initialize(Unit unit)
        {
            _unit = unit;
        }

        private void Heal()
        {
            _unit.healthAbility.Healed(Random.Range(1, 100));
        }

        private void InfinityShield()
        {
            _unit.healthAbility.AddShield(Random.Range(1, 100));
        }

        private void DurationShield()
        {
            _unit.healthAbility.AddShield(Random.Range(1, 100), 3);
        }
    }
}