using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIBattleSyncButton : UIBase
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
        private CoinSystem _coinSystem;
        private AgentSyncSystem _syncSystem;
        private bool _isInitialize;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _needText = GetText((int)Texts.NeedText);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(RequestIncreaseSync);

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        #region BattleEvent
        private void OnBattleInitialize()
        {
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _syncSystem = BattleManager.Instance.GetSubSystem<AgentSyncSystem>();

            _isInitialize = false;
        }

        private void OnBattleDeinitialize()
        {
            _coinSystem = null;
            _syncSystem = null;
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

                int needCoin = _syncSystem.GetNeedCoin(_agentUnit.agentData);
                UpdateButtonState(needCoin, _coinSystem.currentCoin);
            }

            _coinSystem.onChangedCoin += OnChangedCoin;
        }

        public void Hide()
        {
            if (_coinSystem != null)
            {
                _coinSystem.onChangedCoin -= OnChangedCoin;
            }
        }
        #endregion

        private void OnChangedCoin(int coin)
        {
            if (_agentUnit == null) return;

            int needCoin = _syncSystem.GetNeedCoin(_agentUnit.agentData);
            UpdateButtonState(needCoin, coin);
        }

        private void RequestIncreaseSync()
        {
            if (_syncSystem == null || _agentUnit == null) return;

            int result = _syncSystem.RequestIncreaseSync(_agentUnit.agentData);

            switch (result) 
            {
                case -3:
                    UIPopupManager.Instance.ShowNotificationPopup("코인이 부족합니다.");
                    return;
                case -2:
                    UIPopupManager.Instance.ShowNotificationPopup("더 이상 동기화율을 상승시킬 수 없습니다.");
                    return;
                case -1:
                    UIPopupManager.Instance.ShowNotificationPopup("비정상적인 동기화율 상승을 시도하고 있습니다.");
                    return;
                case 1:
                    UIPopupManager.Instance.ShowNotificationPopup("성공적으로 동기화율을 상승시켰습니다.");
                    break;
                default:
                    return;
            }

            RefreshText();
        }

        private void RefreshText()
        {
            int sync = _agentUnit.sync;
            int needCoin = _syncSystem.GetNeedCoin(_agentUnit.agentData);

            _text.text = sync.ToString();
            _needText.text = $"<sprite name=\"Coin\"> {needCoin}";

            UpdateButtonState(needCoin, _coinSystem.currentCoin);
        }

        private void UpdateButtonState(int needCoin, int currentCoin)
        {
            bool canUpgrade = needCoin > 0;
            _button.gameObject.SetActive(canUpgrade);

            if (!canUpgrade) return;

            if (currentCoin >= needCoin)
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