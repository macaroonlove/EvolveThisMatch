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
    }
}