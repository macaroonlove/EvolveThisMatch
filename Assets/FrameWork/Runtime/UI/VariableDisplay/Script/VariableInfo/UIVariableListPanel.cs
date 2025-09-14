using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UI
{
    public class UIVariableListPanel : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        private Transform _parent;

        private List<UIVariableListItem> _variableListItems;

        private UnityAction<VariableInfo> _action;

        internal void Initialize(VariableInfoTemplate variableInfoTemplate, UnityAction<VariableInfo> action)
        {
            _action = action;

            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;

            InitializeVariableListItem(variableInfoTemplate);
        }

        internal void SelectItem(ObscuredIntVariable variable)
        {
            var item = _variableListItems.Find(x => x.info.variable == variable);
            item.SelectItem();
        }

        #region 리스트 아이템 생성
        private void InitializeVariableListItem(VariableInfoTemplate variableInfoTemplate)
        {
            int count = variableInfoTemplate.infos.Count;

            _variableListItems = new List<UIVariableListItem>(count);

            var variableListItem = GetComponentInChildren<UIVariableListItem>();

            // 나머지 프리팹 인스턴스 생성
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(variableListItem.gameObject, _parent).GetComponent<UIVariableListItem>();
                item.Initialize(variableInfoTemplate.infos[i], ChangeVariable);
                _variableListItems.Add(item);
            }

            Destroy(variableListItem.gameObject);
        }

        private void ChangeVariable(VariableInfo variableGroupTemplate)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _variableListItems) item.DeSelectItem();

            _action?.Invoke(variableGroupTemplate);
        }
        #endregion
    }
}