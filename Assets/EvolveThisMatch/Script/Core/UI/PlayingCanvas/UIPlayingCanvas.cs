using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIPlayingCanvas : UIBase
    {
        private UICreateUnitButton _uiCreateUnitButton;
        private UITrainingSchoolCanvas _uiTrainingSchoolCanvas;
        private UIEngraveCanvas _uiEngraveCanvas;

        protected override void Initialize()
        {
            _uiCreateUnitButton = GetComponentInChildren<UICreateUnitButton>();
            _uiTrainingSchoolCanvas = GetComponentInChildren<UITrainingSchoolCanvas>();
            _uiEngraveCanvas = GetComponentInChildren<UIEngraveCanvas>();

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
        }

        private void OnBattleInitialize()
        {
            _uiCreateUnitButton.InitializeBattle();
            _uiTrainingSchoolCanvas.InitializeBattle();
            _uiEngraveCanvas.InitializeBattle();
        }

        private void OnBattleDeinitialize()
        {
            _uiEngraveCanvas.DeinitializeBattle();
        }
    }
}