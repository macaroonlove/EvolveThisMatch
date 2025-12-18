using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIBattleLimitButton : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Text,
            NeedText,
        }
        enum Buttons
        {
            Button,
        }
        #endregion

        private TextMeshProUGUI _text;
        private TextMeshProUGUI _needText;
        private Button _button;

        private AgentUnit _agentUnit;
        private CrystalSystem _crystalSystem;
        private AgentLimitSystem _limitSystem;
        private bool _isInitialize;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _needText = GetText((int)Texts.NeedText);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(RequestUpgradeLimit);

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        #region BattleEvent
        private void OnBattleInitialize()
        {
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
            _limitSystem = BattleManager.Instance.GetSubSystem<AgentLimitSystem>();

            _isInitialize = false;
        }

        private void OnBattleDeinitialize()
        {
            _crystalSystem = null;
            _limitSystem = null;
        }

        private void OnUnsubscribe()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= OnUnsubscribe;
        }
        #endregion

        #region Show/Hide
        internal void Show(AgentUnit agentUnit)
        {
            _agentUnit = agentUnit;

            RefreshText();

            if (!_isInitialize)
            {
                _isInitialize = true;

                UpdateButtonState(_crystalSystem.currentCrystal);
            }

            _crystalSystem.onChangedCrystal += OnChangedCrystal;
        }

        public void Hide()
        {
            _crystalSystem.onChangedCrystal -= OnChangedCrystal;
        }
        #endregion

        private void OnChangedCrystal(int crystal)
        {
            if (_agentUnit == null) return;

            UpdateButtonState(crystal);
        }

        private void RequestUpgradeLimit()
        {
            if (_limitSystem == null || _agentUnit == null) return;

            UIPopupManager.Instance.ShowConfirmCancelPopup($"격을 임시 돌파 하시겠습니까?\n{_agentUnit.agentData.limit.agentLimitData.GetUpgradeLimitProbability()}", (isOn) =>
            {
                if (!isOn) return;

                int result = _limitSystem.RequestUpgradeLimit(_agentUnit.agentData);

                switch (result)
                {
                    case -3:
                        UIPopupManager.Instance.ShowNotificationPopup("세계의 파편이 부족합니다.");
                        return;
                    case -2:
                        UIPopupManager.Instance.ShowNotificationPopup("이미 신화급 격을 가지고 있습니다.");
                        return;
                    case -1:
                        UIPopupManager.Instance.ShowNotificationPopup("비정상적인 한계 돌파를 시도하고 있습니다.");
                        return;
                    case 0:
                        UIPopupManager.Instance.ShowNotificationPopup("격의 한계를 임시 돌파하는데 실패하였습니다.");
                        break;
                    case 1:
                        UIPopupManager.Instance.ShowNotificationPopup("격의 한계를 돌파하는데 성공하였습니다.");
                        break;
                    case 2:
                        UIPopupManager.Instance.ShowNotificationPopup("격의 한계를 돌파하는데 대성공하였습니다.");
                        break;
                    case 3:
                        UIPopupManager.Instance.ShowNotificationPopup("격이 새로운 경지로 승화되었습니다!");
                        break;
                    case 4:
                        UIPopupManager.Instance.ShowNotificationPopup("축하드립니다! 격이 인간의 영역을 초월하였습니다.");
                        break;
                    default:
                        return;
                }

                RefreshText();
            });
        }

        private void RefreshText()
        {
            _text.text = _agentUnit.limit.displayName.ToString();

            UpdateButtonState(_crystalSystem.currentCrystal);
        }

        private void UpdateButtonState(int crystal)
        {
            var rarity = _agentUnit.limit.rarity;
            bool canUpgrade = rarity != EAgentRarity.Myth;
            _button.gameObject.SetActive(canUpgrade);

            if (!canUpgrade) return;

            if (crystal >= 1)
            {
                _needText.color = new Color(0.1019f, 0.2352f, 0.3294f);
            }
            else
            {
                _needText.color = Color.red;
            }
        }
    }
}