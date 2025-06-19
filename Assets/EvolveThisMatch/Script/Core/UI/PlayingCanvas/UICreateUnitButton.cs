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
        private CostSystem _costSystem;
        private int _needCost;

        internal new void Initialize()
        {
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _costSystem = BattleManager.Instance.GetSubSystem<CostSystem>();
            _costSystem.onChangedCost += OnChangeCost;

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _needCost = 20;
            _needCostText = GetText((int)Texts.NeedCostText);
            _needCostText.text = _needCost.ToString();

            _button = GetButton((int)Buttons.CraeteUnitButton);
            _button.onClick.AddListener(Create);
        }

        private void OnDestroy()
        {
            _costSystem.onChangedCost -= OnChangeCost;
        }

        private void OnChangeCost(int currentCost)
        {
            if (_needCost > currentCost)
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
                _costSystem.PayCost(_needCost);
                _needCost++;
                _needCostText.text = _needCost.ToString();
            }            
        }
    }
}