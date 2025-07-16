using FrameWork.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 유닛의 버프들을 관리하는 클래스
    /// </summary>
    public class BuffAbility : AlwaysAbility
    {
        #region Effect List
        private List<MoveIncreaseDataEffect> _moveIncreaseDataEffects = new List<MoveIncreaseDataEffect>();
        private List<MoveMultiplierDataEffect> _moveMultiplierDataEffects = new List<MoveMultiplierDataEffect>();

        private Dictionary<ATKAdditionalDataEffect, string> _atkAdditionalDataEffects = new Dictionary<ATKAdditionalDataEffect, string>();
        private Dictionary<ATKIncreaseDataEffect, string> _atkIncreaseDataEffects = new Dictionary<ATKIncreaseDataEffect, string>();
        private Dictionary<ATKMultiplierDataEffect, string> _atkMultiplierDataEffects = new Dictionary<ATKMultiplierDataEffect, string>();

        private List<AttackCountAdditionalDataEffect> _attackCountAdditionalDataEffects = new List<AttackCountAdditionalDataEffect>();

        private Dictionary<AttackSpeedIncreaseDataEffect, string> _attackSpeedIncreaseDataEffects = new Dictionary<AttackSpeedIncreaseDataEffect, string>();
        private Dictionary<AttackSpeedMultiplierDataEffect, string> _attackSpeedMultiplierDataEffects = new Dictionary<AttackSpeedMultiplierDataEffect, string>();

        private List<AvoidanceAdditionalDataEffect> _avoidanceAdditionalDataEffects = new List<AvoidanceAdditionalDataEffect>();

        private List<PhysicalPenetrationAdditionalDataEffect> _physicalPenetrationAdditionalDataEffects = new List<PhysicalPenetrationAdditionalDataEffect>();
        private List<PhysicalPenetrationIncreaseDataEffect> _physicalPenetrationIncreaseDataEffects = new List<PhysicalPenetrationIncreaseDataEffect>();
        private List<PhysicalPenetrationMultiplierDataEffect> _physicalPenetrationMultiplierDataEffects = new List<PhysicalPenetrationMultiplierDataEffect>();

        private Dictionary<PhysicalResistanceAdditionalDataEffect, string> _physicalResistanceAdditionalDataEffects = new Dictionary<PhysicalResistanceAdditionalDataEffect, string>();
        private Dictionary<PhysicalResistanceIncreaseDataEffect, string> _physicalResistanceIncreaseDataEffects = new Dictionary<PhysicalResistanceIncreaseDataEffect, string>();
        private Dictionary<PhysicalResistanceMultiplierDataEffect, string> _physicalResistanceMultiplierDataEffects = new Dictionary<PhysicalResistanceMultiplierDataEffect, string>();

        private List<MagicPenetrationAdditionalDataEffect> _magicPenetrationAdditionalDataEffects = new List<MagicPenetrationAdditionalDataEffect>();
        private List<MagicPenetrationIncreaseDataEffect> _magicPenetrationIncreaseDataEffects = new List<MagicPenetrationIncreaseDataEffect>();
        private List<MagicPenetrationMultiplierDataEffect> _magicPenetrationMultiplierDataEffects = new List<MagicPenetrationMultiplierDataEffect>();

        private Dictionary<MagicResistanceAdditionalDataEffect, string> _magicResistanceAdditionalDataEffects = new Dictionary<MagicResistanceAdditionalDataEffect, string>();
        private Dictionary<MagicResistanceIncreaseDataEffect, string> _magicResistanceIncreaseDataEffects = new Dictionary<MagicResistanceIncreaseDataEffect, string>();
        private Dictionary<MagicResistanceMultiplierDataEffect, string> _magicResistanceMultiplierDataEffects = new Dictionary<MagicResistanceMultiplierDataEffect, string>();

        private List<DamageAdditionalDataEffect> _damageAdditionalDataEffects = new List<DamageAdditionalDataEffect>();
        private List<DamageIncreaseDataEffect> _damageIncreaseDataEffects = new List<DamageIncreaseDataEffect>();
        private List<DamageMultiplierDataEffect> _damageMultiplierDataEffects = new List<DamageMultiplierDataEffect>();

        private List<ReceiveDamageAdditionalDataEffect> _receiveDamageAdditionalDataEffects = new List<ReceiveDamageAdditionalDataEffect>();
        private List<ReceiveDamageIncreaseDataEffect> _receiveDamageIncreaseDataEffects = new List<ReceiveDamageIncreaseDataEffect>();
        private List<ReceiveDamageMultiplierDataEffect> _receiveDamageMultiplierDataEffects = new List<ReceiveDamageMultiplierDataEffect>();

        private List<CriticalHitChanceAdditionalDataEffect> _criticalHitChanceAdditionalDataEffects = new List<CriticalHitChanceAdditionalDataEffect>();
        private List<CriticalHitDamageAdditionalDataEffect> _criticalHitDamageAdditionalDataEffects = new List<CriticalHitDamageAdditionalDataEffect>();
        private List<CriticalHitDamageIncreaseDataEffect> _criticalHitDamageIncreaseDataEffects = new List<CriticalHitDamageIncreaseDataEffect>();
        private List<CriticalHitDamageMultiplierDataEffect> _criticalHitDamageMultiplierDataEffects = new List<CriticalHitDamageMultiplierDataEffect>();

        private List<MaxHPAdditionalDataEffect> _maxHPAdditionalDataEffects = new List<MaxHPAdditionalDataEffect>();
        private List<MaxHPIncreaseDataEffect> _maxHPIncreaseDataEffects = new List<MaxHPIncreaseDataEffect>();
        private List<MaxHPMultiplierDataEffect> _maxHPMultiplierDataEffects = new List<MaxHPMultiplierDataEffect>();

        private List<HealingAdditionalDataEffect> _healingAdditionalDataEffects = new List<HealingAdditionalDataEffect>();
        private List<HealingIncreaseDataEffect> _healingIncreaseDataEffects = new List<HealingIncreaseDataEffect>();
        private List<HealingMultiplierDataEffect> _healingMultiplierDataEffects = new List<HealingMultiplierDataEffect>();

        private Dictionary<SkillCooldownIncreaseDataEffect, string> _skillCooldownIncreaseDataEffects = new Dictionary<SkillCooldownIncreaseDataEffect, string>();

        private List<SetMinHPEffect> _setMinHPEffects = new List<SetMinHPEffect>();
        private List<SetAttackTypeEffect> _setAttackTypeEffects = new List<SetAttackTypeEffect>();
        private List<SetDamageTypeEffect> _setDamageTypeEffects = new List<SetDamageTypeEffect>();

        private List<UnableToTargetOfAttackEffect> _unableToTargetOfAttackEffects = new List<UnableToTargetOfAttackEffect>();

        #region 프로퍼티
        internal IReadOnlyList<MoveIncreaseDataEffect> MoveIncreaseDataEffects => _moveIncreaseDataEffects;
        internal IReadOnlyList<MoveMultiplierDataEffect> MoveMultiplierDataEffects => _moveMultiplierDataEffects;

        internal IReadOnlyDictionary<ATKAdditionalDataEffect, string> ATKAdditionalDataEffects => _atkAdditionalDataEffects;
        internal IReadOnlyDictionary<ATKIncreaseDataEffect, string> ATKIncreaseDataEffects => _atkIncreaseDataEffects;
        internal IReadOnlyDictionary<ATKMultiplierDataEffect, string> ATKMultiplierDataEffects => _atkMultiplierDataEffects;

        internal IReadOnlyList<AttackCountAdditionalDataEffect> AttackCountAdditionalDataEffects => _attackCountAdditionalDataEffects;

        internal IReadOnlyDictionary<AttackSpeedIncreaseDataEffect, string> AttackSpeedIncreaseDataEffects => _attackSpeedIncreaseDataEffects;
        internal IReadOnlyDictionary<AttackSpeedMultiplierDataEffect, string> AttackSpeedMultiplierDataEffects => _attackSpeedMultiplierDataEffects;

        internal IReadOnlyList<AvoidanceAdditionalDataEffect> AvoidanceAdditionalDataEffects => _avoidanceAdditionalDataEffects;

        internal IReadOnlyList<PhysicalPenetrationAdditionalDataEffect> PhysicalPenetrationAdditionalDataEffects => _physicalPenetrationAdditionalDataEffects;
        internal IReadOnlyList<PhysicalPenetrationIncreaseDataEffect> PhysicalPenetrationIncreaseDataEffects => _physicalPenetrationIncreaseDataEffects;
        internal IReadOnlyList<PhysicalPenetrationMultiplierDataEffect> PhysicalPenetrationMultiplierDataEffects => _physicalPenetrationMultiplierDataEffects;

        internal IReadOnlyDictionary<PhysicalResistanceAdditionalDataEffect, string> PhysicalResistanceAdditionalDataEffects => _physicalResistanceAdditionalDataEffects;
        internal IReadOnlyDictionary<PhysicalResistanceIncreaseDataEffect, string> PhysicalResistanceIncreaseDataEffects => _physicalResistanceIncreaseDataEffects;
        internal IReadOnlyDictionary<PhysicalResistanceMultiplierDataEffect, string> PhysicalResistanceMultiplierDataEffects => _physicalResistanceMultiplierDataEffects;

        internal IReadOnlyList<MagicPenetrationAdditionalDataEffect> MagicPenetrationAdditionalDataEffects => _magicPenetrationAdditionalDataEffects;
        internal IReadOnlyList<MagicPenetrationIncreaseDataEffect> MagicPenetrationIncreaseDataEffects => _magicPenetrationIncreaseDataEffects;
        internal IReadOnlyList<MagicPenetrationMultiplierDataEffect> MagicPenetrationMultiplierDataEffects => _magicPenetrationMultiplierDataEffects;

        internal IReadOnlyDictionary<MagicResistanceAdditionalDataEffect, string> MagicResistanceAdditionalDataEffects => _magicResistanceAdditionalDataEffects;
        internal IReadOnlyDictionary<MagicResistanceIncreaseDataEffect, string> MagicResistanceIncreaseDataEffects => _magicResistanceIncreaseDataEffects;
        internal IReadOnlyDictionary<MagicResistanceMultiplierDataEffect, string> MagicResistanceMultiplierDataEffects => _magicResistanceMultiplierDataEffects;

        internal IReadOnlyList<DamageAdditionalDataEffect> DamageAdditionalDataEffects => _damageAdditionalDataEffects;
        internal IReadOnlyList<DamageIncreaseDataEffect> DamageIncreaseDataEffects => _damageIncreaseDataEffects;
        internal IReadOnlyList<DamageMultiplierDataEffect> DamageMultiplierDataEffects => _damageMultiplierDataEffects;

        internal IReadOnlyList<ReceiveDamageAdditionalDataEffect> ReceiveDamageAdditionalDataEffects => _receiveDamageAdditionalDataEffects;
        internal IReadOnlyList<ReceiveDamageIncreaseDataEffect> ReceiveDamageIncreaseDataEffects => _receiveDamageIncreaseDataEffects;
        internal IReadOnlyList<ReceiveDamageMultiplierDataEffect> ReceiveDamageMultiplierDataEffects => _receiveDamageMultiplierDataEffects;

        internal IReadOnlyList<CriticalHitChanceAdditionalDataEffect> CriticalHitChanceAdditionalDataEffects => _criticalHitChanceAdditionalDataEffects;
        internal IReadOnlyList<CriticalHitDamageAdditionalDataEffect> CriticalHitDamageAdditionalDataEffects => _criticalHitDamageAdditionalDataEffects;
        internal IReadOnlyList<CriticalHitDamageIncreaseDataEffect> CriticalHitDamageIncreaseDataEffects => _criticalHitDamageIncreaseDataEffects;
        internal IReadOnlyList<CriticalHitDamageMultiplierDataEffect> CriticalHitDamageMultiplierDataEffects => _criticalHitDamageMultiplierDataEffects;

        internal IReadOnlyList<MaxHPAdditionalDataEffect> MaxHPAdditionalDataEffects => _maxHPAdditionalDataEffects;
        internal IReadOnlyList<MaxHPIncreaseDataEffect> MaxHPIncreaseDataEffects => _maxHPIncreaseDataEffects;
        internal IReadOnlyList<MaxHPMultiplierDataEffect> MaxHPMultiplierDataEffects => _maxHPMultiplierDataEffects;

        internal IReadOnlyList<HealingAdditionalDataEffect> HealingAdditionalDataEffects => _healingAdditionalDataEffects;
        internal IReadOnlyList<HealingIncreaseDataEffect> HealingIncreaseDataEffects => _healingIncreaseDataEffects;
        internal IReadOnlyList<HealingMultiplierDataEffect> HealingMultiplierDataEffects => _healingMultiplierDataEffects;

        internal IReadOnlyDictionary<SkillCooldownIncreaseDataEffect, string> SkillCooldownIncreaseDataEffects => _skillCooldownIncreaseDataEffects;

        internal IReadOnlyList<SetMinHPEffect> SetMinHPEffects => _setMinHPEffects;
        internal IReadOnlyList<SetAttackTypeEffect> SetAttackTypeEffects => _setAttackTypeEffects;
        internal IReadOnlyList<SetDamageTypeEffect> SetDamageTypeEffects => _setDamageTypeEffects;

        internal IReadOnlyList<UnableToTargetOfAttackEffect> UnableToTargetOfAttackEffects => _unableToTargetOfAttackEffects;
        #endregion
        #endregion

        private Dictionary<BuffTemplate, StatusInstance> statusDic = new Dictionary<BuffTemplate, StatusInstance>();

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private List<BuffTemplate> statusList = new List<BuffTemplate>();
#endif

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            unit.GetAbility<AttackAbility>().onAttack += RemoveStatusByAttack;
            unit.GetAbility<HealthAbility>().onDeath += ClearStatusEffects;
        }

        internal override void Deinitialize()
        {
            unit.GetAbility<AttackAbility>().onAttack -= RemoveStatusByAttack;
            unit.GetAbility<HealthAbility>().onDeath -= ClearStatusEffects;
        }

        internal void ApplyBuff(BuffTemplate template, float duration)
        {
            if (this == null || gameObject == null) return;

            var isContained = false;

            if (statusDic.ContainsKey(template))
            {
                isContained = true;

                var instance = statusDic[template];
                if (instance.IsOld(duration))
                {
                    instance.duration = duration;
                    instance.startTime = Time.time;

                    if (template.useAttackCountLimit)
                    {
                        instance.useCountLimit = true;
                        instance.count = template.attackCount;
                    }
                    return;
                }
                else
                {
                    return;
                }
            }

            if (template.delay > 0)
            {
                StartCoroutine(CoAddStatus(template, duration, isContained));
            }
            else
            {
                AddStatus(template, duration, isContained);
            }
        }

        private IEnumerator CoAddStatus(BuffTemplate template, float duration, bool isContained)
        {
            yield return new WaitForSeconds(template.delay);
            AddStatus(template, duration, isContained);
        }

        /// <summary>
        /// 버프 추가
        /// </summary>
        private void AddStatus(BuffTemplate template, float duration, bool isContained)
        {
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);
            
            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }
            
            // 공격시 상태이상이 해제되야 한다면
            if (template.useAttackCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.attackCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            // 버프 효과 적용 (동일한 버프 효과는 중복되지 않음)
            if (isContained == false)
            {
                ExecuteApplyFX(template);

                foreach (var effect in template.effects)
                {
                    if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                    {
                        _moveIncreaseDataEffects.Add(moveIncreaseDataEffect);
                    }
                    else if (effect is MoveMultiplierDataEffect moveMultiplierDataEffect)
                    {
                        _moveMultiplierDataEffects.Add(moveMultiplierDataEffect);
                    }

                    else if (effect is ATKAdditionalDataEffect atkAdditionalDataEffects)
                    {
                        _atkAdditionalDataEffects.Add(atkAdditionalDataEffects, template.displayName);
                    }
                    else if (effect is ATKIncreaseDataEffect atkIncreaseDataEffect)
                    {
                        _atkIncreaseDataEffects.Add(atkIncreaseDataEffect, template.displayName);
                    }
                    else if (effect is ATKMultiplierDataEffect atkMultiplierDataEffect)
                    {
                        _atkMultiplierDataEffects.Add(atkMultiplierDataEffect, template.displayName);
                    }

                    else if (effect is AttackCountAdditionalDataEffect attackCountAdditionalDataEffect)
                    {
                        _attackCountAdditionalDataEffects.Add(attackCountAdditionalDataEffect);
                    }

                    else if (effect is AttackSpeedIncreaseDataEffect AttackSpeedIncreaseDataEffect)
                    {
                        _attackSpeedIncreaseDataEffects.Add(AttackSpeedIncreaseDataEffect, template.displayName);
                    }
                    else if (effect is AttackSpeedMultiplierDataEffect AttackSpeedMultiplierDataEffect)
                    {
                        _attackSpeedMultiplierDataEffects.Add(AttackSpeedMultiplierDataEffect, template.displayName);
                    }

                    else if (effect is AvoidanceAdditionalDataEffect AvoidanceAdditionalDataEffect)
                    {
                        _avoidanceAdditionalDataEffects.Add(AvoidanceAdditionalDataEffect);
                    }

                    else if (effect is PhysicalPenetrationAdditionalDataEffect physicalPenetrationAdditionalDataEffect)
                    {
                        _physicalPenetrationAdditionalDataEffects.Add(physicalPenetrationAdditionalDataEffect);
                    }
                    else if (effect is PhysicalPenetrationIncreaseDataEffect physicalPenetrationIncreaseDataEffect)
                    {
                        _physicalPenetrationIncreaseDataEffects.Add(physicalPenetrationIncreaseDataEffect);
                    }
                    else if (effect is PhysicalPenetrationMultiplierDataEffect physicalPenetrationMultiplierDataEffect)
                    {
                        _physicalPenetrationMultiplierDataEffects.Add(physicalPenetrationMultiplierDataEffect);
                    }

                    else if (effect is PhysicalResistanceAdditionalDataEffect physicalResistanceAdditionalDataEffect)
                    {
                        _physicalResistanceAdditionalDataEffects.Add(physicalResistanceAdditionalDataEffect, template.displayName);
                    }
                    else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                    {
                        _physicalResistanceIncreaseDataEffects.Add(physicalResistanceIncreaseDataEffect, template.displayName);
                    }
                    else if (effect is PhysicalResistanceMultiplierDataEffect physicalResistanceMultiplierDataEffect)
                    {
                        _physicalResistanceMultiplierDataEffects.Add(physicalResistanceMultiplierDataEffect, template.displayName);
                    }

                    else if (effect is MagicPenetrationAdditionalDataEffect magicPenetrationAdditionalDataEffect)
                    {
                        _magicPenetrationAdditionalDataEffects.Add(magicPenetrationAdditionalDataEffect);
                    }
                    else if (effect is MagicPenetrationIncreaseDataEffect magicPenetrationIncreaseDataEffect)
                    {
                        _magicPenetrationIncreaseDataEffects.Add(magicPenetrationIncreaseDataEffect);
                    }
                    else if (effect is MagicPenetrationMultiplierDataEffect magicPenetrationMultiplierDataEffect)
                    {
                        _magicPenetrationMultiplierDataEffects.Add(magicPenetrationMultiplierDataEffect);
                    }

                    else if (effect is MagicResistanceAdditionalDataEffect magicResistanceAdditionalDataEffect)
                    {
                        _magicResistanceAdditionalDataEffects.Add(magicResistanceAdditionalDataEffect, template.displayName);
                    }
                    else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                    {
                        _magicResistanceIncreaseDataEffects.Add(magicResistanceIncreaseDataEffect, template.displayName);
                    }
                    else if (effect is MagicResistanceMultiplierDataEffect magicResistanceMultiplierDataEffect)
                    {
                        _magicResistanceMultiplierDataEffects.Add(magicResistanceMultiplierDataEffect, template.displayName);
                    }

                    else if (effect is DamageAdditionalDataEffect damageAdditionalDataEffect)
                    {
                        _damageAdditionalDataEffects.Add(damageAdditionalDataEffect);
                    }
                    else if (effect is DamageIncreaseDataEffect damageIncreaseDataEffect)
                    {
                        _damageIncreaseDataEffects.Add(damageIncreaseDataEffect);
                    }
                    else if (effect is DamageMultiplierDataEffect damageMultiplierDataEffect)
                    {
                        _damageMultiplierDataEffects.Add(damageMultiplierDataEffect);
                    }

                    else if (effect is ReceiveDamageAdditionalDataEffect receiveDamageAdditionalDataEffect)
                    {
                        _receiveDamageAdditionalDataEffects.Add(receiveDamageAdditionalDataEffect);
                    }
                    else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                    {
                        _receiveDamageIncreaseDataEffects.Add(receiveDamageIncreaseDataEffect);
                    }
                    else if (effect is ReceiveDamageMultiplierDataEffect receiveDamageMultiplierDataEffect)
                    {
                        _receiveDamageMultiplierDataEffects.Add(receiveDamageMultiplierDataEffect);
                    }

                    else if (effect is CriticalHitChanceAdditionalDataEffect criticalHitChanceAdditionalDataEffect)
                    {
                        _criticalHitChanceAdditionalDataEffects.Add(criticalHitChanceAdditionalDataEffect);
                    }
                    else if (effect is CriticalHitDamageAdditionalDataEffect criticalHitDamageAdditionalDataEffect)
                    {
                        _criticalHitDamageAdditionalDataEffects.Add(criticalHitDamageAdditionalDataEffect);
                    }
                    else if (effect is CriticalHitDamageIncreaseDataEffect criticalHitDamageIncreaseDataEffect)
                    {
                        _criticalHitDamageIncreaseDataEffects.Add(criticalHitDamageIncreaseDataEffect);
                    }
                    else if (effect is CriticalHitDamageMultiplierDataEffect criticalHitDamageMultiplierDataEffect)
                    {
                        _criticalHitDamageMultiplierDataEffects.Add(criticalHitDamageMultiplierDataEffect);
                    }

                    else if (effect is MaxHPAdditionalDataEffect maxHPAdditionalDataEffect)
                    {
                        _maxHPAdditionalDataEffects.Add(maxHPAdditionalDataEffect);
                    }
                    else if (effect is MaxHPIncreaseDataEffect maxHPIncreaseDataEffect)
                    {
                        _maxHPIncreaseDataEffects.Add(maxHPIncreaseDataEffect);
                    }
                    else if (effect is MaxHPMultiplierDataEffect maxHPMultiplierDataEffect)
                    {
                        _maxHPMultiplierDataEffects.Add(maxHPMultiplierDataEffect);
                    }

                    else if (effect is HealingAdditionalDataEffect healingAdditionalDataEffect)
                    {
                        _healingAdditionalDataEffects.Add(healingAdditionalDataEffect);
                    }
                    else if (effect is HealingIncreaseDataEffect healingIncreaseDataEffect)
                    {
                        _healingIncreaseDataEffects.Add(healingIncreaseDataEffect);
                    }
                    else if (effect is HealingMultiplierDataEffect healingMultiplierDataEffect)
                    {
                        _healingMultiplierDataEffects.Add(healingMultiplierDataEffect);
                    }
                    
                    else if (effect is SkillCooldownIncreaseDataEffect skillCooldownIncreaseDataEffect)
                    {
                        _skillCooldownIncreaseDataEffects.Add(skillCooldownIncreaseDataEffect, template.displayName);
                    }

                    else if (effect is SetMinHPEffect setMinHPEffect)
                    {
                        _setMinHPEffects.Add(setMinHPEffect);
                    }
                    else if (effect is SetAttackTypeEffect setAttackTypeEffect)
                    {
                        _setAttackTypeEffects.Add(setAttackTypeEffect);
                    }
                    else if (effect is SetDamageTypeEffect setDamageTypeEffect)
                    {
                        _setDamageTypeEffects.Add(setDamageTypeEffect);
                    }

                    else if (effect is UnableToTargetOfAttackEffect unableToTargetOfAttackEffect)
                    {
                        _unableToTargetOfAttackEffects.Add(unableToTargetOfAttackEffect);
                    }
                }
            }
        }

        private IEnumerator CoStatus(StatusInstance statusInstance, BuffTemplate template)
        {
            while (statusInstance.IsCompete == false)
            {
                yield return null;
            }

            RemoveBuff(template);
        }

        #region 콜백 메서드
        private void RemoveStatusByAttack()
        {
            List<BuffTemplate> templates = new List<BuffTemplate>();
            foreach (var status in statusDic)
            {
                var template = status.Key;
                var instance = status.Value;

                if (instance.useCountLimit)
                {
                    instance.count--;

                    if (instance.count == 0)
                    {
                        RemoveStatus(template.effects);
                        
                        if (instance.corutine != null)
                        {
                            StopCoroutine(instance.corutine);
                            instance.corutine = null;
                        }
                        templates.Add(template);
                    }
                }
            }

            foreach (var template in templates)
            {
                if (statusDic.ContainsKey(template))
                {
                    statusDic.Remove(template);

#if UNITY_EDITOR
                    statusList.Remove(template);
#endif

                    ExecuteRemoveFX(template);
                }
            }
        }

        private void ClearStatusEffects()
        {
            foreach (var status in statusDic)
            {
                var instance = status.Value;

                RemoveStatus(status.Key.effects);

                ExecuteRemoveFX(status.Key);

                if (instance.corutine != null)
                {
                    StopCoroutine(instance.corutine);
                    instance.corutine = null;
                }
            }

            statusDic.Clear();

#if UNITY_EDITOR
            statusList.Clear();
#endif
        }
        #endregion

        internal void RemoveBuff(BuffTemplate template)
        {
            RemoveStatus(template.effects);

            if (statusDic.ContainsKey(template))
            {
                statusDic.Remove(template);

#if UNITY_EDITOR
                statusList.Remove(template);
#endif

                ExecuteRemoveFX(template);
            }
        }

        /// <summary>
        /// 버프 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                {
                    _moveIncreaseDataEffects.Remove(moveIncreaseDataEffect);
                }
                else if (effect is MoveMultiplierDataEffect moveMultiplierDataEffect)
                {
                    _moveMultiplierDataEffects.Remove(moveMultiplierDataEffect);
                }

                else if (effect is ATKAdditionalDataEffect atkAdditionalDataEffects)
                {
                    _atkAdditionalDataEffects.Remove(atkAdditionalDataEffects);
                }
                else if (effect is ATKIncreaseDataEffect atkIncreaseDataEffect)
                {
                    _atkIncreaseDataEffects.Remove(atkIncreaseDataEffect);
                }
                else if (effect is ATKMultiplierDataEffect atkMultiplierDataEffect)
                {
                    _atkMultiplierDataEffects.Remove(atkMultiplierDataEffect);
                }

                else if (effect is AttackCountAdditionalDataEffect attackCountAdditionalDataEffect)
                {
                    _attackCountAdditionalDataEffects.Remove(attackCountAdditionalDataEffect);
                }

                else if (effect is AttackSpeedIncreaseDataEffect AttackSpeedIncreaseDataEffect)
                {
                    _attackSpeedIncreaseDataEffects.Remove(AttackSpeedIncreaseDataEffect);
                }
                else if (effect is AttackSpeedMultiplierDataEffect AttackSpeedMultiplierDataEffect)
                {
                    _attackSpeedMultiplierDataEffects.Remove(AttackSpeedMultiplierDataEffect);
                }

                else if (effect is AvoidanceAdditionalDataEffect AvoidanceAdditionalDataEffect)
                {
                    _avoidanceAdditionalDataEffects.Remove(AvoidanceAdditionalDataEffect);
                }

                else if (effect is PhysicalPenetrationAdditionalDataEffect physicalPenetrationAdditionalDataEffect)
                {
                    _physicalPenetrationAdditionalDataEffects.Remove(physicalPenetrationAdditionalDataEffect);
                }
                else if (effect is PhysicalPenetrationIncreaseDataEffect physicalPenetrationIncreaseDataEffect)
                {
                    _physicalPenetrationIncreaseDataEffects.Remove(physicalPenetrationIncreaseDataEffect);
                }
                else if (effect is PhysicalPenetrationMultiplierDataEffect physicalPenetrationMultiplierDataEffect)
                {
                    _physicalPenetrationMultiplierDataEffects.Remove(physicalPenetrationMultiplierDataEffect);
                }

                else if (effect is PhysicalResistanceAdditionalDataEffect physicalResistanceAdditionalDataEffect)
                {
                    _physicalResistanceAdditionalDataEffects.Remove(physicalResistanceAdditionalDataEffect);
                }
                else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                {
                    _physicalResistanceIncreaseDataEffects.Remove(physicalResistanceIncreaseDataEffect);
                }
                else if (effect is PhysicalResistanceMultiplierDataEffect physicalResistanceMultiplierDataEffect)
                {
                    _physicalResistanceMultiplierDataEffects.Remove(physicalResistanceMultiplierDataEffect);
                }

                else if (effect is MagicPenetrationAdditionalDataEffect magicPenetrationAdditionalDataEffect)
                {
                    _magicPenetrationAdditionalDataEffects.Remove(magicPenetrationAdditionalDataEffect);
                }
                else if (effect is MagicPenetrationIncreaseDataEffect magicPenetrationIncreaseDataEffect)
                {
                    _magicPenetrationIncreaseDataEffects.Remove(magicPenetrationIncreaseDataEffect);
                }
                else if (effect is MagicPenetrationMultiplierDataEffect magicPenetrationMultiplierDataEffect)
                {
                    _magicPenetrationMultiplierDataEffects.Remove(magicPenetrationMultiplierDataEffect);
                }

                else if (effect is MagicResistanceAdditionalDataEffect magicResistanceAdditionalDataEffect)
                {
                    _magicResistanceAdditionalDataEffects.Remove(magicResistanceAdditionalDataEffect);
                }
                else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                {
                    _magicResistanceIncreaseDataEffects.Remove(magicResistanceIncreaseDataEffect);
                }
                else if (effect is MagicResistanceMultiplierDataEffect magicResistanceMultiplierDataEffect)
                {
                    _magicResistanceMultiplierDataEffects.Remove(magicResistanceMultiplierDataEffect);
                }

                else if (effect is DamageAdditionalDataEffect damageAdditionalDataEffect)
                {
                    _damageAdditionalDataEffects.Remove(damageAdditionalDataEffect);
                }
                else if (effect is DamageIncreaseDataEffect damageIncreaseDataEffect)
                {
                    _damageIncreaseDataEffects.Remove(damageIncreaseDataEffect);
                }
                else if (effect is DamageMultiplierDataEffect damageMultiplierDataEffect)
                {
                    _damageMultiplierDataEffects.Remove(damageMultiplierDataEffect);
                }

                else if (effect is ReceiveDamageAdditionalDataEffect receiveDamageAdditionalDataEffect)
                {
                    _receiveDamageAdditionalDataEffects.Remove(receiveDamageAdditionalDataEffect);
                }
                else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                {
                    _receiveDamageIncreaseDataEffects.Remove(receiveDamageIncreaseDataEffect);
                }
                else if (effect is ReceiveDamageMultiplierDataEffect receiveDamageMultiplierDataEffect)
                {
                    _receiveDamageMultiplierDataEffects.Remove(receiveDamageMultiplierDataEffect);
                }

                else if (effect is CriticalHitChanceAdditionalDataEffect criticalHitChanceAdditionalDataEffect)
                {
                    _criticalHitChanceAdditionalDataEffects.Remove(criticalHitChanceAdditionalDataEffect);
                }
                else if (effect is CriticalHitDamageAdditionalDataEffect criticalHitDamageAdditionalDataEffect)
                {
                    _criticalHitDamageAdditionalDataEffects.Remove(criticalHitDamageAdditionalDataEffect);
                }
                else if (effect is CriticalHitDamageIncreaseDataEffect criticalHitDamageIncreaseDataEffect)
                {
                    _criticalHitDamageIncreaseDataEffects.Remove(criticalHitDamageIncreaseDataEffect);
                }
                else if (effect is CriticalHitDamageMultiplierDataEffect criticalHitDamageMultiplierDataEffect)
                {
                    _criticalHitDamageMultiplierDataEffects.Remove(criticalHitDamageMultiplierDataEffect);
                }

                else if (effect is MaxHPAdditionalDataEffect maxHPAdditionalDataEffect)
                {
                    _maxHPAdditionalDataEffects.Remove(maxHPAdditionalDataEffect);
                }
                else if (effect is MaxHPIncreaseDataEffect maxHPIncreaseDataEffect)
                {
                    _maxHPIncreaseDataEffects.Remove(maxHPIncreaseDataEffect);
                }
                else if (effect is MaxHPMultiplierDataEffect maxHPMultiplierDataEffect)
                {
                    _maxHPMultiplierDataEffects.Remove(maxHPMultiplierDataEffect);
                }

                else if (effect is HealingAdditionalDataEffect healingAdditionalDataEffect)
                {
                    _healingAdditionalDataEffects.Remove(healingAdditionalDataEffect);
                }
                else if (effect is HealingIncreaseDataEffect healingIncreaseDataEffect)
                {
                    _healingIncreaseDataEffects.Remove(healingIncreaseDataEffect);
                }
                else if (effect is HealingMultiplierDataEffect healingMultiplierDataEffect)
                {
                    _healingMultiplierDataEffects.Remove(healingMultiplierDataEffect);
                }

                else if (effect is SkillCooldownIncreaseDataEffect skillCooldownIncreaseDataEffect)
                {
                    _skillCooldownIncreaseDataEffects.Remove(skillCooldownIncreaseDataEffect);
                }

                else if (effect is SetMinHPEffect setMinHPEffect)
                {
                    _setMinHPEffects.Remove(setMinHPEffect);
                }
                else if (effect is SetAttackTypeEffect setAttackTypeEffect)
                {
                    _setAttackTypeEffects.Remove(setAttackTypeEffect);
                }
                else if (effect is SetDamageTypeEffect setDamageTypeEffect)
                {
                    _setDamageTypeEffects.Remove(setDamageTypeEffect);
                }

                else if (effect is UnableToTargetOfAttackEffect unableToTargetOfAttackEffect)
                {
                    _unableToTargetOfAttackEffects.Remove(unableToTargetOfAttackEffect);
                }
            }
        }

        #region 유틸리티 메서드
        internal bool Contains(BuffTemplate template)
        {
            return statusDic.ContainsKey(template);
        }

        internal bool Contains(List<BuffTemplate> templates)
        {
            var isContains = false;
            foreach (var template in templates)
            {
                if (statusDic.ContainsKey(template))
                {
                    isContains = true;
                }
            }
            return isContains;
        }
        #endregion

        #region FX
        private void ExecuteApplyFX(BuffTemplate template)
        {
            if (template.applyFX != null)
            {
                template.applyFX.Play(unit);
            }
        }

        private void ExecuteRemoveFX(BuffTemplate template)
        {
            if (template.removeFX != null)
            {
                template.removeFX.Play(unit);
            }
        }
        #endregion
    }
}