using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIBattleStatCanvas : MonoBehaviour
    {
        private List<IBattleStat> _stats = new List<IBattleStat>();

        private void Awake()
        {
            _stats = GetComponentsInChildren<IBattleStat>().ToList();
        }

        internal void ShowInfomation(AllyUnit unit)
        {
            foreach (var stat in _stats)
            {
                stat.Initialize(unit);
            }

            if (unit is AgentUnit agentUnit)
            {
                if (agentUnit.template.job.job != EJob.Melee)
                {
                    _stats[2].Deinitialize();
                    _stats[3].Deinitialize();
                }
            }
        }
    }
}