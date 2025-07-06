using FrameWork;
using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UITrainingSchoolCanvas : UIBase
    {
        #region 바인딩
        enum Toggles
        {
            TrainingSchoolToggle,
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

        private CanvasGroupController _panel;

        private AgentCreateSystem _agentCreateSystem;
        private CrystalSystem _crystalSystem;

        protected override void Initialize()
        {
            BindToggle(typeof(Toggles));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

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
                // TODO: 재능의 파편이 부족하다고 알림 주기
            }

            if (_agentCreateSystem.CreateRandomUnit(rarity))
            {
                _crystalSystem.PayCrystal(payCrystal);
            }
            else
            {
                // TODO: 자리가 부족하다고 알림 주기
            }
        }
    }
}