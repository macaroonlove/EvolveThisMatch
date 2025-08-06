using FrameWork.Editor;
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
        // Move
        private Dictionary<MoveIncreaseDataEffect, int> _moveIncreaseDataEffects = new Dictionary<MoveIncreaseDataEffect, int>();
        private Dictionary<MoveMultiplierDataEffect, int> _moveMultiplierDataEffects = new Dictionary<MoveMultiplierDataEffect, int>();

        // Attack
        private Dictionary<ATKAdditionalDataEffect, (int level, string displayName)> _atkAdditionalDataEffects = new Dictionary<ATKAdditionalDataEffect, (int, string)>();
        private Dictionary<ATKIncreaseDataEffect, (int level, string displayName)> _atkIncreaseDataEffects = new Dictionary<ATKIncreaseDataEffect, (int, string)>();
        private Dictionary<ATKMultiplierDataEffect, (int level, string displayName)> _atkMultiplierDataEffects = new Dictionary<ATKMultiplierDataEffect, (int, string)>();

        private Dictionary<AttackCountAdditionalDataEffect, int> _attackCountAdditionalDataEffects = new Dictionary<AttackCountAdditionalDataEffect, int>();

        private Dictionary<AttackSpeedIncreaseDataEffect, (int level, string displayName)> _attackSpeedIncreaseDataEffects = new Dictionary<AttackSpeedIncreaseDataEffect, (int, string)>();
        private Dictionary<AttackSpeedMultiplierDataEffect, (int level, string displayName)> _attackSpeedMultiplierDataEffects = new Dictionary<AttackSpeedMultiplierDataEffect, (int, string)>();

        // Avoidance
        private Dictionary<AvoidanceAdditionalDataEffect, int> _avoidanceAdditionalDataEffects = new Dictionary<AvoidanceAdditionalDataEffect, int>();

        // Physical Penetration
        private Dictionary<PhysicalPenetrationAdditionalDataEffect, int> _physicalPenetrationAdditionalDataEffects = new Dictionary<PhysicalPenetrationAdditionalDataEffect, int>();
        private Dictionary<PhysicalPenetrationIncreaseDataEffect, int> _physicalPenetrationIncreaseDataEffects = new Dictionary<PhysicalPenetrationIncreaseDataEffect, int>();
        private Dictionary<PhysicalPenetrationMultiplierDataEffect, int> _physicalPenetrationMultiplierDataEffects = new Dictionary<PhysicalPenetrationMultiplierDataEffect, int>();

        // Physical Resistance
        private Dictionary<PhysicalResistanceAdditionalDataEffect, (int level, string displayName)> _physicalResistanceAdditionalDataEffects = new Dictionary<PhysicalResistanceAdditionalDataEffect, (int, string)>();
        private Dictionary<PhysicalResistanceIncreaseDataEffect, (int level, string displayName)> _physicalResistanceIncreaseDataEffects = new Dictionary<PhysicalResistanceIncreaseDataEffect, (int, string)>();
        private Dictionary<PhysicalResistanceMultiplierDataEffect, (int level, string displayName)> _physicalResistanceMultiplierDataEffects = new Dictionary<PhysicalResistanceMultiplierDataEffect, (int, string)>();

        // Magic Penetration
        private Dictionary<MagicPenetrationAdditionalDataEffect, int> _magicPenetrationAdditionalDataEffects = new Dictionary<MagicPenetrationAdditionalDataEffect, int>();
        private Dictionary<MagicPenetrationIncreaseDataEffect, int> _magicPenetrationIncreaseDataEffects = new Dictionary<MagicPenetrationIncreaseDataEffect, int>();
        private Dictionary<MagicPenetrationMultiplierDataEffect, int> _magicPenetrationMultiplierDataEffects = new Dictionary<MagicPenetrationMultiplierDataEffect, int>();

        // Magic Resistance
        private Dictionary<MagicResistanceAdditionalDataEffect, (int level, string displayName)> _magicResistanceAdditionalDataEffects = new Dictionary<MagicResistanceAdditionalDataEffect, (int, string)>();
        private Dictionary<MagicResistanceIncreaseDataEffect, (int level, string displayName)> _magicResistanceIncreaseDataEffects = new Dictionary<MagicResistanceIncreaseDataEffect, (int, string)>();
        private Dictionary<MagicResistanceMultiplierDataEffect, (int level, string displayName)> _magicResistanceMultiplierDataEffects = new Dictionary<MagicResistanceMultiplierDataEffect, (int, string)>();

        // Damage
        private Dictionary<DamageAdditionalDataEffect, int> _damageAdditionalDataEffects = new Dictionary<DamageAdditionalDataEffect, int>();
        private Dictionary<DamageIncreaseDataEffect, int> _damageIncreaseDataEffects = new Dictionary<DamageIncreaseDataEffect, int>();
        private Dictionary<DamageMultiplierDataEffect, int> _damageMultiplierDataEffects = new Dictionary<DamageMultiplierDataEffect, int>();

        // Receive Damage
        private Dictionary<ReceiveDamageAdditionalDataEffect, int> _receiveDamageAdditionalDataEffects = new Dictionary<ReceiveDamageAdditionalDataEffect, int>();
        private Dictionary<ReceiveDamageIncreaseDataEffect, int> _receiveDamageIncreaseDataEffects = new Dictionary<ReceiveDamageIncreaseDataEffect, int>();
        private Dictionary<ReceiveDamageMultiplierDataEffect, int> _receiveDamageMultiplierDataEffects = new Dictionary<ReceiveDamageMultiplierDataEffect, int>();

        // Critical
        private Dictionary<CriticalHitChanceAdditionalDataEffect, int> _criticalHitChanceAdditionalDataEffects = new Dictionary<CriticalHitChanceAdditionalDataEffect, int>();
        private Dictionary<CriticalHitDamageAdditionalDataEffect, int> _criticalHitDamageAdditionalDataEffects = new Dictionary<CriticalHitDamageAdditionalDataEffect, int>();
        private Dictionary<CriticalHitDamageIncreaseDataEffect, int> _criticalHitDamageIncreaseDataEffects = new Dictionary<CriticalHitDamageIncreaseDataEffect, int>();
        private Dictionary<CriticalHitDamageMultiplierDataEffect, int> _criticalHitDamageMultiplierDataEffects = new Dictionary<CriticalHitDamageMultiplierDataEffect, int>();

        // Max HP
        private Dictionary<MaxHPAdditionalDataEffect, int> _maxHPAdditionalDataEffects = new Dictionary<MaxHPAdditionalDataEffect, int>();
        private Dictionary<MaxHPIncreaseDataEffect, int> _maxHPIncreaseDataEffects = new Dictionary<MaxHPIncreaseDataEffect, int>();
        private Dictionary<MaxHPMultiplierDataEffect, int> _maxHPMultiplierDataEffects = new Dictionary<MaxHPMultiplierDataEffect, int>();

        // Healing
        private Dictionary<HealingAdditionalDataEffect, int> _healingAdditionalDataEffects = new Dictionary<HealingAdditionalDataEffect, int>();
        private Dictionary<HealingIncreaseDataEffect, int> _healingIncreaseDataEffects = new Dictionary<HealingIncreaseDataEffect, int>();
        private Dictionary<HealingMultiplierDataEffect, int> _healingMultiplierDataEffects = new Dictionary<HealingMultiplierDataEffect, int>();

        // HP Recovery / Abnormal Status
        private Dictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int> _hpRecoveryPerSecByMaxHPIncreaseDataEffects = new Dictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int>();
        private Dictionary<AbnormalStatusResistanceAdditionalDataEffect, int> _abnormalStatusResistanceAdditionalDataEffects = new Dictionary<AbnormalStatusResistanceAdditionalDataEffect, int>();

        // Skill Cooldown
        private Dictionary<SkillCooldownIncreaseDataEffect, (int level, string displayName)> _skillCooldownIncreaseDataEffects = new Dictionary<SkillCooldownIncreaseDataEffect, (int, string)>();

        private List<SetMinHPEffect> _setMinHPEffects = new List<SetMinHPEffect>();
        private List<SetAttackTypeEffect> _setAttackTypeEffects = new List<SetAttackTypeEffect>();
        private List<SetDamageTypeEffect> _setDamageTypeEffects = new List<SetDamageTypeEffect>();

        private List<UnableToTargetOfAttackEffect> _unableToTargetOfAttackEffects = new List<UnableToTargetOfAttackEffect>();

        #region 프로퍼티
        internal IReadOnlyDictionary<MoveIncreaseDataEffect, int> MoveIncreaseDataEffects => _moveIncreaseDataEffects;
        internal IReadOnlyDictionary<MoveMultiplierDataEffect, int> MoveMultiplierDataEffects => _moveMultiplierDataEffects;

        public IReadOnlyDictionary<ATKAdditionalDataEffect, (int level, string displayName)> ATKAdditionalDataEffects => _atkAdditionalDataEffects;
        public IReadOnlyDictionary<ATKIncreaseDataEffect, (int level, string displayName)> ATKIncreaseDataEffects => _atkIncreaseDataEffects;
        public IReadOnlyDictionary<ATKMultiplierDataEffect, (int level, string displayName)> ATKMultiplierDataEffects => _atkMultiplierDataEffects;

        internal IReadOnlyDictionary<AttackCountAdditionalDataEffect, int> AttackCountAdditionalDataEffects => _attackCountAdditionalDataEffects;

        public IReadOnlyDictionary<AttackSpeedIncreaseDataEffect, (int level, string displayName)> AttackSpeedIncreaseDataEffects => _attackSpeedIncreaseDataEffects;
        public IReadOnlyDictionary<AttackSpeedMultiplierDataEffect, (int level, string displayName)> AttackSpeedMultiplierDataEffects => _attackSpeedMultiplierDataEffects;

        internal IReadOnlyDictionary<AvoidanceAdditionalDataEffect, int> AvoidanceAdditionalDataEffects => _avoidanceAdditionalDataEffects;

        internal IReadOnlyDictionary<PhysicalPenetrationAdditionalDataEffect, int> PhysicalPenetrationAdditionalDataEffects => _physicalPenetrationAdditionalDataEffects;
        internal IReadOnlyDictionary<PhysicalPenetrationIncreaseDataEffect, int> PhysicalPenetrationIncreaseDataEffects => _physicalPenetrationIncreaseDataEffects;
        internal IReadOnlyDictionary<PhysicalPenetrationMultiplierDataEffect, int> PhysicalPenetrationMultiplierDataEffects => _physicalPenetrationMultiplierDataEffects;

        public IReadOnlyDictionary<PhysicalResistanceAdditionalDataEffect, (int level, string displayName)> PhysicalResistanceAdditionalDataEffects => _physicalResistanceAdditionalDataEffects;
        public IReadOnlyDictionary<PhysicalResistanceIncreaseDataEffect, (int level, string displayName)> PhysicalResistanceIncreaseDataEffects => _physicalResistanceIncreaseDataEffects;
        public IReadOnlyDictionary<PhysicalResistanceMultiplierDataEffect, (int level, string displayName)> PhysicalResistanceMultiplierDataEffects => _physicalResistanceMultiplierDataEffects;

        internal IReadOnlyDictionary<MagicPenetrationAdditionalDataEffect, int> MagicPenetrationAdditionalDataEffects => _magicPenetrationAdditionalDataEffects;
        internal IReadOnlyDictionary<MagicPenetrationIncreaseDataEffect, int> MagicPenetrationIncreaseDataEffects => _magicPenetrationIncreaseDataEffects;
        internal IReadOnlyDictionary<MagicPenetrationMultiplierDataEffect, int> MagicPenetrationMultiplierDataEffects => _magicPenetrationMultiplierDataEffects;

        public IReadOnlyDictionary<MagicResistanceAdditionalDataEffect, (int level, string displayName)> MagicResistanceAdditionalDataEffects => _magicResistanceAdditionalDataEffects;
        public IReadOnlyDictionary<MagicResistanceIncreaseDataEffect, (int level, string displayName)> MagicResistanceIncreaseDataEffects => _magicResistanceIncreaseDataEffects;
        public IReadOnlyDictionary<MagicResistanceMultiplierDataEffect, (int level, string displayName)> MagicResistanceMultiplierDataEffects => _magicResistanceMultiplierDataEffects;

        internal IReadOnlyDictionary<DamageAdditionalDataEffect, int> DamageAdditionalDataEffects => _damageAdditionalDataEffects;
        internal IReadOnlyDictionary<DamageIncreaseDataEffect, int> DamageIncreaseDataEffects => _damageIncreaseDataEffects;
        internal IReadOnlyDictionary<DamageMultiplierDataEffect, int> DamageMultiplierDataEffects => _damageMultiplierDataEffects;

        internal IReadOnlyDictionary<ReceiveDamageAdditionalDataEffect, int> ReceiveDamageAdditionalDataEffects => _receiveDamageAdditionalDataEffects;
        internal IReadOnlyDictionary<ReceiveDamageIncreaseDataEffect, int> ReceiveDamageIncreaseDataEffects => _receiveDamageIncreaseDataEffects;
        internal IReadOnlyDictionary<ReceiveDamageMultiplierDataEffect, int> ReceiveDamageMultiplierDataEffects => _receiveDamageMultiplierDataEffects;

        internal IReadOnlyDictionary<CriticalHitChanceAdditionalDataEffect, int> CriticalHitChanceAdditionalDataEffects => _criticalHitChanceAdditionalDataEffects;
        internal IReadOnlyDictionary<CriticalHitDamageAdditionalDataEffect, int> CriticalHitDamageAdditionalDataEffects => _criticalHitDamageAdditionalDataEffects;
        internal IReadOnlyDictionary<CriticalHitDamageIncreaseDataEffect, int> CriticalHitDamageIncreaseDataEffects => _criticalHitDamageIncreaseDataEffects;
        internal IReadOnlyDictionary<CriticalHitDamageMultiplierDataEffect, int> CriticalHitDamageMultiplierDataEffects => _criticalHitDamageMultiplierDataEffects;

        internal IReadOnlyDictionary<MaxHPAdditionalDataEffect, int> MaxHPAdditionalDataEffects => _maxHPAdditionalDataEffects;
        internal IReadOnlyDictionary<MaxHPIncreaseDataEffect, int> MaxHPIncreaseDataEffects => _maxHPIncreaseDataEffects;
        internal IReadOnlyDictionary<MaxHPMultiplierDataEffect, int> MaxHPMultiplierDataEffects => _maxHPMultiplierDataEffects;

        internal IReadOnlyDictionary<HealingAdditionalDataEffect, int> HealingAdditionalDataEffects => _healingAdditionalDataEffects;
        internal IReadOnlyDictionary<HealingIncreaseDataEffect, int> HealingIncreaseDataEffects => _healingIncreaseDataEffects;
        internal IReadOnlyDictionary<HealingMultiplierDataEffect, int> HealingMultiplierDataEffects => _healingMultiplierDataEffects;

        internal IReadOnlyDictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int> HPRecoveryPerSecByMaxHPIncreaseDataEffects => _hpRecoveryPerSecByMaxHPIncreaseDataEffects;
        internal IReadOnlyDictionary<AbnormalStatusResistanceAdditionalDataEffect, int> AbnormalStatusResistanceAdditionalDataEffects => _abnormalStatusResistanceAdditionalDataEffects;

        public IReadOnlyDictionary<SkillCooldownIncreaseDataEffect, (int level, string displayName)> SkillCooldownIncreaseDataEffects => _skillCooldownIncreaseDataEffects;

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

        public void ApplyBuff(BuffTemplate template, float duration, int level = 1)
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
                StartCoroutine(CoAddStatus(template, duration, isContained, level));
            }
            else
            {
                AddStatus(template, duration, isContained, level);
            }
        }

        private IEnumerator CoAddStatus(BuffTemplate template, float duration, bool isContained, int level)
        {
            yield return new WaitForSeconds(template.delay);
            AddStatus(template, duration, isContained, level);
        }

        /// <summary>
        /// 버프 추가
        /// </summary>
        private void AddStatus(BuffTemplate template, float duration, bool isContained, int level)
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
                        _moveIncreaseDataEffects.Add(moveIncreaseDataEffect, level);
                    }
                    else if (effect is MoveMultiplierDataEffect moveMultiplierDataEffect)
                    {
                        _moveMultiplierDataEffects.Add(moveMultiplierDataEffect, level);
                    }

                    else if (effect is ATKAdditionalDataEffect atkAdditionalDataEffects)
                    {
                        _atkAdditionalDataEffects.Add(atkAdditionalDataEffects, (level, template.displayName));
                    }
                    else if (effect is ATKIncreaseDataEffect atkIncreaseDataEffect)
                    {
                        _atkIncreaseDataEffects.Add(atkIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is ATKMultiplierDataEffect atkMultiplierDataEffect)
                    {
                        _atkMultiplierDataEffects.Add(atkMultiplierDataEffect, (level, template.displayName));
                    }

                    else if (effect is AttackCountAdditionalDataEffect attackCountAdditionalDataEffect)
                    {
                        _attackCountAdditionalDataEffects.Add(attackCountAdditionalDataEffect, level);
                    }

                    else if (effect is AttackSpeedIncreaseDataEffect AttackSpeedIncreaseDataEffect)
                    {
                        _attackSpeedIncreaseDataEffects.Add(AttackSpeedIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is AttackSpeedMultiplierDataEffect AttackSpeedMultiplierDataEffect)
                    {
                        _attackSpeedMultiplierDataEffects.Add(AttackSpeedMultiplierDataEffect, (level, template.displayName));
                    }

                    else if (effect is AvoidanceAdditionalDataEffect AvoidanceAdditionalDataEffect)
                    {
                        _avoidanceAdditionalDataEffects.Add(AvoidanceAdditionalDataEffect, level);
                    }

                    else if (effect is PhysicalPenetrationAdditionalDataEffect physicalPenetrationAdditionalDataEffect)
                    {
                        _physicalPenetrationAdditionalDataEffects.Add(physicalPenetrationAdditionalDataEffect, level);
                    }
                    else if (effect is PhysicalPenetrationIncreaseDataEffect physicalPenetrationIncreaseDataEffect)
                    {
                        _physicalPenetrationIncreaseDataEffects.Add(physicalPenetrationIncreaseDataEffect, level);
                    }
                    else if (effect is PhysicalPenetrationMultiplierDataEffect physicalPenetrationMultiplierDataEffect)
                    {
                        _physicalPenetrationMultiplierDataEffects.Add(physicalPenetrationMultiplierDataEffect, level);
                    }

                    else if (effect is PhysicalResistanceAdditionalDataEffect physicalResistanceAdditionalDataEffect)
                    {
                        _physicalResistanceAdditionalDataEffects.Add(physicalResistanceAdditionalDataEffect, (level, template.displayName));
                    }
                    else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                    {
                        _physicalResistanceIncreaseDataEffects.Add(physicalResistanceIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is PhysicalResistanceMultiplierDataEffect physicalResistanceMultiplierDataEffect)
                    {
                        _physicalResistanceMultiplierDataEffects.Add(physicalResistanceMultiplierDataEffect, (level, template.displayName));
                    }

                    else if (effect is MagicPenetrationAdditionalDataEffect magicPenetrationAdditionalDataEffect)
                    {
                        _magicPenetrationAdditionalDataEffects.Add(magicPenetrationAdditionalDataEffect, level);
                    }
                    else if (effect is MagicPenetrationIncreaseDataEffect magicPenetrationIncreaseDataEffect)
                    {
                        _magicPenetrationIncreaseDataEffects.Add(magicPenetrationIncreaseDataEffect, level);
                    }
                    else if (effect is MagicPenetrationMultiplierDataEffect magicPenetrationMultiplierDataEffect)
                    {
                        _magicPenetrationMultiplierDataEffects.Add(magicPenetrationMultiplierDataEffect, level);
                    }

                    else if (effect is MagicResistanceAdditionalDataEffect magicResistanceAdditionalDataEffect)
                    {
                        _magicResistanceAdditionalDataEffects.Add(magicResistanceAdditionalDataEffect, (level, template.displayName));
                    }
                    else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                    {
                        _magicResistanceIncreaseDataEffects.Add(magicResistanceIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is MagicResistanceMultiplierDataEffect magicResistanceMultiplierDataEffect)
                    {
                        _magicResistanceMultiplierDataEffects.Add(magicResistanceMultiplierDataEffect, (level, template.displayName));
                    }

                    else if (effect is DamageAdditionalDataEffect damageAdditionalDataEffect)
                    {
                        _damageAdditionalDataEffects.Add(damageAdditionalDataEffect, level);
                    }
                    else if (effect is DamageIncreaseDataEffect damageIncreaseDataEffect)
                    {
                        _damageIncreaseDataEffects.Add(damageIncreaseDataEffect, level);
                    }
                    else if (effect is DamageMultiplierDataEffect damageMultiplierDataEffect)
                    {
                        _damageMultiplierDataEffects.Add(damageMultiplierDataEffect, level);
                    }

                    else if (effect is ReceiveDamageAdditionalDataEffect receiveDamageAdditionalDataEffect)
                    {
                        _receiveDamageAdditionalDataEffects.Add(receiveDamageAdditionalDataEffect, level);
                    }
                    else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                    {
                        _receiveDamageIncreaseDataEffects.Add(receiveDamageIncreaseDataEffect, level);
                    }
                    else if (effect is ReceiveDamageMultiplierDataEffect receiveDamageMultiplierDataEffect)
                    {
                        _receiveDamageMultiplierDataEffects.Add(receiveDamageMultiplierDataEffect, level);
                    }

                    else if (effect is CriticalHitChanceAdditionalDataEffect criticalHitChanceAdditionalDataEffect)
                    {
                        _criticalHitChanceAdditionalDataEffects.Add(criticalHitChanceAdditionalDataEffect, level);
                    }
                    else if (effect is CriticalHitDamageAdditionalDataEffect criticalHitDamageAdditionalDataEffect)
                    {
                        _criticalHitDamageAdditionalDataEffects.Add(criticalHitDamageAdditionalDataEffect, level);
                    }
                    else if (effect is CriticalHitDamageIncreaseDataEffect criticalHitDamageIncreaseDataEffect)
                    {
                        _criticalHitDamageIncreaseDataEffects.Add(criticalHitDamageIncreaseDataEffect, level);
                    }
                    else if (effect is CriticalHitDamageMultiplierDataEffect criticalHitDamageMultiplierDataEffect)
                    {
                        _criticalHitDamageMultiplierDataEffects.Add(criticalHitDamageMultiplierDataEffect, level);
                    }

                    else if (effect is MaxHPAdditionalDataEffect maxHPAdditionalDataEffect)
                    {
                        _maxHPAdditionalDataEffects.Add(maxHPAdditionalDataEffect, level);
                    }
                    else if (effect is MaxHPIncreaseDataEffect maxHPIncreaseDataEffect)
                    {
                        _maxHPIncreaseDataEffects.Add(maxHPIncreaseDataEffect, level);
                    }
                    else if (effect is MaxHPMultiplierDataEffect maxHPMultiplierDataEffect)
                    {
                        _maxHPMultiplierDataEffects.Add(maxHPMultiplierDataEffect, level);
                    }

                    else if (effect is HealingAdditionalDataEffect healingAdditionalDataEffect)
                    {
                        _healingAdditionalDataEffects.Add(healingAdditionalDataEffect, level);
                    }
                    else if (effect is HealingIncreaseDataEffect healingIncreaseDataEffect)
                    {
                        _healingIncreaseDataEffects.Add(healingIncreaseDataEffect, level);
                    }
                    else if (effect is HealingMultiplierDataEffect healingMultiplierDataEffect)
                    {
                        _healingMultiplierDataEffects.Add(healingMultiplierDataEffect, level);
                    }

                    else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                    {
                        _hpRecoveryPerSecByMaxHPIncreaseDataEffects.Add(hpRecoveryPerSecByMaxHPIncreaseDataEffect, level);
                    }
                    else if (effect is AbnormalStatusResistanceAdditionalDataEffect abnormalStatusResistanceAdditionalDataEffect)
                    {
                        _abnormalStatusResistanceAdditionalDataEffects.Add(abnormalStatusResistanceAdditionalDataEffect, level);
                    }

                    else if (effect is SkillCooldownIncreaseDataEffect skillCooldownIncreaseDataEffect)
                    {
                        _skillCooldownIncreaseDataEffects.Add(skillCooldownIncreaseDataEffect, (level, template.displayName));
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

        public void RemoveBuff(BuffTemplate template)
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

                else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                {
                    _hpRecoveryPerSecByMaxHPIncreaseDataEffects.Remove(hpRecoveryPerSecByMaxHPIncreaseDataEffect);
                }
                else if (effect is AbnormalStatusResistanceAdditionalDataEffect abnormalStatusResistanceAdditionalDataEffect)
                {
                    _abnormalStatusResistanceAdditionalDataEffects.Remove(abnormalStatusResistanceAdditionalDataEffect);
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