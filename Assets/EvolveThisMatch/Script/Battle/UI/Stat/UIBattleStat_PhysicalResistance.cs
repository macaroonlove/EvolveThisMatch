using EvolveThisMatch.Core;
using System.Text;

namespace EvolveThisMatch.Battle
{
    public class UIBattleStat_PhysicalResistance : UIBattleStat<int>, IBattleStat
    {
        private DamageCalculateAbility _damageCalculateAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _damageCalculateAbility = _unit.GetAbility<DamageCalculateAbility>();
            _buffAbility = _unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = _unit.GetAbility<AbnormalStatusAbility>();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();

            _damageCalculateAbility = null;
            _buffAbility = null;
            _abnormalStatusAbility = null;
        }

        protected override int GetValue()
        {
            return _damageCalculateAbility.finalPhysicalResistance;
        }

        protected override string GetBaseValue()
        {
            return $"기본 방어력: {_damageCalculateAbility.basePhysicalResistance}";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("N0");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var instance in _buffAbility.PhysicalResistanceAdditionalDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Add)}");
            }

            foreach (var instance in _buffAbility.PhysicalResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Increase)}");
            }
            foreach (var instance in _abnormalStatusAbility.PhysicalResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Increase)}");
            }

            foreach (var instance in _buffAbility.PhysicalResistanceMultiplierDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}