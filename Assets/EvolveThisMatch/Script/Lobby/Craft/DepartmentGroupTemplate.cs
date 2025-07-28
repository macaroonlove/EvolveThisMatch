using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [CreateAssetMenu(menuName = "Templates/Lobby/DepartmentGroup", fileName = "DepartmentGroup", order = 0)]
    public class DepartmentGroupTemplate : ScriptableObject
    {
        [SerializeField] private List<DepartmentTemplate> _departments = new List<DepartmentTemplate>();

        internal IReadOnlyList<DepartmentTemplate> departments => _departments;
    }
}