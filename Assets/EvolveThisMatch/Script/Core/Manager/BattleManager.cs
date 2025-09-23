using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using FrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 배틀에 관련된 시스템 관리
    /// </summary>
    public class BattleManager : Singleton<BattleManager>
    {
        private Dictionary<Type, IBattleSystem> _subSystems = new Dictionary<Type, IBattleSystem>();

        internal Transform canvas { get; private set; }

        public event UnityAction onBattleInitialize;
        public event UnityAction onBattleDeinitialize;
        public event UnityAction onBattleManagerDestroy;

        [SerializeField] private GlobalEvent _battleStartGlobalEvent;

        protected override void Awake()
        {
            base.Awake();

            var systems = this.GetComponentsInChildren<IBattleSystem>(true);
            foreach (var system in systems)
            {
                var type = system.GetType();
                _subSystems.Add(type, system);

                var baseType = type.BaseType;
                while (baseType != null && typeof(IBattleSystem).IsAssignableFrom(baseType))
                {
                    if (!_subSystems.ContainsKey(baseType))
                    {
                        _subSystems.Add(baseType, system);
                    }
                    baseType = baseType.BaseType;
                }
            }

            canvas = GetComponentInChildren<Canvas>().transform;

            // TODO: 포톤을 통해 방에 입장하면 RPC를 통해 target을 AllViaServer로 Ready 여부를 보냄, 두 플레이어가 모두 준비 완료했을 때, 호출하도록 구현
            //InitializeBattle();
        }

        private void OnDestroy()
        {
            onBattleManagerDestroy?.Invoke();
        }

        [ContextMenu("배틀시작")]
        public void InitializeBattle()
        {
            foreach (var system in new HashSet<IBattleSystem>(_subSystems.Values))
            {
                system.Initialize();
            }

            // 각 템플릿 초기화
            GameDataManager.Instance.InitializeData();

            onBattleInitialize?.Invoke();
            _battleStartGlobalEvent?.Raise();
        }

        public void DeinitializeBattle()
        {
            onBattleDeinitialize?.Invoke();

            foreach (var item in _subSystems.Values)
            {
                item.Deinitialize();
            }
        }

        public T GetSubSystem<T>() where T : IBattleSystem
        {
            _subSystems.TryGetValue(typeof(T), out var subSystem);
            return (T)subSystem;
        }
    }
}