using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UIAgentSelectCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            ApplyButton,
        }

        enum Texts
        {
            DisplayName,
        }
        #endregion

        [SerializeField] private ToggleGroup _parent;
        [SerializeField] private GameObject _prefab;

        private TextMeshProUGUI _displayNameText;
        private UIGeneralStatCanvas _statCanvas;

        protected List<UIAgentSelectItem> _agentSelectItems = new List<UIAgentSelectItem>();

        protected UIAgentSelectItem _currentItem;

        protected override void Initialize()
        {
            _statCanvas = GetComponentInChildren<UIGeneralStatCanvas>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _displayNameText = GetText((int)Texts.DisplayName);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.ApplyButton).onClick.AddListener(Apply);
        }

        public override void Show(bool isForce = false)
        {
            _displayNameText.text = "유닛을 선택해주세요.";
            _statCanvas.Clear();

            foreach (var item in _agentSelectItems)
            {
                item.UnSelect();
            }

            base.Show(isForce);
        }

        protected void SetItems(List<AgentTemplate> agentTemplates)
        {
            int count = agentTemplates.Count;

            // 필요한 수만큼 생성
            while (_agentSelectItems.Count < count)
            {
                var instance = Instantiate(_prefab, _parent.transform);
                var toggle = instance.GetComponent<UIAgentSelectItem>();
                _agentSelectItems.Add(toggle);
            }

            // 초기화
            for (int i = 0; i < _agentSelectItems.Count; i++)
            {
                if (i < count)
                {
                    _agentSelectItems[i].Show(agentTemplates[i], Select);
                }
                else
                {
                    _agentSelectItems[i].Hide();
                }
            }
        }

        protected virtual void Select(UIAgentSelectItem item)
        {
            _currentItem?.UnSelect();
            _currentItem = item;
            _currentItem?.Select();

            // 유닛 정보 적용
            var template = item.template;
            _displayNameText.text = template.displayName;
            _statCanvas.ShowInfomation(template);
        }

        protected void Clear()
        {
            _currentItem = null;

            _displayNameText.text = "유닛을 선택해주세요.";
            _statCanvas.Clear();
        }

        protected virtual void Apply()
        {
            Hide();
        }

        protected virtual void Hide()
        {
            base.Hide();
        }
    }
}