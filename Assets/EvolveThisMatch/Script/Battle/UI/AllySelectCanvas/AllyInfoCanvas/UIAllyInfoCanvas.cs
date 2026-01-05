using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIAllyInfoCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
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

        private AgentUnit _agentUnit;
        private CrystalSystem _crystalSystem;

        private UIAllySelectCanvas _allySelectCanvas;
        private UIRarityTag _rarityTag;
        private UIJobTag _jobTag;
        private UIBattleSyncButton _syncButton;
        private UIBattleLimitButton _limitButton;
        private UIBattleStatCanvas _battleStatCanvas;
        private UISkillView _skillCanvas;

        protected override void Initialize()
        {
            _allySelectCanvas = GetComponentInParent<UIAllySelectCanvas>();
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _jobTag = GetComponentInChildren<UIJobTag>();
            _syncButton = GetComponentInChildren<UIBattleSyncButton>();
            _limitButton = GetComponentInChildren<UIBattleLimitButton>();
            _battleStatCanvas = GetComponentInChildren<UIBattleStatCanvas>();
            _skillCanvas = GetComponentInChildren<UISkillView>();

            _crystalSystem = BattleManager.Instance.GetSubSystem<CrystalSystem>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _fullBodyImage = GetImage((int)Images.FullBodyImage);
            _displayNameText = GetText((int)Texts.DisplayName);
            _synergyText = GetText((int)Texts.SynergyText);

            GetButton((int)Buttons.DestinyRecastButton).onClick.AddListener(DestinyRecast);
        }

        internal void Show(AgentUnit agentUnit)
        {
            _agentUnit = agentUnit;

            _fullBodyImage.sprite = agentUnit.template.sprite;
            _synergyText.text = agentUnit.template.synergy[0].displayName;
            _displayNameText.text = agentUnit.template.displayName;

            // 등급 태그
            _rarityTag.Show(agentUnit.template.rarity);

            // 직업 태그
            _jobTag.Show(agentUnit.template.job);

            // 레벨 버튼
            _syncButton.Show(agentUnit);

            // 재능 한계 버튼
            _limitButton.Show(agentUnit);

            // 스탯 캔버스
            _battleStatCanvas.Show(agentUnit);

            // 스킬 캔버스
            _skillCanvas.Show(agentUnit);

            Show(true);
        }

        internal void Show(SummonUnit summonUnit)
        {
            _fullBodyImage.sprite = summonUnit.template.sprite;
            _displayNameText.text = summonUnit.template.displayName;

            // 스탯 캔버스
            _battleStatCanvas.Show(summonUnit);

            // 스킬 캔버스
            //_skillCanvas.ShowSkill(summonUnit);

            Show(true);
        }

        internal void Hide()
        {
            ResetCanvas();

            _agentUnit = null;
        }

        private void ResetCanvas()
        {
            _syncButton.Hide();
            _limitButton.Hide();
            _skillCanvas.Hide(_agentUnit);
        }

        private void DestinyRecast()
        {
            UIPopupManager.Instance.ShowConfirmCancelPopup($"<sprite name=\"Crystal\">을 1개 사용하여\n유닛의 운명을 바꾸시겠습니까?", (isOn) =>
            {
                if (isOn && _crystalSystem.PayCrystal(1))
                {
                    ResetCanvas();

                    var agentData = _agentUnit.agentData;
                    _agentUnit.DestinyRecast();

                    _allySelectCanvas.DestinyRecast(agentData.agentUnit);
                }
            });
        }
    }
}