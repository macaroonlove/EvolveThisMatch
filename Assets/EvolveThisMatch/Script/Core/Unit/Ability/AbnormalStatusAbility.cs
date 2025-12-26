using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusAbility : AlwaysAbility
    {
        #region Effect List
        #region Data
        private List<DataEffectInstance<MoveIncreaseDataEffect>> _moveIncreaseDataEffects = new List<DataEffectInstance<MoveIncreaseDataEffect>>();
        private List<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>> _physicalResistanceIncreaseDataEffects = new List<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>>();
        private List<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>> _magicResistanceIncreaseDataEffects = new List<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>>();
        private List<DataEffectInstance<ReceiveDamageIncreaseDataEffect>> _receiveDamageIncreaseDataEffects = new List<DataEffectInstance<ReceiveDamageIncreaseDataEffect>>();
        private List<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>> _hpRecoveryPerSecByMaxHPIncreaseDataEffects = new List<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>>();
        #endregion

        #region Unable
        private List<UnableToMoveEffect> _unableToMoveEffects = new List<UnableToMoveEffect>();
        private List<UnableToAttackEffect> _unableToAttackEffects = new List<UnableToAttackEffect>();
        private List<UnableToHealEffect> _unableToHealEffects = new List<UnableToHealEffect>();
        private List<UnableToSkillEffect> _unableToSkillEffects = new List<UnableToSkillEffect>();
        #endregion
        #endregion

        #region 프로퍼티
        #region Data
        public IReadOnlyList<DataEffectInstance<MoveIncreaseDataEffect>> MoveIncreaseDataEffects => _moveIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<PhysicalResistanceIncreaseDataEffect>> PhysicalResistanceIncreaseDataEffects => _physicalResistanceIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<MagicResistanceIncreaseDataEffect>> MagicResistanceIncreaseDataEffects => _magicResistanceIncreaseDataEffects;
        public IReadOnlyList<DataEffectInstance<ReceiveDamageIncreaseDataEffect>> ReceiveDamageIncreaseDataEffects => _receiveDamageIncreaseDataEffects;
        public IReadOnlyList<DataEffectInstance<HPRecoveryPerSecByMaxHPIncreaseDataEffect>> HPRecoveryPerSecByMaxHPIncreaseDataEffects => _hpRecoveryPerSecByMaxHPIncreaseDataEffects;
        #endregion

        #region Unable
        internal IReadOnlyList<UnableToMoveEffect> UnableToMoveEffects => _unableToMoveEffects;
        internal IReadOnlyList<UnableToAttackEffect> UnableToAttackEffects => _unableToAttackEffects;
        internal IReadOnlyList<UnableToHealEffect> UnableToHealEffects => _unableToHealEffects;
        internal IReadOnlyList<UnableToSkillEffect> UnableToSkillEffects => _unableToSkillEffects;
        #endregion
        #endregion

        #region 스탯 계산
        public float finalAbnormalStatusResistance
        {
            get
            {
                float result = 0;

                #region 추가·차감
                foreach (var instance in _buffAbility.AbnormalStatusResistanceAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
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

        internal void ApplyAbnormalStatus(AbnormalStatusTemplate template, float duration, EffectContext context)
        {
            if (this == null || gameObject == null || template == null) return;

            // 상태이상 저항율을 넘지 못하면 반환
            if (Random.value < finalAbnormalStatusResistance) return;

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

                // 피격 시, 상태이상이 해제되야 한다면 횟수 초기화
                if (template.useHitCountLimit)
                {
                    instance.useCountLimit = true;
                    instance.count = template.hitCount;
                }

                return;
            }

            if (template.delay > 0)
                StartCoroutine(CoAddStatus(template, duration, context));
            else
                AddInstance(template, duration, context);
        }

        private IEnumerator CoAddStatus(AbnormalStatusTemplate template, float duration, EffectContext context)
        {
            yield return new WaitForSeconds(template.delay);
            AddInstance(template, duration, context);
        }

        #region 상태이상 추가
        private void AddInstance(AbnormalStatusTemplate template, float duration, EffectContext context)
        {
            // 포함되어 있지 않다면 생성
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);

            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }

            // 피격 시, 상태이상이 해제되야 한다면 횟수 초기화
            if (template.useHitCountLimit)
            {
                statusInstance.useCountLimit = true;
                statusInstance.count = template.hitCount;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            AddStatus(template, context);
        }

        /// <summary>
        /// 상태이상 추가
        /// </summary>
        private void AddStatus(AbnormalStatusTemplate template, EffectContext context)
        {
            ExecuteApplyFX(template);
            string displayName = template.displayName;

            foreach (var effect in template.effects)
            {
                #region Data
                if (AddDataEffect(effect, context, _moveIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _physicalResistanceIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, displayName, _magicResistanceIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _receiveDamageIncreaseDataEffects)) continue;
                if (AddDataEffect(effect, context, _hpRecoveryPerSecByMaxHPIncreaseDataEffects)) continue;
                #endregion

                #region Unable
                if (AddDataEffect(effect, _unableToMoveEffects)) continue;
                if (AddDataEffect(effect, _unableToAttackEffects)) continue;
                if (AddDataEffect(effect, _unableToHealEffects)) continue;
                if (AddDataEffect(effect, _unableToSkillEffects)) continue;
                #endregion
            }
        }
        #endregion

        #region 상태이상 유지시간 관리
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
        #endregion

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

        #region 상태이상 제거
        /// <summary>
        /// 상태이상 효과 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                #region Data
                if (RemoveDataEffect(effect, _moveIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _physicalResistanceIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _magicResistanceIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _receiveDamageIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _hpRecoveryPerSecByMaxHPIncreaseDataEffects)) continue;
                #endregion

                #region Unable
                if (RemoveDataEffect(effect, _unableToMoveEffects)) continue;
                if (RemoveDataEffect(effect, _unableToAttackEffects)) continue;
                if (RemoveDataEffect(effect, _unableToHealEffects)) continue;
                if (RemoveDataEffect(effect, _unableToSkillEffects)) continue;
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

        #region 상태이상 템플릿 포함 여부
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