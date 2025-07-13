using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIPlayingCanvas : UIBase
    {
        private UICreateUnitButton _uiCreateUnitButton;
        private UIEnemyCountCanvas _uiEnemyCountCanvas;
        private UITrainingSchoolCanvas _uiTrainingSchoolCanvas;
        private UIEngraveCanvas _uiEngraveCanvas;
        private UISynergyCanvas _uiSynergyCanvas;

        protected override void Initialize()
        {
            _uiCreateUnitButton = GetComponentInChildren<UICreateUnitButton>();
            _uiEnemyCountCanvas = GetComponentInChildren<UIEnemyCountCanvas>();
            _uiTrainingSchoolCanvas = GetComponentInChildren<UITrainingSchoolCanvas>();
            _uiEngraveCanvas = GetComponentInChildren<UIEngraveCanvas>();
            _uiSynergyCanvas = GetComponentInChildren<UISynergyCanvas>();

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
            _uiEnemyCountCanvas.InitializeBattle();
            _uiTrainingSchoolCanvas.InitializeBattle();
            _uiEngraveCanvas.InitializeBattle();
            _uiSynergyCanvas.InitializeBattle();
        }

        private void OnBattleDeinitialize()
        {
            _uiEnemyCountCanvas.DeinitializeBattle();
            _uiEngraveCanvas.DeinitializeBattle();
            _uiSynergyCanvas.DeinitializeBattle();
        }
    }
}