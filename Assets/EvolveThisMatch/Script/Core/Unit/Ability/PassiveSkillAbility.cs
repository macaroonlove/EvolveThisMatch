using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 유닛의 패시브 스킬 사용을 제어합니다.
    /// </summary>
    public class PassiveSkillAbility : AlwaysAbility
    {
        #region Effect List
        private List<UnitEffect> _attackEventEffects = new List<UnitEffect>();
        private List<UnitEffect> _hitEventEffects = new List<UnitEffect>();
        private List<UnitEffect> _healEventEffects = new List<UnitEffect>();
        private List<UnitEffect> _destroyShieldEventEffects = new List<UnitEffect>();

        #region 프로퍼티
        internal IReadOnlyList<UnitEffect> attackEventEffects => _attackEventEffects;
        internal IReadOnlyList<UnitEffect> hitEventEffects => _hitEventEffects;
        internal IReadOnlyList<UnitEffect> healEventEffects => _healEventEffects;
        internal IReadOnlyList<UnitEffect> destroyShieldEventEffects => _destroyShieldEventEffects;
        #endregion
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            if (unit is AgentUnit agentUnit && agentUnit.template.skillTemplates.Count > 0)
            {
                CheckUnlockSkill(agentUnit.agentData);
            }
            else if (unit is SummonUnit summonUnit && summonUnit.template.skillTemplates.Count > 0)
            {
                InitializePassiveSkill(summonUnit.template.skillTemplates);
            }
            else if (unit is EnemyUnit enemyUnit && enemyUnit.enemyData.template.skillTemplates.Count > 0)
            {
                InitializePassiveSkill(enemyUnit.enemyData.template.skillTemplates);
            }
        }

        private async void CheckUnlockSkill(AgentBattleData agentData)
        {
            await UniTask.WaitUntil(() => agentData.skillUnlock >= 2);
            InitializePassiveSkill(agentData.agentTemplate.skillTemplates);
        }

        private void InitializePassiveSkill(IReadOnlyList<SkillTemplate> skillTemplates)
        {
            foreach (var skillTemplate in skillTemplates)
            {
                if (skillTemplate is PassiveSkillTemplate skill)
                {
                    foreach (var trigger in skill.triggers)
                    {
                        // 상시 적용될 효과
                        if (trigger is AlwaysUnitTrigger alwaysUnitTrigger)
                        {
                            foreach (var effect in alwaysUnitTrigger.effects)
                            {
                                if (effect is SingleUnitEffect singleUnitEffect)
                                {
                                    singleUnitEffect.Execute(unit.effectContext, unit);
                                }
                            }
                        }
                        // 기본 공격/회복 시 적용될 효과
                        else if (trigger is AttackEventUnitTrigger attackEventUnitTrigger)
                        {
                            foreach (var effect in attackEventUnitTrigger.effects)
                            {
                                if (effect is UnitEffect eventEffect)
                                {
                                    _attackEventEffects.Add(eventEffect);
                                }
                            }
                        }
                        // 피격 시 적용될 효과
                        else if (trigger is HitEventUnitTrigger hitEventUnitTrigger)
                        {
                            foreach (var effect in hitEventUnitTrigger.effects)
                            {
                                if (effect is UnitEffect eventEffect)
                                {
                                    _hitEventEffects.Add(eventEffect);
                                }
                            }
                        }
                        // 회복을 받을 시 적용될 효과
                        else if (trigger is HealEventUnitTrigger healEventUnitTrigger)
                        {
                            foreach (var effect in healEventUnitTrigger.effects)
                            {
                                if (effect is UnitEffect unitEffect)
                                {
                                    _healEventEffects.Add(unitEffect);
                                }
                            }
                        }
                        // 보호막이 파괴될 때 적용될 효과
                        else if (trigger is DestroyShieldEventUnitTrigger destroyShieldEventUnitTrigger)
                        {
                            foreach (var effect in destroyShieldEventUnitTrigger.effects)
                            {
                                if (effect is UnitEffect unitEffect)
                                {
                                    _destroyShieldEventEffects.Add(unitEffect);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal override void Deinitialize()
        {
            _attackEventEffects.Clear();
            _hitEventEffects.Clear();
            _healEventEffects.Clear();
            _destroyShieldEventEffects.Clear();
        }
    }
}