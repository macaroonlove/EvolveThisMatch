using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TileSystem : MonoBehaviour, IBattleSystem
    {
        private List<TileController> _controllers = new List<TileController>();
        
        internal Transform sortiePoint { get; private set; }

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

        #region 유틸리티 메서드
        /// <summary>
        /// 배치 가능한 타일 찾기
        /// </summary>
        internal TileController GetPlaceAbleTile(int id)
        {
            return _controllers.FirstOrDefault(controller => !controller.isPlaceUnit);
        }

        #endregion
    }
}