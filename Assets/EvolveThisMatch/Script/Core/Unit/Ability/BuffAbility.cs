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
        #region Data
        // Move
        private List<DataEffectInstance<MoveIncreaseDataEffect>> _moveIncreaseDataEffects = new List<DataEffectInstance<MoveIncreaseDataEffect>>();
        private List<DataEffectInstance<MoveMultiplierDataEffect>> _moveMultiplierDataEffects = new List<DataEffectInstance<MoveMultiplierDataEffect>>();

        // Attack
        private List<StatDataEffectInstance<ATKAdditionalDataEffect>> _atkAdditionalDataEffects = new List<StatDataEffectInstance<ATKAdditionalDataEffect>>();
        private List<StatDataEffectInstance<ATKIncreaseDataEffect>> _atkIncreaseDataEffects = new List<StatDataEffectInstance<ATKIncreaseDataEffect>>();
        private List<StatDataEffectInstance<ATKMultiplierDataEffect>> _atkMultiplierDataEffects = new List<StatDataEffectInstance<ATKMultiplierDataEffect>>();

        private List<DataEffectInstance<AttackCountAdditionalDataEffect>> _attackCountAdditionalDataEffects = new List<DataEffectInstance<AttackCountAdditionalDataEffect>>();

        private List<StatDataEffectInstance<AttackSpeedIncreaseDataEffect>> _attackSpeedIncreaseDataEffects = new List<StatDataEffectInstance<AttackSpeedIncreaseDataEffect>>();
        private List<StatDataEffectInstance<AttackSpeedMultiplierDataEffect>> _attackSpeedMultiplierDataEffects = new List<StatDataEffectInstance<AttackSpeedMultiplierDataEffect>>();

        // Avoidance
        private List<DataEffectInstance<AvoidanceAdditionalDataEffect>> _avoidanceAdditionalDataEffects = new List<DataEffectInstance<AvoidanceAdditionalDataEffect>>();

        // Physical Penetration
        private List<DataEffectInstance<PhysicalPenetrationAdditionalDataEffect>> _physicalPenetrationAdditionalDataEffects = new List<DataEffectInstance<PhysicalPenetrationAdditionalDataEffect>>();
        private List<DataEffectInstance<PhysicalPenetrationIncreaseDataEffect>> _physicalPenetrationIncreaseDataEffects = new List<DataEffectInstance<PhysicalPenetrationIncreaseDataEffect>>();
        private List<DataEffectInstance<PhysicalPenetrationMultiplierDataEffect>> _physicalPenetrationMultiplierDataEffects = new List<DataEffectInstance<PhysicalPenetrationMultiplierDataEffect>>();

        // Physical Resistance
        private List<StatDataEffectInstance<PhysicalResistanceAdditionalDataEffect>> _physicalResistanceAdditionalDataEffects = new List<StatDataEffectInstance<PhysicalResistanceAdditionalDataEffect>>();
        private List<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>> _physicalResistanceIncreaseDataEffects = new List<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>>();
        private List<StatDataEffectInstance<PhysicalResistanceMultiplierDataEffect>> _physicalResistanceMultiplierDataEffects = new List<StatDataEffectInstance<PhysicalResistanceMultiplierDataEffect>>();

        // Magic Penetration
        private List<DataEffectInstance<MagicPenetrationAdditionalDataEffect>> _magicPenetrationAdditionalDataEffects = new List<DataEffectInstance<MagicPenetrationAdditionalDataEffect>>();
        private List<DataEffectInstance<MagicPenetrationIncreaseDataEffect>> _magicPenetrationIncreaseDataEffects = new List<DataEffectInstance<MagicPenetrationIncreaseDataEffect>>();
        private List<DataEffectInstance<MagicPenetrationMultiplierDataEffect>> _magicPenetrationMultiplierDataEffects = new List<DataEffectInstance<MagicPenetrationMultiplierDataEffect>>();

        // Magic Resistance
        private List<StatDataEffectInstance<MagicResistanceAdditionalDataEffect>> _magicResistanceAdditionalDataEffects = new List<StatDataEffectInstance<MagicResistanceAdditionalDataEffect>>();
        private List<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>> _magicResistanceIncreaseDataEffects = new List<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>>();
        private List<StatDataEffectInstance<MagicResistanceMultiplierDataEffect>> _magicResistanceMultiplierDataEffects = new List<StatDataEffectInstance<MagicResistanceMultiplierDataEffect>>();

        // Damage
        private List<DataEffectInstance<DamageAdditionalDataEffect>> _damageAdditionalDataEffects = new List<DataEffectInstance<DamageAdditionalDataEffect>>();
        private List<DataEffectInstance<DamageIncreaseDataEffect>> _damageIncreaseDataEffects = new List<DataEffectInstance<DamageIncreaseDataEffect>>();
        private List<DataEffectInstance<DamageMultiplierDataEffect>> _damageMultiplierDataEffects = new List<DataEffectInstance<DamageMultiplierDataEffect>>();

        // Receive Damage
        private List<DataEffectInstance<ReceiveDamageAdditionalDataEffect>> _receiveDamageAdditionalDataEffects = new List<DataEffectInstance<ReceiveDamageAdditionalDataEffect>>();
        private List<DataEffectInstance<ReceiveDamageIncreaseDataEffect>> _receiveDamageIncreaseDataEffects = new List<DataEffectInstance<ReceiveDamageIncreaseDataEffect>>();
        private List<DataEffectInstance<ReceiveDamageMultiplierDataEffect>> _receiveDamageMultiplierDataEffects = new List<DataEffectInstance<ReceiveDamageMultiplierDataEffect>>();

        // Critical
        private List<DataEffectInstance<CriticalHitChanceAdditionalDataEffect>> _criticalHitChanceAdditionalDataEffects = new List<DataEffectInstance<CriticalHitChanceAdditionalDataEffect>>();
        private List<DataEffectInstance<CriticalHitDamageAdditionalDataEffect>> _criticalHitDamageAdditionalDataEffects = new List<DataEffectInstance<CriticalHitDamageAdditionalDataEffect>>();
        private List<DataEffectInstance<CriticalHitDamageIncreaseDataEffect>> _criticalHitDamageIncreaseDataEffects = new List<DataEffectInstance<CriticalHitDamageIncreaseDataEffect>>();
        private List<DataEffectInstance<CriticalHitDamageMultiplierDataEffect>> _criticalHitDamageMultiplierDataEffects = new List<DataEffectInstance<CriticalHitDamageMultiplierDataEffect>>();

        // Max HP
        private List<DataEffectInstance<MaxHPAdditionalDataEffect>> _maxHPAdditionalDataEffects = new List<DataEffectInstance<MaxHPAdditionalDataEffect>>();
        private List<DataEffectInstance<MaxHPIncreaseDataEffect>> _maxHPIncreaseDataEffects = new List<DataEffectInstance<MaxHPIncreaseDataEffect>>();
        private List<DataEffectInstance<MaxHPMultiplierDataEffect>> _maxHPMultiplierDataEffects = new List<DataEffectInstance<MaxHPMultiplierDataEffect>>();

        // Healing
        private List<DataEffectInstance<HealingAdditionalDataEffect>> _healingAdditionalDataEffects = new List<DataEffectInstance<HealingAdditionalDataEffect>>();
        private List<DataEffectInstance<HealingIncreaseDataEffect>> _healingIncreaseDataEffects = new List<DataEffectInstance<HealingIncreaseDataEffect>>();
        private List<DataEffectInstance<HealingMultiplierDataEffect>> _healingMultiplierDataEffects = new List<DataEffectInstance<HealingMultiplierDataEffect>>();

        // HP Recovery / Abnormal Status
        private List<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>> _hpRecoveryPerSecByMaxHPIncreaseDataEffects = new List<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>>();
        private List<DataEffectInstance<AbnormalStatusResistanceAdditionalDataEffect>> _abnormalStatusResistanceAdditionalDataEffects = new List<DataEffectInstance<AbnormalStatusResistanceAdditionalDataEffect>>();

        // Skill Cooldown
        private List<StatDataEffectInstance<SkillCooldownIncreaseDataEffect>> _skillCooldownIncreaseDataEffects = new List<StatDataEffectInstance<SkillCooldownIncreaseDataEffect>>();
        #endregion

        #region Set
        private List<SetMinHPEffect> _setMinHPEffects = new List<SetMinHPEffect>();
        private List<SetAttackTypeEffect> _setAttackTypeEffects = new List<SetAttackTypeEffect>();
        private List<SetDamageTypeEffect> _setDamageTypeEffects = new List<SetDamageTypeEffect>();
        #endregion

        #region Unable
        private List<UnableToTargetOfAttackEffect> _unableToTargetOfAttackEffects = new List<UnableToTargetOfAttackEffect>();
        #endregion
        #endregion

        #region 프로퍼티
        #region Data
        internal IReadOnlyList<DataEffectInstance<MoveIncreaseDataEffect>> MoveIncreaseDataEffects => _moveIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<MoveMultiplierDataEffect>> MoveMultiplierDataEffects => _moveMultiplierDataEffects;

        public IReadOnlyList<StatDataEffectInstance<ATKAdditionalDataEffect>> ATKAdditionalDataEffects => _atkAdditionalDataEffects;
        public IReadOnlyList<StatDataEffectInstance<ATKIncreaseDataEffect>> ATKIncreaseDataEffects => _atkIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<ATKMultiplierDataEffect>> ATKMultiplierDataEffects => _atkMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<AttackCountAdditionalDataEffect>> AttackCountAdditionalDataEffects => _attackCountAdditionalDataEffects;

        public IReadOnlyList<StatDataEffectInstance<AttackSpeedIncreaseDataEffect>> AttackSpeedIncreaseDataEffects => _attackSpeedIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<AttackSpeedMultiplierDataEffect>> AttackSpeedMultiplierDataEffects => _attackSpeedMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<AvoidanceAdditionalDataEffect>> AvoidanceAdditionalDataEffects => _avoidanceAdditionalDataEffects;

        internal IReadOnlyList<DataEffectInstance<PhysicalPenetrationAdditionalDataEffect>> PhysicalPenetrationAdditionalDataEffects => _physicalPenetrationAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<PhysicalPenetrationIncreaseDataEffect>> PhysicalPenetrationIncreaseDataEffects => _physicalPenetrationIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<PhysicalPenetrationMultiplierDataEffect>> PhysicalPenetrationMultiplierDataEffects => _physicalPenetrationMultiplierDataEffects;

        public IReadOnlyList<StatDataEffectInstance<PhysicalResistanceAdditionalDataEffect>> PhysicalResistanceAdditionalDataEffects => _physicalResistanceAdditionalDataEffects;
        public IReadOnlyList<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>> PhysicalResistanceIncreaseDataEffects => _physicalResistanceIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<PhysicalResistanceMultiplierDataEffect>> PhysicalResistanceMultiplierDataEffects => _physicalResistanceMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<MagicPenetrationAdditionalDataEffect>> MagicPenetrationAdditionalDataEffects => _magicPenetrationAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<MagicPenetrationIncreaseDataEffect>> MagicPenetrationIncreaseDataEffects => _magicPenetrationIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<MagicPenetrationMultiplierDataEffect>> MagicPenetrationMultiplierDataEffects => _magicPenetrationMultiplierDataEffects;

        public IReadOnlyList<StatDataEffectInstance<MagicResistanceAdditionalDataEffect>> MagicResistanceAdditionalDataEffects => _magicResistanceAdditionalDataEffects;
        public IReadOnlyList<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>> MagicResistanceIncreaseDataEffects => _magicResistanceIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<MagicResistanceMultiplierDataEffect>> MagicResistanceMultiplierDataEffects => _magicResistanceMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<DamageAdditionalDataEffect>> DamageAdditionalDataEffects => _damageAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<DamageIncreaseDataEffect>> DamageIncreaseDataEffects => _damageIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<DamageMultiplierDataEffect>> DamageMultiplierDataEffects => _damageMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<ReceiveDamageAdditionalDataEffect>> ReceiveDamageAdditionalDataEffects => _receiveDamageAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<ReceiveDamageIncreaseDataEffect>> ReceiveDamageIncreaseDataEffects => _receiveDamageIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<ReceiveDamageMultiplierDataEffect>> ReceiveDamageMultiplierDataEffects => _receiveDamageMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<CriticalHitChanceAdditionalDataEffect>> CriticalHitChanceAdditionalDataEffects => _criticalHitChanceAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<CriticalHitDamageAdditionalDataEffect>> CriticalHitDamageAdditionalDataEffects => _criticalHitDamageAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<CriticalHitDamageIncreaseDataEffect>> CriticalHitDamageIncreaseDataEffects => _criticalHitDamageIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<CriticalHitDamageMultiplierDataEffect>> CriticalHitDamageMultiplierDataEffects => _criticalHitDamageMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<MaxHPAdditionalDataEffect>> MaxHPAdditionalDataEffects => _maxHPAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<MaxHPIncreaseDataEffect>> MaxHPIncreaseDataEffects => _maxHPIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<MaxHPMultiplierDataEffect>> MaxHPMultiplierDataEffects => _maxHPMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<HealingAdditionalDataEffect>> HealingAdditionalDataEffects => _healingAdditionalDataEffects;
        internal IReadOnlyList<DataEffectInstance<HealingIncreaseDataEffect>> HealingIncreaseDataEffects => _healingIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<HealingMultiplierDataEffect>> HealingMultiplierDataEffects => _healingMultiplierDataEffects;

        internal IReadOnlyList<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>> HPRecoveryPerSecByMaxHPIncreaseDataEffects => _hpRecoveryPerSecByMaxHPIncreaseDataEffects;
        internal IReadOnlyList<DataEffectInstance<AbnormalStatusResistanceAdditionalDataEffect>> AbnormalStatusResistanceAdditionalDataEffects => _abnormalStatusResistanceAdditionalDataEffects;

        public IReadOnlyList<StatDataEffectInstance<SkillCooldownIncreaseDataEffect>> SkillCooldownIncreaseDataEffects => _skillCooldownIncreaseDataEffects;
        #endregion

        #region Set
        internal IReadOnlyList<SetMinHPEffect> SetMinHPEffects => _setMinHPEffects;
        internal IReadOnlyList<SetAttackTypeEffect> SetAttackTypeEffects => _setAttackTypeEffects;
        internal IReadOnlyList<SetDamageTypeEffect> SetDamageTypeEffects => _setDamageTypeEffects;
        #endregion

        #region Unable
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

        public void ApplyBuff(BuffTemplate template, float duration, EffectContext context)
        {
            if (this == null || gameObject == null || template == null) return;

            // 이미 포함되어 있다면
            if (statusDic.ContainsKey(template))
            {
                var instance = statusDic[template];

                // 더 효과가 길게 유지된다면 최신화
                if (instance.IsOld(duration))
                {
                    instance.duration = duration;
                    instance.startTime = Time.time;
                }

                // 공격 시, 버프가 해제되야 한다면 횟수 초기화
                if (template.useAttackCountLimit)
                {
                    instance.useCountLimit = true;
                    instance.count = template.attackCount;
                }

                return;
            }

            if (template.delay > 0)
                StartCoroutine(CoAddStatus(template, duration, context));
            else
                AddInstance(template, duration, context);
        }

        private IEnumerator CoAddStatus(BuffTemplate template, float duration, EffectContext context)
        {
            yield return new WaitForSeconds(template.delay);
            AddInstance(template, duration, context);
        }

        #region 버프 추가
        private void AddInstance(BuffTemplate template, float duration, EffectContext context)
        {
            // 포함되어 있지 않다면 생성
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);

            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }

            // 공격 시, 버프가 해제되야 한다면 횟수 초기화
            if (template.useAttackCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.attackCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            AddStatus(template, context);
        }

        /// <summary>
        /// 버프 추가
        /// </summary>
        private void AddStatus(BuffTemplate template, EffectContext context)
        {
            ExecuteApplyFX(template);
            string displayName = template.displayName;

            foreach (var effect in template.effects)
            {
                #region 이동
                if (AddDataEffect(effect, context, _moveIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _moveMultiplierDataEffects)) continue;
                #endregion

                #region 전투력
                if (AddDataEffect(effect, context, displayName, _atkAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _atkIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _atkMultiplierDataEffects)) continue;
                #endregion

                #region 공격 관련
                // 공격 횟수
                if (AddDataEffect(effect, context, _attackCountAdditionalDataEffects)) continue;
                // 공격 간격
                if (AddDataEffect(effect, context, displayName, _attackSpeedIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _attackSpeedMultiplierDataEffects)) continue;
                #endregion

                #region 회피
                if (AddDataEffect(effect, context, _avoidanceAdditionalDataEffects)) continue;
                #endregion

                #region 물리 관통력
                if (AddDataEffect(effect, context, _physicalPenetrationAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _physicalPenetrationIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _physicalPenetrationMultiplierDataEffects)) continue;
                #endregion

                #region 물리 저항력
                if (AddDataEffect(effect, context, displayName, _physicalResistanceAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _physicalResistanceIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _physicalResistanceMultiplierDataEffects)) continue;
                #endregion

                #region 마법 관통력
                if (AddDataEffect(effect, context, _magicPenetrationAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _magicPenetrationIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _magicPenetrationMultiplierDataEffects)) continue;
                #endregion

                #region 마법 저항력
                if (AddDataEffect(effect, context, displayName, _magicResistanceAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _magicResistanceIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _magicResistanceMultiplierDataEffects)) continue;
                #endregion

                #region 피해량
                if (AddDataEffect(effect, context, _damageAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _damageIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _damageMultiplierDataEffects)) continue;
                #endregion

                #region 받는 피해량
                if (AddDataEffect(effect, context, _receiveDamageAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _receiveDamageIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _receiveDamageMultiplierDataEffects)) continue;
                #endregion

                #region 치명타
                // 치명타 확률
                if (AddDataEffect(effect, context, _criticalHitChanceAdditionalDataEffects)) continue;

                // 치명타 데미지
                if (AddDataEffect(effect, context, _criticalHitDamageAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _criticalHitDamageIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _criticalHitDamageMultiplierDataEffects)) continue;
                #endregion

                #region 최대 체력
                if (AddDataEffect(effect, context, _maxHPAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _maxHPIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _maxHPMultiplierDataEffects)) continue;
                #endregion

                #region 회복
                // 회복량
                if (AddDataEffect(effect, context, _healingAdditionalDataEffects)) continue;
                if (AddDataEffect(effect, context, _healingIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _healingMultiplierDataEffects)) continue;

                // 최대 체력 비례 초당 체력 회복량
                if (AddDataEffect(effect, context, _hpRecoveryPerSecByMaxHPIncreaseDataEffects)) continue;
                #endregion

                #region 상태이상 저항력
                if (AddDataEffect(effect, context, _abnormalStatusResistanceAdditionalDataEffects)) continue;
                #endregion

                #region 스킬 가속
                if (AddDataEffect(effect, context, displayName, _skillCooldownIncreaseDataEffects)) continue;
                #endregion

                #region Set
                if (AddDataEffect(effect, _setMinHPEffects)) continue;
                if (AddDataEffect(effect, _setAttackTypeEffects)) continue;
                if (AddDataEffect(effect, _setDamageTypeEffects)) continue;
                #endregion

                #region Unable
                if (AddDataEffect(effect, _unableToTargetOfAttackEffects)) continue;
                #endregion
            }
        }
        #endregion

        #region 버프 유지시간 관리
        private IEnumerator CoStatus(StatusInstance statusInstance, BuffTemplate template)
        {
            while (statusInstance.IsCompete == false)
            {
                yield return null;
            }

            RemoveBuff(template);
        }
        #endregion

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
        #endregion

        #region 버프 제거
        /// <summary>
        /// 모든 버프 제거
        /// </summary>
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

        /// <summary>
        /// 특정 버프 제거
        /// </summary>
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
        /// 버프 효과 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                #region 이동
                if (RemoveDataEffect(effect, _moveIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _moveMultiplierDataEffects)) continue;
                #endregion

                #region 전투력
                if (RemoveDataEffect(effect, _atkAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _atkIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _atkMultiplierDataEffects)) continue;
                #endregion

                #region 공격 관련
                if (RemoveDataEffect(effect, _attackCountAdditionalDataEffects)) continue;

                if (RemoveDataEffect(effect, _attackSpeedIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _attackSpeedMultiplierDataEffects)) continue;
                #endregion

                #region 회피
                if (RemoveDataEffect(effect, _avoidanceAdditionalDataEffects)) continue;
                #endregion

                #region 물리 관통력
                if (RemoveDataEffect(effect, _physicalPenetrationAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _physicalPenetrationIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _physicalPenetrationMultiplierDataEffects)) continue;
                #endregion

                #region 물리 저항력
                if (RemoveDataEffect(effect, _physicalResistanceAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _physicalResistanceIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _physicalResistanceMultiplierDataEffects)) continue;
                #endregion

                #region 마법 관통력
                if (RemoveDataEffect(effect, _magicPenetrationAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _magicPenetrationIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _magicPenetrationMultiplierDataEffects)) continue;
                #endregion

                #region 마법 저항력
                if (RemoveDataEffect(effect, _magicResistanceAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _magicResistanceIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _magicResistanceMultiplierDataEffects)) continue;
                #endregion

                #region 피해량
                if (RemoveDataEffect(effect, _damageAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _damageIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _damageMultiplierDataEffects)) continue;
                #endregion

                #region 받는 피해량
                if (RemoveDataEffect(effect, _receiveDamageAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _receiveDamageIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _receiveDamageMultiplierDataEffects)) continue;
                #endregion

                #region 치명타
                // 치명타 확률
                if (RemoveDataEffect(effect, _criticalHitChanceAdditionalDataEffects)) continue;

                // 치명타 데미지
                if (RemoveDataEffect(effect, _criticalHitDamageAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _criticalHitDamageIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _criticalHitDamageMultiplierDataEffects)) continue;
                #endregion

                #region 최대 체력
                if (RemoveDataEffect(effect, _maxHPAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _maxHPIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _maxHPMultiplierDataEffects)) continue;
                #endregion

                #region 회복
                // 회복량
                if (RemoveDataEffect(effect, _healingAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _healingIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _healingMultiplierDataEffects)) continue;

                // 최대 체력 비례 초당 체력 회복량
                if (RemoveDataEffect(effect, _hpRecoveryPerSecByMaxHPIncreaseDataEffects)) continue;
                #endregion

                #region 상태이상 저항력
                if (RemoveDataEffect(effect, _abnormalStatusResistanceAdditionalDataEffects)) continue;
                #endregion

                #region 스킬 가속
                if (RemoveDataEffect(effect, _skillCooldownIncreaseDataEffects)) continue;
                #endregion

                #region Set
                if (RemoveDataEffect(effect, _setMinHPEffects)) continue;
                if (RemoveDataEffect(effect, _setAttackTypeEffects)) continue;
                if (RemoveDataEffect(effect, _setDamageTypeEffects)) continue;
                #endregion

                #region Unable
                if (RemoveDataEffect(effect, _unableToTargetOfAttackEffects)) continue;
                #endregion
            }
        }
        #endregion

        #region 유틸리티 메서드
        #region 이펙트 추가
        private bool AddDataEffect<T>(Effect effect, List<T> list) where T : Effect
        {
            if (effect is T typed)
            {
                list.Add(typed);
                return true;
            }
            return false;
        }

        private bool AddDataEffect<T>(Effect effect, EffectContext context, List<DataEffectInstance<T>> list) where T : DataEffectBase
        {
            if (effect is T typed)
            {
                list.Add(new DataEffectInstance<T>(typed, context));
                return true;
            }
            return false;
        }

        private bool AddDataEffect<T>(Effect effect, EffectContext context, string displayName, List<StatDataEffectInstance<T>> list) where T : DataEffectBase
        {
            if (effect is T typed)
            {
                list.Add(new StatDataEffectInstance<T>(typed, context, displayName));
                return true;
            }
            return false;
        }
        #endregion

        #region 이펙트 제거
        private bool RemoveDataEffect<T>(Effect effect, List<T> list) where T : Effect
        {
            if (effect is not T typed) return false;

            list.Remove(typed);
            return true;
        }

        private bool RemoveDataEffect<T>(Effect effect, List<DataEffectInstance<T>> list) where T : DataEffectBase
        {
            if (effect is not T typed) return false;

            list.RemoveAll(x => x.effect == typed);
            return true;
        }

        private bool RemoveDataEffect<T>(Effect effect, List<StatDataEffectInstance<T>> list) where T : DataEffectBase
        {
            if (effect is not T typed) return false;

            list.RemoveAll(x => x.effect == typed);
            return true;
        }
        #endregion

        #region 버프 템플릿 포함 여부
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