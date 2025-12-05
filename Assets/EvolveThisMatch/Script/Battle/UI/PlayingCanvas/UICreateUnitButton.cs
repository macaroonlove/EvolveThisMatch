using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
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
            _needCostText.text = $"<sprite name=coin> {_needCoin}";
        }

        private void OnDestroy()
        {
            if (_coinSystem != null)
            {
                _coinSystem.onChangedCoin -= OnChangeCoin;
            }
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
                UIPopupManager.Instance.ShowNotificationPopup("코인이 부족해 소환할 수 없습니다.");
            }

            var rarity = EAgentRarity.Common;

            if (BattleContext.heroSeal)
            {
                rarity = EAgentRarity.Epic;
                BattleContext.heroSeal = false;
            }

            if (_agentCreateSystem.CreateRandomUnit(rarity))
            {
                _coinSystem.PayCoin(_needCoin);
                _needCoin++;
                _needCostText.text = $"<sprite name=coin> {_needCoin}";
            }
            else
            {
                UIPopupManager.Instance.ShowNotificationPopup("더 이상 인물을 투영시키기 힘들어..");
            }
        }
    }
}