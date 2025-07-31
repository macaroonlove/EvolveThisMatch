using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposeSettingPanel : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            ConfilmButton,
            CloseButton,
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

        private UIAgentListCanvas_Department _agentListCanvas;
        private UICraftListPanel _craftListPanel;

        private TextMeshProUGUI _maxCountText; 
        private TextMeshProUGUI _counterText;
        private Slider _counterSlider;

        private DepartmentSaveData.Department _departmentData;
        private int _id;
        private int _agentId;
        private int _itemIndex;
        private int _craftAmount;

        private UnityAction _action;

        protected override void Initialize()
        {
            _agentListCanvas = GetComponentInChildren<UIAgentListCanvas_Department>();
            _craftListPanel = GetComponentInChildren<UICraftListPanel>();

            _agentListCanvas.Initialize(ChangeAgent);
            _craftListPanel.Initialize(ChangeCraft);

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindSlider(typeof(Sliders));

            _maxCountText = GetText((int)Texts.MaxCountText);
            _counterText = GetText((int)Texts.CounterText);
            _counterSlider = GetSlider((int)Sliders.CounterSlider);

            GetButton((int)Buttons.ConfilmButton).onClick.AddListener(Confilm);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            _counterSlider.onValueChanged.AddListener(ChangeCounter);
        }

        internal void Show(int id, DepartmentTemplate template, DepartmentSaveData.Department departmentData, UnityAction action)
        {
            _id = id;
            _departmentData = departmentData;
            _action = action;

            var job = departmentData.GetActiveJob(id);
            if (job != null) _counterSlider.value = job.maxAmount;
            else _counterSlider.value = 1;

            _craftListPanel.Show(template);
            
            Show(true);
        }

        private void ChangeAgent(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            _agentId = template.id;
        }

        private void ChangeCraft(UICraftListItem item)
        {
            _itemIndex = item.index;
        }

        private void ChangeCounter(float value)
        {
            _craftAmount = (int)value;

            _counterText.text = $"{_craftAmount} 개";
        }

        private async void Confilm()
        {
            _departmentData.SetActiveJob(_id, _agentId, _itemIndex, _craftAmount);
            
            Hide(true);
            _action?.Invoke();

            await SaveManager.Instance.Save_DepartmentData();
        }
    }
}