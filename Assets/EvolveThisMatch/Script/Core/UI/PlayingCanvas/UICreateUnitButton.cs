using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UICreateUnitButton : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CraeteUnitButton,
        }

        enum Texts
        {
            NeedCostText,
        }
        #endregion

        private TextMeshProUGUI _needCostText;
        private Button _button;

        private AgentCreateSystem _agentCreateSystem;
        private CoinSystem _coinSystem;
        private int _needCoin;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _needCostText = GetText((int)Texts.NeedCostText);
            _button = GetButton((int)Buttons.CraeteUnitButton);

            _button.onClick.AddListener(Create);
        }

        internal void InitializeBattle()
        {
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _coinSystem.onChangedCoin += OnChangeCoin;

            _needCoin = 20;
            _needCostText.text = _needCoin.ToString();
        }

        private void OnDestroy()
        {
            _coinSystem.onChangedCoin -= OnChangeCoin;
        }

        private void OnChangeCoin(int currentCoin)
        {
            if (_needCoin > currentCoin)
            {
                _button.enabled = false;
                _needCostText.color = Color.red;
            }
            else
            {
                _button.enabled = true;
                _needCostText.color = Color.white;
            }
        }

        private void Create()
        {
            if (!_coinSystem.CheckCoin(_needCoin))
            {
                // TODO: 코인이 부족하다고 알림 주기
            }

            if (_agentCreateSystem.CreateRandomUnit())
            {
                _coinSystem.PayCoin(_needCoin);
                _needCoin++;
                _needCostText.text = _needCoin.ToString();
            }
            else
            {
                // TODO: 자리가 부족하다고 알림 주기
            }
        }
    }
}