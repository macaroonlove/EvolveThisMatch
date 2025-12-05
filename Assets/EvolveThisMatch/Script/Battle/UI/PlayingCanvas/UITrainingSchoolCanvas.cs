using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UITrainingSchoolCanvas : UIBase
    {
        #region 바인딩
        enum Toggles
        {
            TrainingSchoolToggle,
        }
        enum Texts
        {
            RarePayText,
            EpicPayText,
            LegendPayText,
        }
        enum Buttons
        {
            RareUnitButton,
            EpicUnitButton,
            LegendUnitButton,
        }
        enum CanvasGroup
        {
            TrainingSchoolPanel,
        }
        #endregion

        private TextMeshProUGUI _rarePayText;
        private TextMeshProUGUI _epicPayText;
        private TextMeshProUGUI _legendPayText;
        private CanvasGroupController _panel;

        private AgentCreateSystem _agentCreateSystem;
        private CrystalSystem _crystalSystem;

        protected override void Initialize()
        {
            BindToggle(typeof(Toggles));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

            _rarePayText = GetText((int)Texts.RarePayText);
            _epicPayText = GetText((int)Texts.EpicPayText);
            _legendPayText = GetText((int)Texts.LegendPayText);
            _panel = GetCanvasGroupController((int)CanvasGroup.TrainingSchoolPanel);

            GetToggle((int)Toggles.TrainingSchoolToggle).onValueChanged.AddListener(ActivePanel);
            GetButton((int)Buttons.RareUnitButton).onClick.AddListener(CreateRareUnit);
            GetButton((int)Buttons.EpicUnitButton).onClick.AddListener(CreateEpicUnit);
            GetButton((int)Buttons.LegendUnitButton).onClick.AddListener(CreateLegendUnit);
        }

        internal void InitializeBattle()
        {
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();

            _crystalSystem.onChangedCrystal += OnChangeCrystal;
        }

        internal void DeinitializeBattle()
        {
            _crystalSystem.onChangedCrystal -= OnChangeCrystal;
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

        private void OnChangeCrystal(int value)
        {
            _rarePayText.color = value >= 1 ? Color.white : Color.red;
            _epicPayText.color = value >= 3 ? Color.white : Color.red;
            _legendPayText.color = value >= 7 ? Color.white : Color.red;
        }

        private void CreateRareUnit()
        {
            CreateUnit(EAgentRarity.Rare, 1);
        }

        private void CreateEpicUnit()
        {
            CreateUnit(EAgentRarity.Epic, 3);
        }

        private void CreateLegendUnit()
        {
            CreateUnit(EAgentRarity.Legend, 7);
        }

        private void CreateUnit(EAgentRarity rarity, int payCrystal)
        {
            if (!_crystalSystem.CheckCrystal(payCrystal))
            {
                UIPopupManager.Instance.ShowNotificationPopup("재능의 파편이 부족해 소환할 수 없습니다.");
                return;
            }

            if (_agentCreateSystem.CreateRandomUnit(rarity))
            {
                _crystalSystem.PayCrystal(payCrystal);
            }
            else
            {
                UIPopupManager.Instance.ShowNotificationPopup("더 이상 인물을 투영시키기 힘들어..");
            }
        }
    }
}