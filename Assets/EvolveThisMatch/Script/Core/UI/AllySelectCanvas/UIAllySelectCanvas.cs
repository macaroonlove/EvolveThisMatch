using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIAllySelectCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UnitRayCastSystem _unitRayCastSystem;

        private UIAllyInfoCanvas _allyInfoCanvas;
        private UIAllyActionCanvas _allyActionCanvas;

        private AttackRangeRenderer _attackRangeRenderer;

        protected override void Initialize()
        {
            _allyInfoCanvas = GetComponentInChildren<UIAllyInfoCanvas>();
            _allyActionCanvas = GetComponentInChildren<UIAllyActionCanvas>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);

            _unitRayCastSystem = BattleManager.Instance.GetSubSystem<UnitRayCastSystem>();
            _attackRangeRenderer = BattleManager.Instance.GetSubSystem<AttackRangeRenderer>();

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        private void OnBattleInitialize()
        {
            _unitRayCastSystem.onCast += Show;
        }

        private void OnBattleDeinitialize()
        {
            _unitRayCastSystem.onCast -= Show;
        }

        private void OnUnsubscribe()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= OnUnsubscribe;
        }

        private void Show(AllyUnit allyUnit)
        {
            if (allyUnit is AgentUnit agentUnit)
            {
                ShowAgentUnit(agentUnit);
            }
            else if (allyUnit is SignBoard signBoard)
            {
                if (signBoard.linkUnit != null)
                {
                    ShowAgentUnit(signBoard.linkUnit);
                }
            }
            else if (allyUnit is SummonUnit summonUnit)
            {
                // 유닛 정보
                _allyInfoCanvas.Show(summonUnit);
            }

            base.Show(true);
        }

        private void ShowAgentUnit(AgentUnit agentUnit)
        {
            // 유닛 정보
            _allyInfoCanvas.Show(agentUnit);

            // 유닛 액션
            _allyActionCanvas.Show(agentUnit);

            // 공격 범위
            if (agentUnit.template.job.job != EJob.Melee)
            {
                _attackRangeRenderer.Show((int)Mathf.Clamp(agentUnit.template.AttackRange, 0, 4));
            }
            else if (agentUnit.GetAbility<DeployAbility>().isSortie)
            {
                _attackRangeRenderer.Show(agentUnit, agentUnit.template.AttackRange);
            }
        }

        internal void Hide()
        {
            _allyInfoCanvas.Hide();
            _allyActionCanvas.Hide(true);
            _attackRangeRenderer.Hide();

            Hide(true);
        }

        internal void DestinyRecast(AgentUnit agentUnit)
        {
            _attackRangeRenderer.Hide();
            Show(agentUnit);
        }
    }
}