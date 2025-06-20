using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UICreateUnitButton : UIBase
    {
        #region ¹ÙÀÎµù
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

        internal new void Initialize()
        {
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
            _coinSystem.onChangedCoin += OnChangeCoin;

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _needCoin = 20;
            _needCostText = GetText((int)Texts.NeedCostText);
            _needCostText.text = _needCoin.ToString();

            _button = GetButton((int)Buttons.CraeteUnitButton);
            _button.onClick.AddListener(Create);
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
            if (_agentCreateSystem.CreateRandomUnit())
            {
                _coinSystem.PayCoin(_needCoin);
                _needCoin++;
                _needCostText.text = _needCoin.ToString();
            }            
        }
    }
}