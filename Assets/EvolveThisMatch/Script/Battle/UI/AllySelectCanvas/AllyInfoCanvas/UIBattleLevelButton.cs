using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
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

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(LevelUp);

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
            var needCoin = _agentUnit.GetNeedCoinToLevelUp();

            if (needCoin == -1) return;

            UIPopupManager.Instance.ShowConfirmCancelPopup($"<sprite name=\"Coin\">을 {_agentUnit.GetNeedCoinToLevelUp()}개 사용하여\n유닛의 레벨을 올리시겠습니까?", (isOn) =>
            {
                if (isOn && _coinSystem.PayCoin(needCoin))
                {
                    _agentUnit.LevelUp();

                    SetText(_agentUnit);
                }
            });
        }

        private void SetText(AgentUnit agentUnit)
        {
            _text.text = agentUnit.level.ToString();
            OnChangedCoin(_coinSystem.currentCoin);
        }
    }
}