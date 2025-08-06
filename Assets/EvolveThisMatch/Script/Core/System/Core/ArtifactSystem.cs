using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 패시브 아이템 효과를 적용시키는 시스템
    /// </summary>
    public class ArtifactSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField, ReadOnly] private List<ArtifactTemplate> _items = new List<ArtifactTemplate>();

        private Dictionary<AlwaysEffect, int> _alwaysEffects = new Dictionary<AlwaysEffect, int>();
        private List<GlobalEvent> _globalEvents = new List<GlobalEvent>();
        private List<UnitEvent> _unitEvents = new List<UnitEvent>();

        private Dictionary<int, ProfileSaveData.Artifact> _ownedArtifactDic;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private List<ArtifactTemplate> _debugItems = new List<ArtifactTemplate>();
#endif

        public async void Initialize()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);
            await UniTask.WaitUntil(() => SaveManager.Instance.profileData.isLoaded);

            var artifactTemplates = GameDataManager.Instance.artifactTemplates.ToList();
            var ownedArtifacts = GameDataManager.Instance.profileSaveData.ownedArtifacts;
            _ownedArtifactDic = ownedArtifacts.ToDictionary(a => a.id);

            foreach (var owned in ownedArtifacts)
            {
                var artifact = artifactTemplates[owned.id];
                AddItem(artifact);
            }

#if UNITY_EDITOR
            foreach (var debugItem in _debugItems)
            {
                AddItem(debugItem);
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

        private async void ApplyAlwaysEvent(Unit unit)
        {
            await UniTask.Yield();

            foreach (var effect in _alwaysEffects)
            {
                effect.Key.Execute(unit, effect.Value);
            }
        }

        /// <summary>
        /// 아이템 추가
        /// (저장되있는 아이템을 로드할 경우 isNewItem을 false로 넘겨주기)
        /// </summary>
        public void AddItem(ArtifactTemplate template)
        {
            if (_items.Contains(template))
            {
#if UNITY_EDITOR
                Debug.LogError($"아이템이 중복되었습니다. {template.displayName}");
#endif
                return;
            }

            _items.Add(template);

            var level = _ownedArtifactDic[template.id].level;

            foreach (var trigger in template.triggers)
            {
                if (trigger is GetGameTrigger getTrigger)
                {
                    foreach (var effect in getTrigger.effects)
                    {
                        if (effect is GlobalEffect globalEffect)
                        {
                            globalEffect.Execute(level);
                        }
                    }
                }
                else if (trigger is AlwaysGameTrigger alwaysTrigger)
                {
                    foreach (var effect in alwaysTrigger.effects)
                    {
                        if (effect is AlwaysEffect alwaysEffect)
                        {
                            _alwaysEffects.Add(alwaysEffect, level);
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
                                globalEffect.Execute(level);
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
                                globalEffect.Execute(level);
                            }
                            else if (effect is UnitEffect unitEffect)
                            {
                                unitEffect.Execute(casterUnit, targetUnit, level);
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
        private void ExecuteCasterFX(ArtifactTemplate template, Unit caster)
        {
            if (template.casterFX != null)
            {
                template.casterFX.Play(caster);
            }
        }
        #endregion
    }
}