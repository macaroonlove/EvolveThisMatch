using EvolveThisMatch.Save;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public enum EEffectScaleBase
    {
        AgentLevel,
        AgentSync,
        ArtifactLevel,
        TomeLevel,
        Talent,
    }

    public enum EEffectType
    {
        ATKIncrease,
        AttackSpeedIncrease,
        CriticalHitChanceAdditional,
        SkillCooldownIncrease,
    }

    public class EffectContext
    {
        // 참조 타입만 추가 가능
        public AgentBattleData agentData;
        public AgentSaveData.Agent agentSaveData;
        public ItemSaveData.Artifact artifactSaveData;
        public ItemSaveData.Tome tomeSaveData;

        public int GetScaleValue(EEffectScaleBase scaleBase, EEffectType effectType)
        {
            int value = -1;
            switch (scaleBase)
            {
                case EEffectScaleBase.AgentLevel:
                    value = agentSaveData != null ? agentSaveData.level : -2;
                    break;
                case EEffectScaleBase.AgentSync:
                    value = agentData != null ? agentData.sync : -2;
                    break;
                case EEffectScaleBase.ArtifactLevel:
                    value = artifactSaveData != null ? artifactSaveData.level : -2;
                    break;
                case EEffectScaleBase.TomeLevel:
                    value = tomeSaveData != null ? tomeSaveData.level : -2;
                    break;
                case EEffectScaleBase.Talent:
                    value = agentSaveData != null ? GetTalentValueByType(effectType) : -2;
                    break;
            }

            if (value == -1)
            {
                value = 1;
#if UNITY_EDITOR
                Debug.LogWarning($"MutableContext: {scaleBase} 기준 데이터가 없습니다.");
#endif
            }

            return value;
        }

        private int GetTalentValueByType(EEffectType type)
        {
            int total = 0;

            var talent = agentSaveData.talent;
            for (int i = 0; i < talent.Length; i++)
            {
                if (talent[i].id == (int)type)
                    total += talent[i].value;
            }

            return total;
        }
    }
}