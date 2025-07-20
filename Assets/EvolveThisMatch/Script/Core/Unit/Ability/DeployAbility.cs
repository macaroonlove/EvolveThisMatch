using Cysharp.Threading.Tasks;
using FrameWork.Editor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DeployAbility : ConditionAbility
    {
        [SerializeField] private FX _sortieStart;
        [SerializeField] private FX _sortieEnd;

        [SerializeField, ReadOnly, Label("성 유닛인가?")] private bool _isCastle;
        [SerializeField, ReadOnly, Label("출격하였는가?")] private bool _isSortie;

        private AgentUnit _agentUnit;
        private MoveAbility _moveAbility;
        private AgentCreateSystem _agentCreateSystem;
        private AgentReturnSystem _agentReturnSystem;

        public bool isSortie => _isSortie;

        internal override void Initialize(Unit unit)
        {
            if (unit is AgentUnit agentUnit)
            {
                if (agentUnit.template.job.job != EJob.Melee)
                {
                    _isCastle = true;
                }
                else
                {
                    _isCastle = false;
                    _isSortie = false;
                    _agentUnit = agentUnit;
                    _moveAbility = unit.GetAbility<MoveChaseAbility>();
                    _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
                    _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();
                }
            }

            base.Initialize(unit);
        }

        internal override bool IsExecute()
        {
            if (_isCastle) return false;

            return !_isSortie;
        }

        public async UniTask StartSortie(Vector3 sortiePosition)
        {
            var agentData = _agentUnit.agentData;

            _isSortie = true;
            unit.SetInteraction(false);

            _sortieStart.Play(_agentUnit);

            await UniTask.Delay(500);

            // 위치 적용
            SetAgentPosition(agentData, sortiePosition);

            _sortieEnd.Play(_agentUnit);

            // 표지판 생성
            var signBoard = _agentCreateSystem.CreateSignBoard(agentData);
            signBoard.SetInteraction(false);

            await UniTask.Delay(1500);

            // 표지판 등록
            agentData.RegistSignBoard(signBoard);

            // 배치 설정
            _isSortie = true;

            unit.SetInteraction(true);
            signBoard.SetInteraction(true);

            // 배치 능력 해제
            unit.ReleaseCurrentAbility();
        }

        public async UniTask ReturnSortie()
        {
            var agentData = _agentUnit.agentData;

            // 배치 해제
            _isSortie = false;
            
            // 이동 멈추기
            _moveAbility.StopAbility();

            unit.SetInteraction(false);

            // 표지판 반환
            if (agentData.signBoard != null)
            {
                _agentReturnSystem.ReturnSignBoard(agentData.signBoard.gameObject);
                agentData.DeregistSignBoard();
            }

            _sortieEnd.Play(unit);

            await UniTask.Delay(500);

            // 위치 적용
            SetAgentPosition(agentData, agentData.mountTile.transform.position);

            _sortieStart.Play(unit);

            unit.SetInteraction(true);
        }

        private void SetAgentPosition(AgentBattleData selectedData, Vector3 targetPosition)
        {
            var agentTransform = selectedData.agentUnit.transform;
            agentTransform.position = targetPosition;

            var pos = agentTransform.position;
            pos.z = pos.y;
            agentTransform.position = pos;

            agentTransform.GetChild(3).localScale = Vector3.one * 0.15f;
        }
    }
}