using FrameWork.Tooltip;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(TooltipTrigger))]
    public class UIBattleLimitButton : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Text,
        }
        enum Buttons
        {
            Button,
        }
        #endregion

        private TextMeshProUGUI _text;
        private Button _button;

        private AgentUnit _agentUnit;
        private CrystalSystem _crystalSystem;
        private TooltipTrigger _tooltipTrigger;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(UpgradeLimit);

            _tooltipTrigger = GetComponent<TooltipTrigger>();

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        private void OnBattleInitialize()
        {
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();
        }

        private void OnBattleDeinitialize()
        {
            _crystalSystem = null;
        }        

        private void OnUnsubscribe()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= OnUnsubscribe;
        }

        private void OnChangedCrystal(int crystal)
        {
            var rarity = _agentUnit.limit.rarity;

            if (crystal >= 1 && rarity != EAgentRarity.Myth)
            {
                _button.gameObject.SetActive(true);
            }
            else
            {
                _button.gameObject.SetActive(false);
            }
        }

        internal void Show(AgentUnit agentUnit)
        {
            _agentUnit = agentUnit;
            SetText(agentUnit);

            _crystalSystem.onChangedCrystal += OnChangedCrystal;
        }

        public void Hide()
        {
            _crystalSystem.onChangedCrystal -= OnChangedCrystal;
        }

        private void UpgradeLimit()
        {
            _agentUnit.UpgradeLimit();

            SetText(_agentUnit);
        }

        private void SetText(AgentUnit agentUnit)
        {
            _text.text = agentUnit.limit.displayName.ToString();
            OnChangedCrystal(_crystalSystem.currentCrystal);

            var limit = _agentUnit.limit;
            if (limit.rarity == EAgentRarity.Myth)
            {
                _tooltipTrigger.enabled = false;
                _tooltipTrigger.StopHover();
                return;
            }

            _tooltipTrigger.enabled = true;
            _tooltipTrigger.SetText("Description", $"재능 해금에 필요한 크리스탈 수: 1\n" +
                $"성공 확률: {limit.successProbability}");
        }
    }
}