using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentInfoCanvas : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        private Transform _parent;

        private List<UIAgentInfoItem> _agentInfoItems;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
        }

        private void Start()
        {
            InitializeAgentInfoItem();
        }

        internal void Show()
        {
            base.Show();
        }

        private void InitializeAgentInfoItem()
        {
            var agentTemplates = GameDataManager.Instance.agentTemplates;
            int count = agentTemplates.Count;

            _agentInfoItems = new List<UIAgentInfoItem>(count);

            var agentInfoItem = GetComponentInChildren<UIAgentInfoItem>();
            _agentInfoItems.Add(agentInfoItem);

            // 나머지 프리팹 인스턴스 생성
            for (int i = 1; i < count; i++)
            {
                var item = Instantiate(agentInfoItem.gameObject, _parent).GetComponent<UIAgentInfoItem>();
                _agentInfoItems.Add(item);
            }

            RegistAgentInfoItem();
        }

        private void RegistAgentInfoItem()
        {
            var agentTemplates = GameDataManager.Instance.agentTemplates;
            var ownedAgents = GameDataManager.Instance.profileSaveData.ownedAgents;
            int count = agentTemplates.Count;

            // 보유한 유닛의 아이디
            var ownedAgentDic = ownedAgents.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                var template = agentTemplates[i];

                if (ownedAgentDic.TryGetValue(template.id, out var owned))
                {
                    // 보유한 유닛 → level, unitCount 전달
                    _agentInfoItems[i].Show(template, owned);
                }
                else
                {
                    // 미보유 유닛
                    _agentInfoItems[i].Show(template, null);
                }
            }
        }
    }
}