using FrameWork.Tooltip;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
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

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _text = GetText((int)Texts.Text);
            _button = GetButton((int)Buttons.Button);

            _button.onClick.AddListener(UpgradeLimit);

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
            UIPopupManager.Instance.ShowConfirmCancelPopup($"<sprite name=\"Crystal\">을 1개 사용하여\n유닛의 잠재력을 향상시키시겠습니까?\n<size=28>성공 확률: {_agentUnit.limit.successProbability}</size>", (isOn) =>
            {
                if (isOn && _crystalSystem.PayCrystal(1))
                {
                    _agentUnit.UpgradeLimit();

                    SetText(_agentUnit);
                }
            });            
        }

        private void SetText(AgentUnit agentUnit)
        {
            _text.text = agentUnit.limit.displayName.ToString();
            OnChangedCrystal(_crystalSystem.currentCrystal);
        }
    }
}