using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentTalentSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private List<GlobalStatusTemplate> _talentTemplates;

        private GlobalStatusSystem _globalStatusSystem;

        public int talentCount => _talentTemplates.Count;

        public void Initialize()
        {
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            foreach (var template in _talentTemplates)
            {
                _globalStatusSystem.ApplyGlobalStatus(template, int.MaxValue, null);
            }
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;
        }

        public string GetTalentTitle(EEffectType effectType)
        {
            return effectType switch
            {
                EEffectType.ATKIncrease => "전투력 증가(%)",
                EEffectType.AttackSpeedIncrease => "공격간격 감소(%)",
                EEffectType.CriticalHitChanceAdditional => "치명타 확률 추가",
                EEffectType.SkillCooldownIncrease => "스킬 가속 증가(%)",
                _ => effectType.ToString()
            };
        }

        public string GetTalentDescription(EEffectType effectType, int value)
        {
            return effectType switch
            {
                EEffectType.ATKIncrease => $"전투력 {value}% 증가",
                EEffectType.AttackSpeedIncrease => $"공격간격 {value}% 감소",
                EEffectType.CriticalHitChanceAdditional => $"치명타 확률 +{value}",
                EEffectType.SkillCooldownIncrease => $"스킬 가속 {value}% 증가",
                _ => effectType.ToString()
            };
        }
    }
}