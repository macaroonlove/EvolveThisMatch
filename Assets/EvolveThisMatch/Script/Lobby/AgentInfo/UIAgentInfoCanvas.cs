using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentInfoCanvas : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        enum Texts
        {
            DisplayName,
            Level,
        }
        enum Images
        {
            FullBody,
            Synergy,
        }
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private Transform _parent;
        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIAgentTier _tierGroup;
        private UIGeneralStatCanvas _generalStatCanvas;
        private UISkillCanvas _skillCanvas;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _level;
        private Image _fullBody;
        private Image _synergy;
        
        private List<UIAgentInfoItem> _agentInfoItems;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _parent = GetObject((int)Objects.Content).transform;
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _tierGroup = GetComponentInChildren<UIAgentTier>();
            _generalStatCanvas = GetComponentInChildren<UIGeneralStatCanvas>();
            _skillCanvas = GetComponentInChildren<UISkillCanvas>();

            _displayName = GetText((int)Texts.DisplayName);
            _level = GetText((int)Texts.Level);
            _fullBody = GetImage((int)Images.FullBody);
            _synergy = GetImage((int)Images.Synergy);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
        }

        private void Start()
        {
            InitializeAgentInfoItem();
        }

        internal void Show(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            _displayName.text = template.displayName;
            _fullBody.sprite = template.sprite;
            _synergy.sprite = template.synergy[0].icon;

            if (owned != null)
            {
                // 유닛 레벨
                _level.text = $"Lv. {owned.level}";

                // 유닛 티어
                _tierGroup.Show(owned.tier);
            }

            // 등급 태그
            _rarityTag.Show(template.rarity);

            // 직업 태그
            _jobTag.Show(template.job);

            // 스탯 캔버스
            _generalStatCanvas.ShowInfomation(template);

            // 스킬 캔버스
            _skillCanvas.ShowSkill(template);
        }

        #region 유닛 리스트 만들기
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
                    _agentInfoItems[i].Show(template, owned, this);
                }
                else
                {
                    // 미보유 유닛
                    _agentInfoItems[i].Show(template, null, this);
                }
            }

            _agentInfoItems[0].OnClick();
        }
        #endregion
    }
}