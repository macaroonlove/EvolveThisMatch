using EvolveThisMatch.Core;
using System.Text;

namespace EvolveThisMatch.Battle
{
    public class UIBattleStat_MagicResistance : UIBattleStat<int>, IBattleStat
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
            return _damageCalculateAbility.finalMagicResistance;
        }

        protected override string GetBaseValue()
        {
            return $"기본 마법 저항력: {_damageCalculateAbility.baseMagicResistance}";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("N0");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var instance in _buffAbility.MagicResistanceAdditionalDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Add)}");
            }

            foreach (var instance in _buffAbility.MagicResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Increase)}");
            }
            foreach (var instance in _abnormalStatusAbility.MagicResistanceIncreaseDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Increase)}");
            }

            foreach (var instance in _buffAbility.MagicResistanceMultiplierDataEffects)
            {
                result.AppendLine($"{instance.displayName} {ValueFormat(instance.effect.GetValue(_unit.effectContext, instance.context), EDataType.Multiplier)}");
            }

            return result.ToString();
        }
    }
}