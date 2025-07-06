using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIEngraveCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Toggles
        {
            EngraveToggle,
        }
        enum Buttons
        {
            LuckEngraveButton,
            DivineEngraveButton,
            DarkEngraveButton,
            FireEngraveButton,
            WaterEngraveButton,
            EarthEngraveButton,
            WindEngraveButton,
            ThunderEngraveButton,
        }
        enum Texts
        {
            PayCoinText,
        }
        enum CanvasGroup
        {
            EngravePanel,
        }
        #endregion

        private CanvasGroupController _panel;
        private TextMeshProUGUI _payCoinText;

        private CoinSystem _coinSystem;

        private UIProbabilityTable _uiProbabilityTable;

        protected override void Initialize()
        {
            _uiProbabilityTable = GetComponentInChildren<UIProbabilityTable>();

            BindToggle(typeof(Toggles));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroup));

            _panel = GetCanvasGroupController((int)CanvasGroup.EngravePanel);
            _payCoinText = GetText((int)Texts.PayCoinText);

            GetToggle((int)Toggles.EngraveToggle).onValueChanged.AddListener(ActivePanel);
            GetButton((int)Buttons.LuckEngraveButton).onClick.AddListener(LuckEngrave);
        }

        internal void InitializeBattle()
        {
            _coinSystem = BattleManager.Instance.GetSubSystem<CoinSystem>();

            RefrashLuckEngrave();
        }

        private void ActivePanel(bool isOn)
        {
            if (isOn)
            {
                _panel.Show();
            }
            else
            {
                _panel.Hide();
            }
        }

        private void LuckEngrave()
        {
            var needCoin = GameDataManager.Instance.GetProbabilityList().needCoin;
            
            if (!_coinSystem.CheckCoin(needCoin)) return;

            if (GameDataManager.Instance.UpgradeProbabilityLevel())
            {
                _coinSystem.PayCoin(needCoin);

                RefrashLuckEngrave();
            }
        }

        private void RefrashLuckEngrave()
        {
            var probability = GameDataManager.Instance.GetProbabilityList();

            if (probability.needCoin == -1)
            {
                _payCoinText.text = "Max";
            }
            else
            {
                _payCoinText.text = probability.needCoin.ToString();
            }

            _uiProbabilityTable.Show(probability);
        }
    }
}