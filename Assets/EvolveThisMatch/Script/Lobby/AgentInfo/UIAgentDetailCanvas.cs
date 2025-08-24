using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UI;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        enum Images
        {
            FullBody,
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
            StatPanel,
            SynergyPanel,
            Skillpanel,
            LevelUpPanel,
            TierUpPanel,
            TalentPanel,
        }
        #endregion

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
        
        private PoolSystem _poolSystem;
        private GameObject _spawnObj;

        private CanvasGroupController[] _panels = new CanvasGroupController[6];

        private AgentSaveData.Agent _owned;
        private UnityAction _action;

        internal void Initialize(UnityAction action = null)
        {
            _action = action;
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroup));

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

            GetButton((int)Buttons.StatButton).onClick.AddListener(() => ShowPanel(0));
            GetButton((int)Buttons.SynergyButton).onClick.AddListener(() => ShowPanel(1));
            GetButton((int)Buttons.SkillButton).onClick.AddListener(() => ShowPanel(2));
            GetButton((int)Buttons.LevelUpButton).onClick.AddListener(() => ShowPanel(3));
            GetButton((int)Buttons.TierUpButton).onClick.AddListener(() => ShowPanel(4));
            GetButton((int)Buttons.TalentButton).onClick.AddListener(() => ShowPanel(5));
        }

        internal void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            if (owned == null) ShowPanel(0);

            if (_spawnObj != null)
            {
                _poolSystem.DeSpawn(_spawnObj);
                _spawnObj = null;
            }
            if (template.overUIPrefab != null)
            {
                _spawnObj = _poolSystem.Spawn(template.overUIPrefab);
                _spawnObj.transform.position = new Vector2(-1.5f, -4);
            }

            _owned = owned;
            _displayName.text = template.displayName;
            _fullBody.sprite = template.sprite;

            // 등급 태그
            _rarityTag.Show(template.rarity);

            // 직업 태그
            _jobTag.Show(template.job);

            // 스탯 캔버스
            _generalStatCanvas.ShowInfomation(template);

            if (owned != null)
            {
                // 유닛 레벨
                _level.text = $"Lv. {owned.level} / {SaveManager.Instance.agentData.GetMaxLevelByTier(owned.tier)}";

                // 유닛 티어
                _tierGroup.Show(owned.tier);

                // 시너지 적용
                _synergyItem.Show(template.synergy[0]);

                // 스킬 캔버스
                _skillCanvas.ShowSkill(template);

                // 레벨업 패널
                _levelupPanel.Show(owned, () => { ReShow(template, owned); });

                // 승격 패널
                _tierUpPanel.Show(owned, () => { ReShow(template, owned); });

                // 재능 패널
                _talentPanel.Show(owned, () => { _talentFileterPanel.Hide(true); }, () => { _talentFileterPanel.Show(); });
            }
        }

        private void ShowPanel(int i)
        {
            if (_owned == null) return;

            VariableDisplayManager.Instance.HideAll();

            if (i == 5) VariableDisplayManager.Instance.Show(CurrencyType.Powder);

            for (int j = 0; j < _panels.Length; j++)
            {
                if (i == j)
                    _panels[j].Show(true);
                else
                    _panels[j].Hide(true);
            }
        }

        internal void HidePanel()
        {
            if (_spawnObj != null)
            {
                _poolSystem.DeSpawn(_spawnObj);
                _spawnObj = null;
            }
        }

        private void ReShow(AgentTemplate template, AgentSaveData.Agent owned)
        {
            Show(template, owned);
            _action?.Invoke();
        }
    }
}