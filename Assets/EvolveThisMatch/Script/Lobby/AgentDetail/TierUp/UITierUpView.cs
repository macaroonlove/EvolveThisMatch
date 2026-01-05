using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITierUpView : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            CounterText,
        }
        enum Images
        {
            CounterImage,
        }
        enum Buttons
        {
            TierUpButton,
        }
        enum Objects
        {
            Arrow,
        }
        #endregion

        private TextMeshProUGUI _counterText;
        private Image _counterImage;
        private Button _tierUpButton;
        private GameObject _arrow;

        private UIAgentTier[] _tierGroups;
        private UITierAdvantageItem[] _tierAdvantageItems;

        private UITierUpController _controller;

        protected override void Initialize()
        {
            var model = new UITierUpModel();
            _controller = new UITierUpController(this, model);

            _tierGroups = GetComponentsInChildren<UIAgentTier>();
            _tierAdvantageItems = GetComponentsInChildren<UITierAdvantageItem>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _counterText = GetText((int)Texts.CounterText);
            _counterImage = GetImage((int)Images.CounterImage);
            _tierUpButton = GetButton((int)Buttons.TierUpButton);
            _arrow = GetObject((int)Objects.Arrow);

            _tierUpButton.onClick.AddListener(_controller.TierUp);
        }

        internal void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _controller.Show(owned, reShow);
        }

        internal void RenderEmpty()
        {
            _tierUpButton.interactable = false;
            _counterText.text = "0/1";
            _counterImage.fillAmount = 0;

            foreach (var item in _tierAdvantageItems)
                item.ShowItem(false);
        }

        public void Render(TierUpViewState state)
        {
            _counterText.text = state.counterText;
            _counterImage.fillAmount = state.counterFill;

            _tierUpButton.interactable = state.canTierUp;

            _tierGroups[0].Show(state.currentTier);

            if (state.showNextTier)
            {
                _tierGroups[1].gameObject.SetActive(true);
                _tierGroups[1].Show(state.nextTier);
                _arrow.SetActive(true);
            }
            else
            {
                _tierGroups[1].gameObject.SetActive(false);
                _arrow.SetActive(false);
            }

            for (int i = 0; i < _tierAdvantageItems.Length; i++)
            {
                _tierAdvantageItems[i].ShowItem(state.tierAdvantages[i]);
            }
        }
    }

    public struct TierUpViewState
    {
        public string counterText;
        public float counterFill;
        public bool showNextTier;
        public int currentTier;
        public int nextTier;
        public bool[] tierAdvantages;
        public bool canTierUp;
    }
}