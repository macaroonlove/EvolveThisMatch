using EvolveThisMatch.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class UIFormationAgentSelectCanvas : UIAgentSelectCanvas
    {
        private UnityAction<AgentTemplate> _singleAction;
        private UnityAction<List<AgentTemplate>> _multiAction;

        private List<UIAgentSelectItem> _selectedItems = new List<UIAgentSelectItem>();
        private int _formationMaxCount;

        #region 초기화
        internal void Show(int currentIndex, UIFormationSlot[] selectedSlots, UnityAction<AgentTemplate> singleAction)
        {
            _selectedItems.Clear();

            var ownedAgents = SaveManager.Instance.agentData.ownedAgents;

            // 중복 제거용 HashSet 추가
            var duplicateTemplates = selectedSlots
                .Where((slot, index) => index != currentIndex && slot.template != null)
                .Select(slot => slot.template)
                .ToHashSet();

            // 보유한 유닛 추가
            var agentTemplates = ownedAgents
                .Select(owned => GameDataManager.Instance.GetAgentTemplateById(owned.id))
                .Where(template => template != null && duplicateTemplates.Contains(template) == false)
                // 등급 정렬하기
                .OrderBy(x => x.rarity.order)
                // 일련번호 정렬하기
                .ThenBy(x => x.id)
                .ToList();

            // 아이템 생성
            SetItems(agentTemplates);

            base.Show();

            // 현재 내가 누른 항목 선택
            var selectedAgentTemplate = selectedSlots[currentIndex].template;
            if (selectedAgentTemplate != null)
            {
                var selectedItem = _agentSelectItems.Find(x => x.template == selectedAgentTemplate);
                base.Select(selectedItem);

                _selectedItems.Add(selectedItem);
            }

            _singleAction = singleAction;
        }

        internal void Show(int formationMaxCount, UIFormationSlot[] selectedSlots, UnityAction<List<AgentTemplate>> multiAction)
        {
            _selectedItems.Clear();
            _formationMaxCount = formationMaxCount;

            var ownedAgents = SaveManager.Instance.agentData.ownedAgents;

            var agentTemplates = ownedAgents
                .Select(owned => GameDataManager.Instance.GetAgentTemplateById(owned.id))
                .Where(template => template != null)
                // 등급 정렬하기
                .OrderBy(x => x.rarity.order)
                // 일련번호 정렬하기
                .ThenBy(x => x.id)
                .ToList();

            // 아이템 생성
            SetItems(agentTemplates);

            base.Show();
            
            // 선택된 아이템 모두 선택
            foreach (var slot in selectedSlots)
            {
                if (slot.isEmpty) break;
                
                var selectedItem = _agentSelectItems.Find(x => x.template == slot.template);
                base.Select(selectedItem);
                
                _selectedItems.Add(selectedItem);
            }
            
            // 인덱스 갱신
            RefreshIndex();

            _multiAction = multiAction;
        }
        #endregion

        protected override void Select(UIAgentSelectItem item)
        {
            // 이미 선택된 아이템일 경우
            if (_selectedItems.Contains(item))
            {
                // 선택된 아이템을 제거
                _selectedItems.Remove(item);
                item?.UnSelect();
                Clear();

                if (_multiAction != null)
                {
                    // 인덱스 갱신
                    RefreshIndex();
                }

                return;
            }

            // 단일 선택 모드
            if (_singleAction != null)
            {
                if (_currentItem != null)
                    _selectedItems.Remove(_currentItem);

                _selectedItems.Add(item);
            }

            // 다중 선택 모드
            else if (_multiAction != null)
            {
                if (_formationMaxCount > _selectedItems.Count)
                {
                    _selectedItems.Add(item);
                    _currentItem = null;

                    // 인덱스 갱신
                    RefreshIndex();
                }
            }

            base.Select(item);
        }

        private void RefreshIndex()
        {
            int index = 1;
            foreach (var item in _selectedItems)
            {
                if (item is UIFormationAgentSelectItem formationItem)
                {
                    formationItem.Select(index);
                }
                index++;
            }
        }

        protected override void Apply()
        {
            if (_selectedItems.Count > 0)
            {                
                _singleAction?.Invoke(_selectedItems.First().template);
                _multiAction?.Invoke(_selectedItems.Select(x => x.template).ToList());
            }
            else
            {
                _singleAction?.Invoke(null);
                _multiAction?.Invoke(_selectedItems.Select(x => x.template).ToList());
            }

            base.Apply();
        }

        protected override void Hide()
        {
            _singleAction = null;
            _multiAction = null;

            base.Hide();
        }
    }
}