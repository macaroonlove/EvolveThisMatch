using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
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
        }
        enum Buttons
        {
            CloseButton,
            StatButton,
            SynergyButton,
            SkillButton,
            LevelUpButton,
            TierUpButton,
            TalentButton,
        }
        enum CanvasGroup
        {
            StatPanel,
            SynergyPanel,
            Skillpanel,
            LevelUpPanel,
            TierUpPanel,
            TalentPanel,
            PowderVariableDisplay,
        }
        #endregion

        private Transform _parent;
        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIAgentTier _tierGroup;
        private UIGeneralStatCanvas _generalStatCanvas;
        private UISynergyItem _synergyItem;
        private UISkillCanvas_Lobby _skillCanvas;
        private UILevelupPanel _levelupPanel;
        private UITierUpPanel _tierUpPanel;
        private UITalentPanel _talentPanel;
        private UITalentFilterPanel _talentFileterPanel;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _level;
        private Image _fullBody;
        private CanvasGroupController[] _panels = new CanvasGroupController[6];
        private CanvasGroupController _PowderVariableDisplay;

        private List<UIAgentInfoItem> _agentInfoItems;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

            _parent = GetObject((int)Objects.Content).transform;
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _tierGroup = GetComponentInChildren<UIAgentTier>();
            _generalStatCanvas = GetComponentInChildren<UIGeneralStatCanvas>();
            _synergyItem = GetComponentInChildren<UISynergyItem>();
            _skillCanvas = GetComponentInChildren<UISkillCanvas_Lobby>();
            _levelupPanel = GetComponentInChildren<UILevelupPanel>();
            _tierUpPanel = GetComponentInChildren<UITierUpPanel>();
            _talentPanel = GetComponentInChildren<UITalentPanel>();
            _talentFileterPanel = GetComponentInChildren<UITalentFilterPanel>();
            _talentFileterPanel.Initialize(_talentPanel);

            _displayName = GetText((int)Texts.DisplayName);
            _level = GetText((int)Texts.Level);
            _fullBody = GetImage((int)Images.FullBody);

            _panels[0] = GetCanvasGroupController((int)CanvasGroup.StatPanel);
            _panels[1] = GetCanvasGroupController((int)CanvasGroup.SynergyPanel);
            _panels[2] = GetCanvasGroupController((int)CanvasGroup.Skillpanel);
            _panels[3] = GetCanvasGroupController((int)CanvasGroup.LevelUpPanel);
            _panels[4] = GetCanvasGroupController((int)CanvasGroup.TierUpPanel);
            _panels[5] = GetCanvasGroupController((int)CanvasGroup.TalentPanel);
            _PowderVariableDisplay = GetCanvasGroupController((int)CanvasGroup.PowderVariableDisplay);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.StatButton).onClick.AddListener(() => ShowPanel(0));
            GetButton((int)Buttons.SynergyButton).onClick.AddListener(() => ShowPanel(1));
            GetButton((int)Buttons.SkillButton).onClick.AddListener(() => ShowPanel(2));
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(() => ShowPanel(3));
            GetButton((int)Buttons.TierUpButton).onClick.AddListener(() => ShowPanel(4));
            GetButton((int)Buttons.TalentButton).onClick.AddListener(() => ShowPanel(5));
        }

        private void Start()
        {
            InitializeAgentInfoItem();

            _agentInfoItems[0].OnClick();
        }

        internal void Show(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            _displayName.text = template.displayName;
            _fullBody.sprite = template.sprite;

            if (owned != null)
            {
                // 유닛 레벨
                _level.text = $"Lv. {owned.level} / {GameDataManager.Instance.profileSaveData.GetMaxLevelByTier(owned.tier)}";

                // 유닛 티어
                _tierGroup.Show(owned.tier);
            }

            // 등급 태그
            _rarityTag.Show(template.rarity);

            // 직업 태그
            _jobTag.Show(template.job);

            // 스탯 캔버스
            _generalStatCanvas.ShowInfomation(template);

            // 시너지 적용
            _synergyItem.Show(template.synergy[0]);

            // 스킬 캔버스
            _skillCanvas.ShowSkill(template);

            // 레벨업 패널
            _levelupPanel.Show(owned, () => { ReShowAndSave(template, owned); });

            // 승격 패널
            _tierUpPanel.Show(owned, () => { ReShowAndSave(template, owned); });

            // 재능 패널
            _talentPanel.Show(owned, () => { Save(); _talentFileterPanel.Hide(true); }, () => { _talentFileterPanel.Show(); });
        }

        private void ShowPanel(int i)
        {
            if (i == 5) _PowderVariableDisplay.Show(true);
            else _PowderVariableDisplay.Hide(true);

            for (int j = 0; j < _panels.Length; j++)
            {
                if (i == j)
                    _panels[j].Show(true);
                else
                    _panels[j].Hide(true);
            }
        }

        private void ReShowAndSave(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            Show(template, owned);
            RegistAgentInfoItem();
            Save();
        }

        private void Save()
        {
            _ = SaveManager.Instance.Save_ProfileData();
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
        }
        #endregion
    }
}