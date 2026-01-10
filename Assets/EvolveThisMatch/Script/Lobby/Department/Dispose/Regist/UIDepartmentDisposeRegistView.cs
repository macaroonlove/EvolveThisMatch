using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentDisposeRegistView : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            ConfilmButton,
            CloseButton,
            MinusButton,
            PlusButton,
        }
        enum Texts
        {
            MaxCountText,
            CounterText,
        }
        enum Sliders
        {
            CounterSlider,
        }
        #endregion

        private UIDepartmentDisposeRegistPresenter _presenter;

        private UIAgentListCanvas_Department _agentListCanvas;
        private UICraftListView _craftListView;

        private TextMeshProUGUI _maxCountText;
        private TextMeshProUGUI _counterText;
        private Slider _counterSlider;
        private Button _confilmButton;

        public event UnityAction<int, int, int, int> onRegistJob;

        protected override void Initialize()
        {
            var model = new UIDepartmentDisposeRegistModel();
            _presenter = new UIDepartmentDisposeRegistPresenter(this, model);

            _agentListCanvas = GetComponentInChildren<UIAgentListCanvas_Department>();
            _craftListView = GetComponentInChildren<UICraftListView>();

            _agentListCanvas.Bind(SelectAgent);
            _craftListView.onSelectCraftItem += SelectCraftItem;

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindSlider(typeof(Sliders));

            _maxCountText = GetText((int)Texts.MaxCountText);
            _counterText = GetText((int)Texts.CounterText);
            _counterSlider = GetSlider((int)Sliders.CounterSlider);
            _confilmButton = GetButton((int)Buttons.ConfilmButton);

            _confilmButton.onClick.AddListener(() => { _presenter.Confilm(onRegistJob); });
            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.MinusButton).onClick.AddListener(() => _counterSlider.value--);
            GetButton((int)Buttons.PlusButton).onClick.AddListener(() => _counterSlider.value++);

            _counterSlider.onValueChanged.AddListener(ChangeCounter);
        }
        public void Bind(DepartmentData titleData, DepartmentLocalSaveData localData) => _presenter.Bind(titleData, localData);

        public void Show(int index)
        {
            _presenter.Show(index);
            base.Show(true);
        }

        private void SelectAgent(AgentTemplate template, AgentSaveData.Agent owned)
        {
            _presenter.SelectAgent(template.id);
        }

        private void SelectCraftItem(int index)
        {
            _presenter.SelectCraftItem(index);
        }

        private void ChangeCounter(float value)
        {
            _presenter.ChangeCounter((int)value);
        }

        public void Render(DepartmentDisposeRegistViewState state)
        {
            _craftListView.Show(state.titleData);
            _agentListCanvas.Show(state.deployList);

            _counterText.text = $"{state.craftCount} 개";
            _counterSlider.value = state.craftCount;
            _maxCountText.text = $"{state.maxCount} 개";
            _confilmButton.interactable = state.canConfilm;
        }

        public void RenderCount(DepartmentDisposeRegistViewState state)
        {
            _counterText.text = $"{state.craftCount} 개";
            _maxCountText.text = $"{state.maxCount} 개";
            _confilmButton.interactable = state.canConfilm;
        }
    }

    public readonly struct DepartmentDisposeRegistViewState
    {
        public readonly DepartmentData titleData;
        public readonly int craftCount;
        public readonly int maxCount;
        public readonly bool canConfilm;
        public readonly List<int> deployList;

        public DepartmentDisposeRegistViewState(DepartmentData titleData, int craftCount, int maxCount, bool isDeploy, List<int> deployList)
        {
            this.titleData = titleData;
            this.craftCount = craftCount;
            this.maxCount = maxCount;
            this.canConfilm = !isDeploy && maxCount > 0;
            this.deployList = deployList;
        }
    }
}