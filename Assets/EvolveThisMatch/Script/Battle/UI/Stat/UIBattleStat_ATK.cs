using EvolveThisMatch.Core;
using System.Text;

namespace EvolveThisMatch.Battle
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
            return $"기본 전투력: {_attackAbility.baseATK}";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("N0");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var instance in _buffAbility.ATKAdditionalDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Add)}");
            }

            foreach (var instance in _buffAbility.ATKIncreaseDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Increase)}");
            }

            foreach (var instance in _buffAbility.ATKMultiplierDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}