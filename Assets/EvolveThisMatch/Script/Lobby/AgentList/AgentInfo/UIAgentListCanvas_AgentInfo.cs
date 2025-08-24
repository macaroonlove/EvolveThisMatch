using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListCanvas_AgentInfo : UIAgentListCanvas
    {
        #region 바인딩
        enum Dropdowns
        {
            Filter,
        }
        enum Toggles
        {
            Order,
        }
        #endregion

        private Transform _orderIcon;

        internal override void Initialize(UnityAction<AgentTemplate, AgentSaveData.Agent> action = null)
        {
            base.Initialize(action);

            BindDropdown(typeof(Dropdowns));
            BindToggle(typeof(Toggles));

            var filter = GetDropdown((int)Dropdowns.Filter);
            var order = GetToggle((int)Toggles.Order);
            _orderIcon = order.transform.GetChild(0);

            order.onValueChanged.AddListener(ChangeSortOrder);
            filter.onValueChanged.AddListener(ChangeFilterOrder);
        }

        private void ChangeSortOrder(bool isOn)
        {
            // 아이콘 돌리기
            Vector3 rotation = _orderIcon.localEulerAngles;
            rotation.z = (isOn) ? 0 : 180;
            _orderIcon.localEulerAngles = rotation;

            // 오름차순이면 True, 내림차순이면 False
            _isAsc = isOn;

            ChangeFilterOrder(_filterIndex);
        }

        protected override void ChangeFilterOrder(int index)
        {
            base.ChangeFilterOrder(index);

            _agentListItems[0].SelectItem();
        }
    }
}