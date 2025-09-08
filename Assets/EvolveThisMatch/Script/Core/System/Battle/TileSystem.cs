using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TileSystem : MonoBehaviour, IBattleSystem
    {
        private List<TileController> _controllers = new List<TileController>();

        public Transform sortiePoint { get; private set; }

        private void Awake()
        {
            sortiePoint = transform.GetChild(0);

            var controllers = this.GetComponentsInChildren<TileController>(true);

            foreach (var controller in controllers)
            {
                _controllers.Add(controller);
            }
        }

        public void Initialize()
        {
            foreach (var controller in _controllers)
            {
                controller.Initialize();
            }
        }

        public void Deinitialize()
        {
            foreach (var controller in _controllers)
            {
                controller.Deinitialize();
            }
        }

        public void VisibleRenderer(bool isOn)
        {
            foreach (var controller in _controllers)
            {
                controller.VisibleRenderer(isOn);
            }
        }

        public List<AgentBattleData> GetPlacedAgentDatas()
        {
            var results = new List<AgentBattleData>();
            foreach (var controller in _controllers)
            {
                results.Add(controller.placedAgentData);
            }
            return results;
        }

        #region 유틸리티 메서드
        /// <summary>
        /// 타일 불러오기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TileController GetTile(int id)
        {
            return _controllers[id];
        }

        /// <summary>
        /// 배치 가능한 타일 찾기
        /// </summary>
        public TileController GetPlaceAbleTile()
        {
            return _controllers.FirstOrDefault(controller => !controller.isPlaceUnit);
        }

        #endregion
    }
}