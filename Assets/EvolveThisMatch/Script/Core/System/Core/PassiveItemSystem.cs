using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 패시브 아이템 효과를 적용시키는 시스템
    /// </summary>
    public class PassiveItemSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField, ReadOnly] private List<PassiveItemTemplate> _items = new List<PassiveItemTemplate>();

        private List<AlwaysEffect> _alwaysEffects = new List<AlwaysEffect>();
        private List<GlobalEvent> _globalEvents = new List<GlobalEvent>();
        private List<UnitEvent> _unitEvents = new List<UnitEvent>();

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private List<PassiveItemTemplate> _debugItems = new List<PassiveItemTemplate>();
#endif

        public void Initialize()
        {
#if UNITY_EDITOR
            foreach (var debugItem in _debugItems)
            {
                AddItem(debugItem, true);
            }
#endif
        }

        public void Deinitialize()
        {

        }

        #region 구독
        private void Start()
        {
            if (BattleManager.Instance == null) return;

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += Unsubscribe;
        }

        private void Unsubscribe()
        {
            if (BattleManager.Instance == null) return;

            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= Unsubscribe;
        }

        private void OnBattleInitialize()
        {
            BattleManager.Instance.GetSubSystem<AllySystem>().onRegist += ApplyAlwaysEvent;
            BattleManager.Instance.GetSubSystem<EnemySystem>().onRegist += ApplyAlwaysEvent;
        }

        private void OnBattleDeinitialize()
        {
            BattleManager.Instance.GetSubSystem<AllySystem>().onRegist -= ApplyAlwaysEvent;
            BattleManager.Instance.GetSubSystem<EnemySystem>().onRegist -= ApplyAlwaysEvent;
        }
        #endregion

        private void ApplyAlwaysEvent(Unit unit)
        {
            foreach (var effect in _alwaysEffects)
            {
                effect.Execute(unit);
            }
        }

        /// <summary>
        /// 아이템 추가
        /// (저장되있는 아이템을 로드할 경우 isNewItem을 false로 넘겨주기)
        /// </summary>
        public void AddItem(PassiveItemTemplate template, bool isNewItem = true)
        {
            if (_items.Contains(template))
            {
#if UNITY_EDITOR
                Debug.LogError($"아이템이 중복되었습니다. {template.displayName}");
#endif
                return;
            }

            _items.Add(template);

            foreach (var trigger in template.triggers)
            {
                if (trigger is GetGameTrigger getTrigger && isNewItem)
                {
                    foreach (var effect in getTrigger.effects)
                    {
                        if (effect is GlobalEffect globalEffect)
                        {
                            globalEffect.Execute();
                        }
                    }
                }
                else if (trigger is AlwaysGameTrigger alwaysTrigger)
                {
                    foreach (var effect in alwaysTrigger.effects)
                    {
                        if (effect is AlwaysEffect alwaysEffect)
                        {
                            _alwaysEffects.Add(alwaysEffect);
                        }
                    }
                }
                else if (trigger is GlobalEventGameTrigger globalTrigger)
                {
                    Action action = () =>
                    {
                        foreach (var effect in globalTrigger.effects)
                        {
                            if (effect is GlobalEffect globalEffect)
                            {
                                globalEffect.Execute();
                            }
                        }
                    };

                    globalTrigger.globalEvent.AddListener(action);

                    _globalEvents.Add(globalTrigger.globalEvent);
                }
                else if (trigger is UnitEventGameTrigger unitTrigger)
                {
                    Action<Unit, Unit> action = (casterUnit, targetUnit) =>
                    {
                        foreach (var effect in unitTrigger.effects)
                        {
                            if (effect is GlobalEffect globalEffect)
                            {
                                globalEffect.Execute();
                            }
                            else if (effect is UnitEffect unitEffect)
                            {
                                unitEffect.Execute(casterUnit, targetUnit);
                            }
                        }

                        ExecuteCasterFX(template, casterUnit);
                    };

                    unitTrigger.unitEvent.AddListener(action);

                    _unitEvents.Add(unitTrigger.unitEvent);
                }
            }
        }

        private void OnDestroy()
        {
            _alwaysEffects.Clear();

            foreach (var item in _globalEvents)
            {
                item.RemoveAll();
            }
            _globalEvents.Clear();

            foreach (var item in _unitEvents)
            {
                item.RemoveAll();
            }
            _unitEvents.Clear();

            _items.Clear();
        }

        #region FX
        private void ExecuteCasterFX(PassiveItemTemplate template, Unit caster)
        {
            if (template.casterFX != null)
            {
                template.casterFX.Play(caster);
            }
        }
        #endregion
    }
}