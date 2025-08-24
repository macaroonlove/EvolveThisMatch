using DG.Tweening;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public abstract class UIAgentListCanvas : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        protected Transform _parent;
        protected List<UIAgentListItem> _agentListItems;
        protected List<AgentTemplate> _agentTemplates;

        protected bool _isAsc;
        protected int _filterIndex;

        protected UnityAction<AgentTemplate, AgentSaveData.Agent> _action;

        internal virtual void Initialize(UnityAction<AgentTemplate, AgentSaveData.Agent> action = null)
        {
            _action = action;

            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
        }

        protected void Start()
        {
            InitializeAgentListItem();
        }

        internal void SelectFirstItem()
        {
            _agentListItems[0].SelectItem();
        }

        #region 리스트 아이템 생성
        private void InitializeAgentListItem()
        {
            _agentTemplates = GameDataManager.Instance.agentTemplates.ToList();
            int count = _agentTemplates.Count;

            _agentListItems = new List<UIAgentListItem>(count);

            var agentInfoItem = GetComponentInChildren<UIAgentListItem>();

            // 나머지 프리팹 인스턴스 생성
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(agentInfoItem.gameObject, _parent).GetComponent<UIAgentListItem>();
                item.Initialize(ChangeAgent);
                _agentListItems.Add(item);
            }

            Destroy(agentInfoItem.gameObject);

            ChangeFilterOrder(0);
        }

        private void ChangeAgent(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _agentListItems) item.DeSelectItem();

            _action?.Invoke(template, owned);
        }

        internal void RegistAgentListItem()
        {
            var ownedAgents = SaveManager.Instance.agentData.ownedAgents;
            int count = _agentTemplates.Count;

            // 보유한 유닛의 아이디
            var ownedAgentDic = ownedAgents.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                var template = _agentTemplates[i];

                if (ownedAgentDic.TryGetValue(template.id, out var owned))
                {
                    // 보유한 유닛 → level, unitCount 전달
                    _agentListItems[i].Show(template, owned);
                }
                else
                {
                    // 미보유 유닛
                    _agentListItems[i].Show(template, null);
                }
            }
        }
        #endregion

        protected virtual void ChangeFilterOrder(int index)
        {
            _filterIndex = index;

            var ownedAgents = SaveManager.Instance.agentData.ownedAgents;

            // 보유한 유닛의 아이디
            var ownedAgentDic = ownedAgents.ToDictionary(a => a.id);

            switch (index)
            {
                case 0:
                    SortBy(t => -(int)t.rarity.rarity);
                    break;
                case 1:
                    SortBy(t => t.ATK);
                    break;
                case 2:
                    SortBy(t => ownedAgentDic.TryGetValue(t.id, out var o) ? o.level : int.MinValue);
                    break;
                case 3:
                    SortBy(t => ownedAgentDic.TryGetValue(t.id, out var o) ? o.tier : int.MinValue);
                    break;
            }

            RegistAgentListItem();

            _parent.DOLocalMoveY(0, 0.1f);
        }

        private void SortBy<T>(Func<AgentTemplate, T> primaryKey) where T : IComparable<T>
        {
            if (_isAsc)
            {
                _agentTemplates = _agentTemplates
                    .OrderBy(primaryKey)
                    .ThenBy(t => t.id)
                    .ToList();
            }
            else
            {
                _agentTemplates = _agentTemplates
                    .OrderByDescending(primaryKey)
                    .ThenBy(t => t.id)
                    .ToList();
            }
        }
    }
}