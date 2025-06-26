using FrameWork.Tooltip;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    [RequireComponent(typeof(TooltipTrigger))]
    public class UIBattleLevelButton : UIBase
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
        private CoinSystem _coinSystem;
        private TooltipTrigger _tooltipTrigger;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(LevelUp);

            _tooltipTrigger = GetComponent<TooltipTrigger>();

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        private void OnBattleInitialize()
        {
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();
        }

        private void OnBattleDeinitialize()
        {
            _coinSystem = null;
        }        

        private void OnUnsubscribe()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= OnUnsubscribe;
        }

        private void OnChangedCoin(int coin)
        {
            var needCoin = _agentUnit.GetNeedCoinToLevelUp();
            if (coin >= needCoin && needCoin != -1)
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

            _coinSystem.onChangedCoin += OnChangedCoin;
        }

        public void Hide()
        {
            _coinSystem.onChangedCoin -= OnChangedCoin;
        }

        private void LevelUp()
        {
            _agentUnit.LevelUp();

            SetText(_agentUnit);
        }

        private void SetText(AgentUnit agentUnit)
        {
            _text.text = agentUnit.level.ToString();
            OnChangedCoin(_coinSystem.currentCoin);

            var needCoin = _agentUnit.GetNeedCoinToLevelUp();
            if (needCoin == -1)
            {
                _tooltipTrigger.enabled = false;
                _tooltipTrigger.StopHover();
                return;
            }

            _tooltipTrigger.enabled = true;
            _tooltipTrigger.SetText("Description", $"레벨업에 필요한 코인 수: {agentUnit.GetNeedCoinToLevelUp()}");
        }
    }
}