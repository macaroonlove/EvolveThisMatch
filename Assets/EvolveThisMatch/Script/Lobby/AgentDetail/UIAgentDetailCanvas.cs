using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentDetailCanvas : UIBase
    {
        #region 바인딩
        enum Texts
        {
            DisplayName,
            Level,
        }
        enum Buttons
        {
            StatButton,
            SynergyButton,
            SkillButton,
            LevelUpButton,
            TierUpButton,
            TalentButton,
        }
        enum CanvasGroup
        {
            StatView,
            SynergyView,
            SkillView,
            LevelUpView,
            TierUpView,
            TalentView,
        }
        #endregion

        private UIAgentDetailController _controller;

        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _level;

        #region 자식 View
        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIAgentTier _tierGroup;
        private UIGeneralStatCanvas _generalStatCanvas;
        private UISynergyItem _synergyItem;
        private UISkillView _skillView;
        private UILevelUpView _levelUpView;
        private UITierUpView _tierUpView;
        private UITalentView _talentView;
        private UITalentFilterView _talentFileterView;
        #endregion

        private CanvasGroupController[] _panels = new CanvasGroupController[6];

        protected override void Initialize()
        {
            #region 자식 View 받아오기
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _tierGroup = GetComponentInChildren<UIAgentTier>();
            _generalStatCanvas = GetComponentInChildren<UIGeneralStatCanvas>();
            _synergyItem = GetComponentInChildren<UISynergyItem>();
            _skillView = GetComponentInChildren<UISkillView>();
            _levelUpView = GetComponentInChildren<UILevelUpView>();
            _tierUpView = GetComponentInChildren<UITierUpView>();
            _talentView = GetComponentInChildren<UITalentView>();
            _talentFileterView = GetComponentInChildren<UITalentFilterView>();
            _talentView.Bind(_talentFileterView);
            #endregion

            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

            _displayName = GetText((int)Texts.DisplayName);
            _level = GetText((int)Texts.Level);

            _panels[0] = GetCanvasGroupController((int)CanvasGroup.StatView);
            _panels[1] = GetCanvasGroupController((int)CanvasGroup.SynergyView);
            _panels[2] = GetCanvasGroupController((int)CanvasGroup.SkillView);
            _panels[3] = GetCanvasGroupController((int)CanvasGroup.LevelUpView);
            _panels[4] = GetCanvasGroupController((int)CanvasGroup.TierUpView);
            _panels[5] = GetCanvasGroupController((int)CanvasGroup.TalentView);

            GetButton((int)Buttons.StatButton).onClick.AddListener(() => _controller.SelectPanel(0));
            GetButton((int)Buttons.SynergyButton).onClick.AddListener(() => _controller.SelectPanel(1));
            GetButton((int)Buttons.SkillButton).onClick.AddListener(() => _controller.SelectPanel(2));
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(() => _controller.SelectPanel(3));
            GetButton((int)Buttons.TierUpButton).onClick.AddListener(() => _controller.SelectPanel(4));
            GetButton((int)Buttons.TalentButton).onClick.AddListener(() => _controller.SelectPanel(5));

            _controller = new UIAgentDetailController(this);
        }

        internal void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            _controller.Show(template, owned);
        }

        internal void Hide()
        {
            _controller.Hide();
        }

        internal void SelectFirstPanel()
        {
            _controller.SelectPanel(0);
            _controller.Refresh();
        }

        internal void ShowTemplate(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 유닛 이름
            _displayName.text = template.displayName;

            // 등급 태그
            _rarityTag.Show(template.rarity);

            // 직업 태그
            _jobTag.Show(template.job);

            // 스탯
            _generalStatCanvas.Show(template, owned);
        }

        internal void ShowOwnedData(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 유닛 레벨
            _level.text = $"Lv. {owned.level} / {SaveManager.Instance.agentData.GetMaxLevelByTier(owned.tier)}";

            // 유닛 티어
            _tierGroup.Show(owned.tier);

            // 시너지 적용
            _synergyItem.Show(template.synergy[0]);

            // 스킬 뷰
            _skillView.Show(template);

            // 레벨업 뷰
            _levelUpView.Show(owned, () => _controller.Refresh());

            // 승격 뷰
            _tierUpView.Show(owned, () => _controller.Refresh());

            // 재능 뷰
            _talentView.Show(owned);
        }

        internal void ShowPanel(int i)
        {
            for (int j = 0; j < _panels.Length; j++)
            {
                if (i == j) _panels[j].Show(true);
                else _panels[j].Hide(true);
            }
        }

        internal void Clear()
        {
            _talentView?.ClearRNG();
        }
    }
}