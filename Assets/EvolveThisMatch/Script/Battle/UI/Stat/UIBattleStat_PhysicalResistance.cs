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

            foreach (var effect in _buffAbility.PhysicalResistanceAdditionalDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Add)}");
            }

            foreach (var effect in _buffAbility.PhysicalResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Increase)}");
            }
            foreach (var effect in _abnormalStatusAbility.PhysicalResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Increase)}");
            }

            foreach (var effect in _buffAbility.PhysicalResistanceMultiplierDataEffects)
            {
                result.AppendLine($"{effect.Value} {ValueFormat(effect.Key.value, EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}