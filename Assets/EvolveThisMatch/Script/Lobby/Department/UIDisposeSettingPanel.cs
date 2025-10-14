using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
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

        private UIAgentListCanvas_Department _agentListCanvas;
        private UICraftListPanel _craftListPanel;

        private TextMeshProUGUI _maxCountText;
        private TextMeshProUGUI _counterText;
        private Slider _counterSlider;
        private Button _confilmButton;

        private UIDepartmentCanvas _departmentCanvas;
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
            _confilmButton = GetButton((int)Buttons.ConfilmButton); 
            
            _confilmButton.onClick.AddListener(Confilm);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.MinusButton).onClick.AddListener(() => _counterSlider.value--);
            GetButton((int)Buttons.PlusButton).onClick.AddListener(() => _counterSlider.value++);

            _counterSlider.onValueChanged.AddListener(ChangeCounter);
        }

        internal void Show(int id, UIDepartmentCanvas departmentCanvas, UnityAction action)
        {
            _id = id;
            _departmentCanvas = departmentCanvas;
            _action = action;

            var job = departmentCanvas.localData.GetActiveJob(id);
            if (job != null) _counterSlider.value = job.maxAmount;
            else
            {
                _craftAmount = 1;
                _counterSlider.value = 1;
                _counterText.text = "1 개";
            }

            _craftListPanel.Show(departmentCanvas.titleData);

            ChangeCraft(0);

            Show(true);
        }

        /// <summary>
        /// 유닛 변경 시
        /// </summary>
        private void ChangeAgent(AgentTemplate template, AgentSaveData.Agent owned)
        {
            _agentId = template.id;
        }

        private void ChangeCraft(UICraftListItem item)
        {
            _itemIndex = item.index;

            ChangeCraft(item.index);
        }

        /// <summary>
        /// 생산품 변경 시
        /// </summary>
        private async void ChangeCraft(int index)
        {
            if (_departmentCanvas == null) return;

            var requiredItems = _departmentCanvas.titleData.CraftItems[index].RequiredItems;

            int maxCraftableCount = 999;

            foreach (var required in requiredItems)
            {
                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(required.Variable);
                int ownedCount = variable.Value;
                int craftableCount = ownedCount / required.Amount;

                maxCraftableCount = Mathf.Min(maxCraftableCount, craftableCount);
            }

            // 최대 생산 가능량
            _maxCountText.text = $"{maxCraftableCount} 개";

            _confilmButton.interactable = maxCraftableCount > 0;
        }

        /// <summary>
        /// 생산 개수 변경 시
        /// </summary>
        private void ChangeCounter(float value)
        {
            _craftAmount = (int)value;

            _counterText.text = $"{_craftAmount} 개";
        }

        /// <summary>
        /// 확정
        /// </summary>
        private void Confilm()
        {
            _departmentCanvas.localData.SetActiveJob(_id, _agentId, _itemIndex, _craftAmount);

            Hide(true);
            _action?.Invoke();

            SaveManager.Instance.departmentData.SaveDepartmentLocalData();
        }
    }
}