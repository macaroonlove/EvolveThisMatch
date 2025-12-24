using FrameWork.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전역 상태들을 관리하는 클래스
    /// </summary>
    public class GlobalStatusSystem : MonoBehaviour, ICoreSystem
    {
        #region Effect List
        #region Data
        // Attack
        private List<StatDataEffectInstance<ATKIncreaseDataEffect>> _atkIncreaseDataEffects = new List<StatDataEffectInstance<ATKIncreaseDataEffect>>();
        private List<StatDataEffectInstance<ATKMultiplierDataEffect>> _atkMultiplierDataEffects = new List<StatDataEffectInstance<ATKMultiplierDataEffect>>();

        // Gold
        private List<DataEffectInstance<GoldGainAdditionalDataEffect>> _goldGainAdditionalDataEffects = new List<DataEffectInstance<GoldGainAdditionalDataEffect>>();
        private List<DataEffectInstance<GoldGainIncreaseDataEffect>> _goldGainIncreaseDataEffects = new List<DataEffectInstance<GoldGainIncreaseDataEffect>>();
        private List<DataEffectInstance<GoldGainMultiplierDataEffect>> _goldGainMultiplierDataEffects = new List<DataEffectInstance<GoldGainMultiplierDataEffect>>();
        #endregion
        #endregion

        #region 프로퍼티
        #region Data
        public IReadOnlyList<StatDataEffectInstance<ATKIncreaseDataEffect>> ATKIncreaseDataEffects => _atkIncreaseDataEffects;
        public IReadOnlyList<StatDataEffectInstance<ATKMultiplierDataEffect>> ATKMultiplierDataEffects => _atkMultiplierDataEffects;

        public IReadOnlyList<DataEffectInstance<GoldGainAdditionalDataEffect>> GoldGainAdditionalDataEffects => _goldGainAdditionalDataEffects;
        public IReadOnlyList<DataEffectInstance<GoldGainIncreaseDataEffect>> GoldGainIncreaseDataEffects => _goldGainIncreaseDataEffects;
        public IReadOnlyList<DataEffectInstance<GoldGainMultiplierDataEffect>> GoldGainMultiplierDataEffects => _goldGainMultiplierDataEffects;
        #endregion

        #endregion

        private Dictionary<GlobalStatusTemplate, StatusInstance> statusDic = new Dictionary<GlobalStatusTemplate, StatusInstance>();

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private List<GlobalStatusTemplate> statusList = new List<GlobalStatusTemplate>();
#endif

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
            ClearStatusEffects();
        }

        public void ApplyGlobalStatus(GlobalStatusTemplate template, float duration, EffectContext context)
        {
            if (this == null || gameObject == null || template == null) return;

            var isContained = false;

            if (statusDic.ContainsKey(template))
            {
                isContained = true;

                var instance = statusDic[template];
                if (instance.IsOld(duration))
                {
                    instance.duration = duration;
                    instance.startTime = Time.time;
                }
                else
                {
                    return;
                }
            }

            AddStatus(template, duration, isContained, context);
        }

        /// <summary>
        /// 전역 상태 추가
        /// </summary>
        private void AddStatus(GlobalStatusTemplate template, float duration, bool isContained, EffectContext context)
        {
            StatusInstance statusInstance = new StatusInstance(duration, Time.time);

            // 무한지속이 아니라면
            if (duration != int.MaxValue)
            {
                var corutine = StartCoroutine(CoStatus(statusInstance, template));
                statusInstance.corutine = corutine;
            }

            statusDic.Add(template, statusInstance);

#if UNITY_EDITOR
            statusList.Add(template);
#endif

            // 전역 상태 적용 (동일한 전역 상태는 중복되지 않음)
            if (isContained == false)
            {
                ExecuteApplyFX(template);
                string displayName = template.displayName;

                foreach (var effect in template.effects)
                {
                    #region 전투력
                    if (AddDataEffect(effect, context, displayName, _atkIncreaseDataEffects)) continue;
                    if (AddDataEffect(effect, context, displayName, _atkMultiplierDataEffects)) continue;
                    #endregion

                    #region 골드 획득량
                    if (AddDataEffect(effect, context, _goldGainAdditionalDataEffects)) continue;
                    if (AddDataEffect(effect, context, _goldGainIncreaseDataEffects)) continue;
                    if (AddDataEffect(effect, context, _goldGainMultiplierDataEffects)) continue;
                    #endregion
                }
            }
        }

        #region 전역 상태 유지시간 관리
        private IEnumerator CoStatus(StatusInstance statusInstance, GlobalStatusTemplate template)
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

        #region 전역 상태 제거
        /// <summary>
        /// 모든 전역 상태 제거
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
        /// 전역 상태 제거
        /// </summary>
        private void RemoveStatus(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                #region 전투력
                if (RemoveDataEffect(effect, _atkIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _atkMultiplierDataEffects)) continue;
                #endregion

                #region 골드 획득량
                if (RemoveDataEffect(effect, _goldGainAdditionalDataEffects)) continue;
                if (RemoveDataEffect(effect, _goldGainIncreaseDataEffects)) continue;
                if (RemoveDataEffect(effect, _goldGainMultiplierDataEffects)) continue;
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

        #region 전역 상태 포함 여부
        internal bool Contains(GlobalStatusTemplate template)
        {
            return statusDic.ContainsKey(template);
        }

        internal bool Contains(List<GlobalStatusTemplate> templates)
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
        private void ExecuteApplyFX(GlobalStatusTemplate template)
        {
            if (template.applyFX != null)
            {
                template.applyFX.Play(null);
            }
        }

        private void ExecuteRemoveFX(GlobalStatusTemplate template)
        {
            if (template.removeFX != null)
            {
                template.removeFX.Play(null);
            }
        }
        #endregion
    }
}