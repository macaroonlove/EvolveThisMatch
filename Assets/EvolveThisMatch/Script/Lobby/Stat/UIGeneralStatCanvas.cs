using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIGeneralStatCanvas : MonoBehaviour
    {
        private List<IGeneralStat> _stats = new List<IGeneralStat>();

        private void Awake()
        {
            _stats = GetComponentsInChildren<IGeneralStat>().ToList();
        }

        private void OnDestroy()
        {
            _stats.Clear();
        }

        public void ShowInfomation(AgentTemplate template)
        {
            foreach (var stat in _stats)
            {
                stat.Initialize(template);
            }

            if (template.job.job != EJob.Melee)
            {
                _stats[3].Deinitialize();
                _stats[4].Deinitialize();
                _stats[7].Deinitialize();
                _stats[8].Deinitialize();
            }
        }

        public void ShowInfomation(EnemyTemplate template)
        {
            foreach (var stat in _stats)
            {
                stat.Initialize(template);
            }
        }

        public void Clear()
        {
            foreach (var stat in _stats)
            {
                stat.Clear();
            }
        }
    }
}