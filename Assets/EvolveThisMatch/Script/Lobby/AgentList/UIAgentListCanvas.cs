using Cysharp.Threading.Tasks;
using DG.Tweening;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.Editor;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
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

        [Header("이벤트")]
        [SerializeField, Label("유닛 데이터 변경 시")] protected GameEvent _agentDataChangedGameEvent;

        protected Transform _parent;
        protected List<UIAgentListItem> _agentListItems;
        protected List<AgentTemplate> _agentTemplates;

        protected bool _isAsc;
        protected int _filterIndex;

        protected UnityAction<AgentTemplate, AgentSaveData.Agent> _onSelected;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            _parent = GetObject((int)Objects.Content).transform;

            _agentDataChangedGameEvent.AddListener(RefreshAgentListItem);

            InitializeAgentListItem();
        }

        internal void Bind(UnityAction<AgentTemplate, AgentSaveData.Agent> onSelected)
        {
            _onSelected = onSelected;
        }

        #region 리스트 아이템 생성
        private void InitializeAgentListItem()
        {
            _agentTemplates = GameDataManager.Instance.agentTemplates.ToList();
            _agentListItems = new List<UIAgentListItem>(_agentTemplates.Count);

            var agentInfoItem = GetComponentInChildren<UIAgentListItem>();

            // 나머지 프리팹 인스턴스 생성
            foreach (var tempalte in _agentTemplates)
            {
                var item = Instantiate(agentInfoItem.gameObject, _parent).GetComponent<UIAgentListItem>();
                item.Initialize(OnAgentSelected);
                _agentListItems.Add(item);
            }

            Destroy(agentInfoItem.gameObject);

            ChangeFilterOrder(0);

            SelectFirstItem().Forget();
        }

        private void OnAgentSelected(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 모든 아이템 선택 취소
            foreach (var item in _agentListItems) item.DeSelectItem();
            
            _onSelected?.Invoke(template, owned);
        }

        private async UniTaskVoid SelectFirstItem()
        {
            await UniTask.WaitUntil(() => _onSelected != null);

            if (_agentListItems.Count > 0)
                _agentListItems[0].SelectItem();
        }
        #endregion

        #region 필터 정렬
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

            RefreshAgentListItem();

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
        #endregion

        #region 리스트 갱신
        private void RefreshAgentListItem()
        {
            // 보유한 유닛의 아이디
            var ownedAgents = SaveManager.Instance.agentData.ownedAgents.ToDictionary(a => a.id);

            for (int i = 0; i < _agentTemplates.Count; i++)
            {
                var template = _agentTemplates[i];
                ownedAgents.TryGetValue(template.id, out var owned);

                _agentListItems[i].Show(template, owned);
            }
        }
        #endregion
    }
}