using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private DepartmentSaveDataTemplate _departmentSaveData;
        [SerializeField] private DepartmentGroupTemplate _departmentGroup;

        internal DepartmentSaveDataTemplate departmentSaveData => _departmentSaveData;
        internal IReadOnlyList<DepartmentTemplate> departments => _departmentGroup.departments;

        protected override async void Initialize()
        {
            await UniTask.WaitUntil(() => SaveManager.Instance.agentData.isLoaded);

            BattleManager.Instance.InitializeBattle();
        }

        private void OnDestroy()
        {
            BattleManager.Instance.DeinitializeBattle();
        }
    }
}