using System.Text;

namespace EvolveThisMatch.Core
{
    public class UIBattleStat_ATK : UIBattleStat<int>, IBattleStat
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

        protected override int GetValue()
        {
            return _attackAbility.finalATK;
        }

        protected override string GetBaseValue()
        {
            return $"기본 공격력: {_attackAbility.baseATK}";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("N0");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var effect in _buffAbility.ATKAdditionalDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Add)}");
            }

            foreach (var effect in _buffAbility.ATKIncreaseDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Increase)}");
            }

            foreach (var effect in _buffAbility.ATKMultiplierDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}