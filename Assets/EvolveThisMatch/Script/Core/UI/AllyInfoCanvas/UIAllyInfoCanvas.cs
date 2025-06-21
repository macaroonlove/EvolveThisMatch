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
        }

        enum Images
        {
            FullBodyImage,
        }
        #endregion

        private TextMeshProUGUI _displayNameText;
        private TextMeshProUGUI _levelText;
        private TextMeshProUGUI _limitText;
        private Image _fullBodyImage;

        private AllyUnit _allyUnit;
        private UnitRayCastSystem _unitRayCastSystem;
        private AttackRangeRenderer _attackRangeRenderer;
        
        private UIBattleStatCanvas _battleStatCanvas;

        protected override void Initialize()
        {
            _battleStatCanvas = GetComponentInChildren<UIBattleStatCanvas>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _fullBodyImage = GetImage((int)Images.FullBodyImage);
            _displayNameText = GetText((int)Texts.DisplayName);
            _levelText = GetText((int)Texts.LevelText);
            _limitText = GetText((int)Texts.LimitText);

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
                _displayNameText.text = agentUnit.template.displayName;
                _levelText.text = agentUnit.level.ToString();
                _limitText.text = agentUnit.limit.ToString();

                _attackRangeRenderer.Show((int)Mathf.Clamp(agentUnit.template.AttackRange, 0, 4));
                _battleStatCanvas.ShowInfomation(agentUnit);
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