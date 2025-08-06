using EvolveThisMatch.Core;
using System.Text;

namespace EvolveThisMatch.Battle
{
    public class UIBattleStat_SkillCooldown : UIBattleStat<int>, IBattleStat
    {
        private BuffAbility _buffAbility;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _buffAbility = _unit.GetAbility<BuffAbility>();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();

            _buffAbility = null;
        }

        protected override int GetValue()
        {
            float finalSkillCooldown = 0;

            foreach (var effect in _buffAbility.SkillCooldownIncreaseDataEffects)
            {
                finalSkillCooldown += effect.Key.GetValue(effect.Value.level);
            }

            return (int)(finalSkillCooldown * 100);
        }

        protected override string GetBaseValue()
        {
            return "기본 스킬 가속: 0";
        }

        protected override string GetValueText()
        {
            return GetValue().ToString("N0");
        }

        protected override string GetTooltip()
        {
            StringBuilder result = new StringBuilder();

            foreach (var effect in _buffAbility.SkillCooldownIncreaseDataEffects)
            {
                result.AppendLine($"{effect.Value.displayName} {ValueFormat(effect.Key.GetValue(effect.Value.level), EDataType.Increase)}");
            }

            return result.ToString();
        }
    }
}