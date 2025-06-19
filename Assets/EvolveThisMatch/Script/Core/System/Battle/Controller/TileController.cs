using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TileController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private AllySystem _allySystem;

        // 배치된 유닛
        private AgentUnit _placedAgentUnit;

        // 해당 타일의 유닛 존재 여부
        public bool isPlaceUnit => _placedAgentUnit != null;

        // 배치된 유닛의 아이디
        public int placedAgentUnitId => _placedAgentUnit.id;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        internal void Initialize()
        {
            _spriteRenderer.enabled = false;

            _allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();
        }

        internal void Deinitialize()
        {

        }

        #region 유닛 배치 & 반환
        /// <summary>
        /// 유닛 배치
        /// </summary>
        internal void PlaceUnit(AgentUnit agentUnit)
        {
            _allySystem.Regist(agentUnit);

            _placedAgentUnit = agentUnit;

            agentUnit.transform.position = transform.position;
        }

        /// <summary>
        /// 유닛 반환
        /// </summary>
        internal void ReturnUnit()
        {
            _allySystem.Deregist(_placedAgentUnit);

            _placedAgentUnit = null;
        }
        #endregion

        public void VisibleRenderer(bool value)
        {
            _spriteRenderer.enabled = value;
        }
    }
}
