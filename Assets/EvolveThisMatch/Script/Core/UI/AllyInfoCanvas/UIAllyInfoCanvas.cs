using FrameWork.UIBinding;
using System;
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
            DestinyRecastButton,
        }
        enum Texts
        {
            DisplayName,
            SynergyText,
        }
        enum Images
        {
            FullBodyImage,
        }
        #endregion

        private TextMeshProUGUI _displayNameText;
        private TextMeshProUGUI _synergyText;
        private Image _fullBodyImage;

        private AllyUnit _allyUnit;
        private UnitRayCastSystem _unitRayCastSystem;
        private AttackRangeRenderer _attackRangeRenderer;

        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIBattleLevelButton _levelButton;
        private UIBattleLimitButton _limitButton;
        private UIBattleStatCanvas _battleStatCanvas;
        private UISkillCanvas _skillCanvas;

        protected override void Initialize()
        {
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _levelButton = GetComponentInChildren<UIBattleLevelButton>();
            _limitButton = GetComponentInChildren<UIBattleLimitButton>();
            _battleStatCanvas = GetComponentInChildren<UIBattleStatCanvas>();
            _skillCanvas = GetComponentInChildren<UISkillCanvas>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _fullBodyImage = GetImage((int)Images.FullBodyImage);
            _displayNameText = GetText((int)Texts.DisplayName);
            _synergyText = GetText((int)Texts.SynergyText);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
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

                // 공격 범위
                _attackRangeRenderer.Show((int)Mathf.Clamp(agentUnit.template.AttackRange, 0, 4));

                // 등급 태그
                _rarityTag.Show(agentUnit.template.rarity);

                // 직업 태그
                _jobTag.Show(agentUnit.template.job);

                // 레벨 버튼
                _levelButton.Show(agentUnit);

                // 재능 한계 버튼
                _limitButton.Show(agentUnit);

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
            _levelButton.Hide();
            _limitButton.Hide();
            Hide(true);
        }

        private void DestinyRecast()
        {

        }
    }
}