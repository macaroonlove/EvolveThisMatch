using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIPlayingCanvas : UIBase
    {
        private UICreateUnitButton _uiCreateUnitButton;

        protected override void Initialize()
        {
            _uiCreateUnitButton = GetComponentInChildren<UICreateUnitButton>();

            BattleManager.Instance.onBattleInitialize += OnBattleStart;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleStart;
        }

        private void OnBattleStart()
        {
            _uiCreateUnitButton.Initialize();
        }
    }
}