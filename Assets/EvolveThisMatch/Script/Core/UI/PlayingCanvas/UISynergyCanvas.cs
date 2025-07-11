using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UISynergyCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            NextButton,
            PrevButton,
        }
        #endregion

        private Button _nextButton;
        private Button _prevButton;
        private AllySystem _allySystem;
        private AgentCreateSystem _agentCreateSystem;
        private AgentReturnSystem _agentReturnSystem;

        private UISynergyItem[] _synergyItems;
        private List<SynergyTemplate> _synergyTemplates = new List<SynergyTemplate>();

        private int _pageSize;
        private int _currentPage = 1;

        protected override void Initialize()
        {
            _synergyItems = GetComponentsInChildren<UISynergyItem>();

            foreach (var item in _synergyItems)
            {
                item.Hide(true);
            }
            _pageSize = 1;//_synergyItems.Length;

            BindButton(typeof(Buttons));

            _nextButton = GetButton((int)Buttons.NextButton);
            _prevButton = GetButton((int)Buttons.PrevButton);

            _nextButton.onClick.AddListener(NextPage);
            _prevButton.onClick.AddListener(PrevPage);
        }

        internal void InitializeBattle()
        {
            _allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();

            _agentCreateSystem.onInitializedUnit += AddSynergy;
            _agentReturnSystem.onDeinitializedUnit += DeleteSynergy;
        }

        internal void DeinitializeBattle()
        {
            _agentCreateSystem.onInitializedUnit -= AddSynergy;
            _agentReturnSystem.onDeinitializedUnit -= DeleteSynergy;
        }

        private void AddSynergy(AgentBattleData data)
        {
            foreach (var synergy in data.agentTemplate.synergy)
            {
                if (!_synergyTemplates.Contains(synergy))
                {
                    _synergyTemplates.Add(synergy);
                }
            }

            Refrash();
        }

        private void DeleteSynergy(AgentBattleData data)
        {
            var agents = _allySystem.GetAllAllies(EUnitType.Agent);

            _synergyTemplates.Clear();
            foreach (var agent in agents)
            {
                if (agent is AgentUnit agentUnit)
                {
                    foreach (var synergy in agentUnit.template.synergy)
                    {
                        if (!_synergyTemplates.Contains(synergy))
                        {
                            _synergyTemplates.Add(synergy);
                        }
                    }
                }
            }

            Refrash();
        }

        private void Refrash()
        {
            int synergyCount = _synergyTemplates.Count;
            int totalPage = Mathf.CeilToInt((float)synergyCount / _pageSize);

            // 현재 페이지가 전체 페이지보다 크면 현재 페이지를 마지막 페이지로
            if (totalPage == 0) _currentPage = 1;
            else if (_currentPage > totalPage) _currentPage = totalPage;

            // 버튼 상태 적용
            _prevButton.interactable = _currentPage > 1;
            _nextButton.interactable = _currentPage < totalPage;

            int startIndex = _pageSize * (_currentPage - 1);
            int endIndex = startIndex + _pageSize;

            int itemIndex = 0;
            for (int synergyIndex = startIndex; synergyIndex < endIndex; synergyIndex++)
            {
                if (synergyCount > synergyIndex)
                {
                    _synergyItems[itemIndex].Show(_synergyTemplates[synergyIndex]);
                }
                else
                {
                    _synergyItems[itemIndex].Hide(true);
                }

                itemIndex++;
            }
        }

        private void NextPage()
        {
            int totalPage = Mathf.CeilToInt((float)_synergyTemplates.Count / _pageSize);
            
            if (_currentPage < totalPage)
            {
                _currentPage++;
                Refrash();
            }
        }

        private void PrevPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                Refrash();
            }
        }
    }
}