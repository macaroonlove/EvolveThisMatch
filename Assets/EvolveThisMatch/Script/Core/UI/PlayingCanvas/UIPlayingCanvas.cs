using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIPlayingCanvas : UIBase
    {
        private UICreateUnitButton _uiCreateUnitButton;
        private UITrainingSchoolCanvas _uiTrainingSchoolCanvas;

        protected override void Initialize()
        {
            _uiCreateUnitButton = GetComponentInChildren<UICreateUnitButton>();
            _uiTrainingSchoolCanvas = GetComponentInChildren<UITrainingSchoolCanvas>();

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
        }

        private void OnBattleInitialize()
        {
            _uiCreateUnitButton.InitializeBattle();
            _uiTrainingSchoolCanvas.InitializeBattle();
        }
    }
}