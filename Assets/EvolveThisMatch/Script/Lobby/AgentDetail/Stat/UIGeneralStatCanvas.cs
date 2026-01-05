using EvolveThisMatch.Save;
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

        public void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            foreach (var stat in _stats)
            {
                if (stat.IsAvailable(template))
                {
                    stat.Initialize(template, owned);
                }
                else
                {
                    stat.Deinitialize();
                }
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