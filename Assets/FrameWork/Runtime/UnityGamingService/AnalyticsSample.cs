using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UGS
{
    public class AnalyticsSample : MonoBehaviour
    {
        private void Start()
        {
            Analytics.Record("Framework", new Dictionary<string, object>
            {
                { "test1", 1 },
                { "test2", "ÀÌ" }
            });
        }
    }
}