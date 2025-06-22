using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class AllyInfoCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            LevelUpgradeButton,
            LimitUpgradeButton,
            DestinyRecastButton,
        }
        enum Texts
        {
            DisplayName,
            LevelText,
            LimitText,
            SynergyText,
        }
        enum Images
        {
            FullBodyImage,
        }
        #endregion

        private TextMeshProUGUI _displayNameText;
        private TextMeshProUGUI _levelText;
        private TextMeshProUGUI _limitText;
        private TextMeshProUGUI _synergyText;
        private Image _fullBodyImage;

        private AllyUnit _allyUnit;
        private UnitRayCastSystem _unitRayCastSystem;
        private AttackRangeRenderer _attackRangeRenderer;

        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIBattleStatCanvas _battleStatCanvas;
        private UISkillCanvas _skillCanvas;

        protected override void Initialize()
        {
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _battleStatCanvas = GetComponentInChildren<UIBattleStatCanvas>();
            _skillCanvas = GetComponentInChildren<UISkillCanvas>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _fullBodyImage = GetImage((int)Images.FullBodyImage);
            _displayNameText = GetText((int)Texts.DisplayName);
            _levelText = GetText((int)Texts.LevelText);
            _limitText = GetText((int)Texts.LimitText);
            _synergyText = GetText((int)Texts.SynergyText);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.LevelUpgradeButton).onClick.AddListener(UpgradeLevel);
            GetButton((int)Buttons.LimitUpgradeButton).onClick.AddListener(UpgradeLimit);
            GetButton((int)Buttons.DestinyRecastButton).onClick.AddListener(DestinyRecast);

            BattleManager.Instance.onBattleInitialize += OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy += OnUnsubscribe;
        }

        private void OnBattleInitialize()
        {
            _unitRayCastSystem = BattleManager.Instance.GetSubSystem<UnitRayCastSystem>();
            _attackRangeRenderer = BattleManager.Instance.GetSubSystem<AttackRangeRenderer>();

            _unitRayCastSystem.onCast += ShowInfomation;
        }

        private void OnBattleDeinitialize()
        {
            _unitRayCastSystem.onCast -= ShowInfomation;

            _unitRayCastSystem = null;
        }

        private void OnUnsubscribe()
        {
            BattleManager.Instance.onBattleInitialize -= OnBattleInitialize;
            BattleManager.Instance.onBattleDeinitialize -= OnBattleDeinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= OnUnsubscribe;
        }

        internal void ShowInfomation(AllyUnit allyUnit)
        {
            _allyUnit = allyUnit;

            if (_allyUnit is AgentUnit agentUnit)
            {
                _fullBodyImage.sprite = agentUnit.template.sprite;
                _synergyText.text = agentUnit.template.synergy[0].displayName;
                _displayNameText.text = agentUnit.template.displayName;
                _levelText.text = agentUnit.level.ToString();
                _limitText.text = agentUnit.limit.ToString();

                // 공격 범위
                _attackRangeRenderer.Show((int)Mathf.Clamp(agentUnit.template.AttackRange, 0, 4));

                // 등급 태그
                _rarityTag.Show(agentUnit.template.rarity);

                // 직업 태그
                _jobTag.Show(agentUnit.template.job);

                // 스탯 캔버스
                _battleStatCanvas.ShowInfomation(agentUnit);

                // 스킬 캔버스
                _skillCanvas.ShowSkill(agentUnit);
            }

            Show(true);
        }

        private void Hide()
        {
            _allyUnit = null;
            _attackRangeRenderer.Hide();
            Hide(true);
        }

        private void UpgradeLevel()
        {
            // TODO: 조건 추가
            if (_allyUnit is AgentUnit agentUnit)
            {
                agentUnit.UpgradeLevel();
            }
        }

        private void UpgradeLimit()
        {
            // TODO: 조건 추가
            if (_allyUnit is AgentUnit agentUnit)
            {
                agentUnit.UpgradeLimit();
            }
        }

        private void DestinyRecast()
        {

        }
    }
}