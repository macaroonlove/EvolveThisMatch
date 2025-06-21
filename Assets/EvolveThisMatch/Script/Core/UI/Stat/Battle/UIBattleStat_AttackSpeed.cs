using System.Text;

namespace EvolveThisMatch.Core
{
    public class UIBattleStat_AttackSpeed : UIBattleStat<float>, IBattleStat
    {
        private AttackAbility _attackAbility;
        private BuffAbility _buffAbility;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _attackAbility = _unit.GetAbility<AttackAbility>();
            _buffAbility = _unit.GetAbility<BuffAbility>();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();

            _attackAbility = null;
            _buffAbility = null;
        }

        protected override float GetValue()
        {
            return _attackAbility.finalAttackTerm;
        }

        protected override string GetBaseValue()
        {
            return $"기본 공격 간격: {_attackAbility.baseAttackTerm}";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("F2");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var effect in _buffAbility.AttackSpeedIncreaseDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Increase)}");
            }

            foreach (var effect in _buffAbility.AttackSpeedMultiplierDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}