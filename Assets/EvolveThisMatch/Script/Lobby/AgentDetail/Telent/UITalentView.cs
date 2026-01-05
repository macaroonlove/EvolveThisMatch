using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITalentView : UIBase
    {
        #region 바인딩
        enum Texts
        {
            ChangeTalentText,
        }
        enum Buttons
        {
            ChangeTalentButton,
            OpenTalentFilterButton,
        }
        enum CanvasGroups
        {
            Dim,
        }
        #endregion

        private TextMeshProUGUI _changeTalentText;
        private Button _changeTalentButton;
        private Button _openTalentFilterButton;
        private CanvasGroupController _dim;

        private UITalentItem[] _items;
        private UITalentFilterView _talentFilterPanel;
        private UITalentController _controller;

        #region 초기화
        protected override void Initialize()
        {
            _items = GetComponentsInChildren<UITalentItem>();

            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroups));

            _changeTalentText = GetText((int)Texts.ChangeTalentText);
            _changeTalentButton = GetButton((int)Buttons.ChangeTalentButton);
            _openTalentFilterButton = GetButton((int)Buttons.OpenTalentFilterButton);
            _dim = GetCanvasGroupController((int)CanvasGroups.Dim);

            InitializeController();
        }

        private void InitializeController()
        {
            var data = AgentSaveDataTemplate.talentTitleData;

            float[] rarityProb =
            {
                data.mythRarity,
                data.legendRarity,
                data.epicRarity,
                data.rareRarity,
                data.commonRarity
            };

            var model = new UITalentModel(rarityProb);
            _controller = new UITalentController(model, this);

            _changeTalentButton.onClick.AddListener(_controller.OnRollOnce);
            _openTalentFilterButton.onClick.AddListener(OpenFilter);
        }

        internal void Bind(UITalentFilterView talentFilterPanel)
        {
            _talentFilterPanel = talentFilterPanel;
        }
        #endregion

        internal void Show(AgentSaveData.Agent owned)
        {
            _controller.Show(owned);
        }

        public void Render(TalentViewState state)
        {
            _dim.ShowOrHide(!state.showTalent);
            if (!state.showTalent) return;

            _changeTalentText.text = state.buttonText;
            _changeTalentButton.interactable = state.canPay;
            _openTalentFilterButton.interactable = state.canPay;

            for (int i = 0; i < _items.Length; i++)
            {
                int idx = i;
                var slot = state.slots[idx];

                _items[idx].Show(slot, isLock =>
                {
                    _controller.OnLockChanged(idx, isLock);
                });
            }
        }

        #region 필터
        private void OpenFilter()
        {
            var buttonText = _controller.currentButtonText;
            _talentFilterPanel.Show(buttonText, condition => _controller.OnRollFiltered(condition), _controller.StopRolling);
        }
        #endregion

        #region 초기화 (외부에서 재능을 적용해서)
        public void ClearRNG() => _controller.ClearRNG();
        #endregion
    }

    public struct TalentViewState
    {
        public bool showTalent;
        public bool canPay;
        public string buttonText;
        public TalentSlotState[] slots;
    }

    public struct TalentSlotState
    {
        public int id;
        public int value;
        public bool isLocked;
        public AgentRarityTemplate rarity;
    }
}