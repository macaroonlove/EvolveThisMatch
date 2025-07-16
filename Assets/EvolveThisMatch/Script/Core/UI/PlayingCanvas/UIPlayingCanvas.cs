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
        private UIActiveItemExecuteButton[] _uIActiveItemExecuteButtons;

        protected override void Initialize()
        {
            _uiCreateUnitButton = GetComponentInChildren<UICreateUnitButton>();
            _uiEnemyCountCanvas = GetComponentInChildren<UIEnemyCountCanvas>();
            _uiTrainingSchoolCanvas = GetComponentInChildren<UITrainingSchoolCanvas>();
            _uiEngraveCanvas = GetComponentInChildren<UIEngraveCanvas>();
            _uiSynergyCanvas = GetComponentInChildren<UISynergyCanvas>();
            _uIActiveItemExecuteButtons = GetComponentsInChildren<UIActiveItemExecuteButton>();

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

            foreach (var button in _uIActiveItemExecuteButtons)
            {
                button.GetActiveItem();
            }
        }

        private void OnBattleDeinitialize()
        {
            _uiEnemyCountCanvas.DeinitializeBattle();
            _uiEngraveCanvas.DeinitializeBattle();
            _uiSynergyCanvas.DeinitializeBattle();

            foreach (var button in _uIActiveItemExecuteButtons)
            {
                button.Hide();
            }
        }
    }
}