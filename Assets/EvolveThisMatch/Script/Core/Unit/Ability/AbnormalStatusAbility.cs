using FrameWork.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusAbility : AlwaysAbility
    {
        #region Effect List
        private List<UnableToMoveEffect> _unableToMoveEffects = new List<UnableToMoveEffect>();
        private List<UnableToAttackEffect> _unableToAttackEffects = new List<UnableToAttackEffect>();
        private List<UnableToHealEffect> _unableToHealEffects = new List<UnableToHealEffect>();
        private List<UnableToSkillEffect> _unableToSkillEffects = new List<UnableToSkillEffect>();
        private Dictionary<MoveIncreaseDataEffect, int> _moveIncreaseDataEffects = new Dictionary<MoveIncreaseDataEffect, int>();
        private Dictionary<PhysicalResistanceIncreaseDataEffect, (int level, string displayName)> _physicalResistanceIncreaseDataEffects = new Dictionary<PhysicalResistanceIncreaseDataEffect, (int level, string displayName)>();
        private Dictionary<MagicResistanceIncreaseDataEffect, (int level, string displayName)> _magicResistanceIncreaseDataEffects = new Dictionary<MagicResistanceIncreaseDataEffect, (int level, string displayName)>();
        private Dictionary<ReceiveDamageIncreaseDataEffect, int> _receiveDamageIncreaseDataEffects = new Dictionary<ReceiveDamageIncreaseDataEffect, int>();
        private Dictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int> _hpRecoveryPerSecByMaxHPIncreaseDataEffects = new Dictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int>();

        #region 프로퍼티
        internal IReadOnlyList<UnableToMoveEffect> UnableToMoveEffects => _unableToMoveEffects;
        internal IReadOnlyList<UnableToAttackEffect> UnableToAttackEffects => _unableToAttackEffects;
        internal IReadOnlyList<UnableToHealEffect> UnableToHealEffects => _unableToHealEffects;
        internal IReadOnlyList<UnableToSkillEffect> UnableToSkillEffects => _unableToSkillEffects;
        internal IReadOnlyDictionary<MoveIncreaseDataEffect, int> MoveIncreaseDataEffects => _moveIncreaseDataEffects;
        public IReadOnlyDictionary<PhysicalResistanceIncreaseDataEffect, (int level, string displayName)> PhysicalResistanceIncreaseDataEffects => _physicalResistanceIncreaseDataEffects;
        public IReadOnlyDictionary<MagicResistanceIncreaseDataEffect, (int level, string displayName)> MagicResistanceIncreaseDataEffects => _magicResistanceIncreaseDataEffects;
        internal IReadOnlyDictionary<ReceiveDamageIncreaseDataEffect, int> ReceiveDamageIncreaseDataEffects => _receiveDamageIncreaseDataEffects;
        internal IReadOnlyDictionary<HPRecoveryPerSecByMaxHPIncreaseDataEffect, int> HPRecoveryPerSecByMaxHPIncreaseDataEffects => _hpRecoveryPerSecByMaxHPIncreaseDataEffects;
        #endregion

        #endregion

        #region 스탯 계산
        public float finalAbnormalStatusResistance
        {
            get
            {
                float result = 0;

                #region 추가·차감
                foreach (var effect in _buffAbility.AbnormalStatusResistanceAdditionalDataEffects)
                {
                    result += effect.Key.GetValue(effect.Value);
                }
                #endregion

                return result;
            }
        }
        #endregion

        private BuffAbility _buffAbility;

        private Dictionary<AbnormalStatusTemplate, StatusInstance> statusDic = new Dictionary<AbnormalStatusTemplate, StatusInstance>();

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private List<AbnormalStatusTemplate> statusList = new List<AbnormalStatusTemplate>();
#endif

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _buffAbility = unit.GetAbility<BuffAbility>();

            unit.GetAbility<HitAbility>().onHit += RemoveStatusByHit;
            unit.GetAbility<HealthAbility>().onDeath += ClearStatusEffects;
        }

        internal void DeInitialize()
        {
            unit.GetAbility<HitAbility>().onHit -= RemoveStatusByHit;
            unit.GetAbility<HealthAbility>().onDeath -= ClearStatusEffects;
        }

        internal void ApplyAbnormalStatus(AbnormalStatusTemplate template, float duration, int level = 1)
        {
            if (this == null || gameObject == null) return;

            if (Random.value < finalAbnormalStatusResistance)
            {
                return;
            }

            var isContained = false;

            if (statusDic.ContainsKey(template))
            {
                isContained = true;

                var instance = statusDic[template];
                if (instance.IsOld(duration))
                {
                    instance.duration = duration;
                    instance.startTime = Time.time;

                    if (template.useHitCountLimit)
                    {
                        instance.useCountLimit = true;
                        instance.count = template.hitCount;
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

        private IEnumerator CoAddStatus(AbnormalStatusTemplate template, float duration, bool isContained, int level)
        {
            yield return new WaitForSeconds(template.delay);
            AddStatus(template, duration, isContained, level);
        }

        /// <summary>
        /// 상태이상 추가
        /// </summary>
        private void AddStatus(AbnormalStatusTemplate template, float duration, bool isContained, int level)
        {
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);
            
            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }
            
            // 피격시 상태이상이 해제되야 한다면
            if (template.useHitCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.hitCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            // 상태이상 효과 적용 (동일한 상태이상 효과는 중복되지 않음)
            if (isContained == false)
            {
                ExecuteApplyFX(template);

                foreach (var effect in template.effects)
                {
                    if (effect is UnableToMoveEffect unableToMoveEffect)
                    {
                        _unableToMoveEffects.Add(unableToMoveEffect);
                    }
                    else if (effect is UnableToAttackEffect unableToAttackEffect)
                    {
                        _unableToAttackEffects.Add(unableToAttackEffect);
                    }
                    else if (effect is UnableToHealEffect unableToHealEffects)
                    {
                        _unableToHealEffects.Add(unableToHealEffects);
                    }
                    else if (effect is UnableToSkillEffect unableToSkillEffect)
                    {
                        _unableToSkillEffects.Add(unableToSkillEffect);
                    }
                    else if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                    {
                        _moveIncreaseDataEffects.Add(moveIncreaseDataEffect, level);
                    }
                    else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                    {
                        _physicalResistanceIncreaseDataEffects.Add(physicalResistanceIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                    {
                        _magicResistanceIncreaseDataEffects.Add(magicResistanceIncreaseDataEffect, (level, template.displayName));
                    }
                    else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                    {
                        _receiveDamageIncreaseDataEffects.Add(receiveDamageIncreaseDataEffect, level);
                    }
                    else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                    {
                        _hpRecoveryPerSecByMaxHPIncreaseDataEffects.Add(hpRecoveryPerSecByMaxHPIncreaseDataEffect, level);
                    }
                }
            }
        }

        private IEnumerator CoStatus(StatusInstance statusInstance, AbnormalStatusTemplate template)
        {
            while (statusInstance.IsCompete == false)
            {
                yield return null;
            }

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

        #region 콜백 메서드
        private void RemoveStatusByHit()
        {
            List<AbnormalStatusTemplate> templates = new List<AbnormalStatusTemplate>();
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

        /// <summary>
        /// 상태이상 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                if (effect is UnableToMoveEffect unableToMoveEffect)
                {
                    _unableToMoveEffects.Remove(unableToMoveEffect);
                }
                else if (effect is UnableToAttackEffect unableToAttackEffect)
                {
                    _unableToAttackEffects.Remove(unableToAttackEffect);
                }
                else if (effect is UnableToHealEffect unableToHealEffects)
                {
                    _unableToHealEffects.Remove(unableToHealEffects);
                }
                else if (effect is UnableToSkillEffect unableToSkillEffect)
                {
                    _unableToSkillEffects.Remove(unableToSkillEffect);
                }
                else if (effect is MoveIncreaseDataEffect moveIncreaseDataEffect)
                {
                    _moveIncreaseDataEffects.Remove(moveIncreaseDataEffect);
                }
                else if (effect is PhysicalResistanceIncreaseDataEffect physicalResistanceIncreaseDataEffect)
                {
                    _physicalResistanceIncreaseDataEffects.Remove(physicalResistanceIncreaseDataEffect);
                }
                else if (effect is MagicResistanceIncreaseDataEffect magicResistanceIncreaseDataEffect)
                {
                    _magicResistanceIncreaseDataEffects.Remove(magicResistanceIncreaseDataEffect);
                }
                else if (effect is ReceiveDamageIncreaseDataEffect receiveDamageIncreaseDataEffect)
                {
                    _receiveDamageIncreaseDataEffects.Remove(receiveDamageIncreaseDataEffect);
                }
                else if (effect is HPRecoveryPerSecByMaxHPIncreaseDataEffect hpRecoveryPerSecByMaxHPIncreaseDataEffect)
                {
                    _hpRecoveryPerSecByMaxHPIncreaseDataEffects.Remove(hpRecoveryPerSecByMaxHPIncreaseDataEffect);
                }
            }
        }

        #region 유틸리티 메서드
        internal bool Contains(AbnormalStatusTemplate template)
        {
            return statusDic.ContainsKey(template);
        }

        internal bool Contains(List<AbnormalStatusTemplate> templates)
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
        private void ExecuteApplyFX(AbnormalStatusTemplate template)
        {
            if (template.applyFX != null)
            {
                template.applyFX.Play(unit);
            }
        }

        private void ExecuteRemoveFX(AbnormalStatusTemplate template)
        {
            if (template.removeFX != null)
            {
                template.removeFX.Play(unit);
            }
        }
        #endregion
    }
}