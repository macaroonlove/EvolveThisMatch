using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentInfoItem : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Level,
            DisplayName,
            CounterText,
        }
        enum Images
        {
            Background,
            FullBody,
            CounterImage,
            Dim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _counterText;
        private Image _background;
        private Image _fullBody;
        private Image _counterImage;
        private Image _dim;
        private UIAgentTier _tierGroup;

        private UIAgentInfoCanvas _agentInfoCanvas;
        private AgentTemplate _template;
        private ProfileSaveData.Agent _owned;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _displayName = GetText((int)Texts.DisplayName);
            _counterText = GetText((int)Texts.CounterText);
            _background = GetImage((int)Images.Background);
            _fullBody = GetImage((int)Images.FullBody);
            _counterImage = GetImage((int)Images.CounterImage);
            _dim = GetImage((int)Images.Dim);

            _tierGroup = GetComponentInChildren<UIAgentTier>();

            var button = GetComponent<Button>();
            button?.onClick.AddListener(OnClick);
        }

        internal void Show(AgentTemplate template, ProfileSaveData.Agent owned, UIAgentInfoCanvas agentInfoCanvas)
        {
            _template = template;
            _owned = owned;
            _agentInfoCanvas = agentInfoCanvas;

            _displayName.text = template.displayName;
            _fullBody.sprite = template.sprite;
            _fullBody.rectTransform.anchoredPosition = template.faceCenterPosition + new Vector2(0, -100);
            _background.sprite = template.rarity.agentInfoSprite;

            if (owned == null)
            {
                _level.text = "미보유";
                _counterText.text = "0/1";
                _counterImage.fillAmount = 0;
                _dim.enabled = true;
                _tierGroup.Show(0);
                return;
            }

            int unitCount = owned.unitCount;
            int maxUnitCount = GameDataManager.Instance.profileSaveData.GetMaxUnitCountByTier(owned.tier);

            _dim.enabled = false;
            _level.text = $"Lv. {owned.level}";
            _counterText.text = $"{unitCount}/{maxUnitCount}";
            _counterImage.fillAmount = unitCount / maxUnitCount;
            _tierGroup.Show(owned.tier);
        }

        internal void OnClick()
        {
            _agentInfoCanvas?.Show(_template, _owned);
        }
    }
}