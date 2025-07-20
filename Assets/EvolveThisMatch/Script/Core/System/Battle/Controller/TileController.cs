using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TileController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private AllySystem _allySystem;

        // 배치된 유닛
        private AgentBattleData _placedAgentData;

        // 해당 타일의 유닛 존재 여부
        public bool isPlaceUnit => _placedAgentData != null;

        // 배치된 유닛의 아이디
        public int placedAgentUnitId => _placedAgentData.agentUnit.id;

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
        public AgentBattleData PlaceUnit(AgentUnit agentUnit, AgentTemplate agentTemplate)
        {
            _placedAgentData = _allySystem.Regist(agentUnit, agentTemplate);
            _placedAgentData.ComfirmTile(this);

            agentUnit.transform.position = transform.position;

            return _placedAgentData;
        }

        /// <summary>
        /// 유닛 반환
        /// </summary>
        public void ReturnUnit()
        {
            _allySystem.Deregist(_placedAgentData);

            _placedAgentData = null;
        }
        #endregion

        public void VisibleRenderer(bool value)
        {
            _spriteRenderer.enabled = value;
        }
    }
}
