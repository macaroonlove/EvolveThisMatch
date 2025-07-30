using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListItem_AgentInfo : UIAgentListItem
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
            SelectDim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _counterText;
        private Image _background;
        private Image _fullBody;
        private Image _counterImage;
        private Image _dim;
        private Image _selectDim;
        private UIAgentTier _tierGroup;

        internal override void Initialize(UnityAction<AgentTemplate, ProfileSaveData.Agent> action = null)
        {
            base.Initialize(action);

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _displayName = GetText((int)Texts.DisplayName);
            _counterText = GetText((int)Texts.CounterText);
            _background = GetImage((int)Images.Background);
            _fullBody = GetImage((int)Images.FullBody);
            _counterImage = GetImage((int)Images.CounterImage);
            _dim = GetImage((int)Images.Dim);
            _selectDim = GetImage((int)Images.SelectDim);

            _tierGroup = GetComponentInChildren<UIAgentTier>();
        }

        internal override void Show(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            base.Show(template, owned);

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

            if (maxUnitCount == -1)
            {
                _counterText.text = $"{unitCount}";
                _counterImage.fillAmount = 1;
            }
            else
            {
                _counterText.text = $"{unitCount}/{maxUnitCount}";
                _counterImage.fillAmount = unitCount / maxUnitCount;
            }

            _dim.enabled = false;
            _level.text = $"Lv. {owned.level}";
            _tierGroup.Show(owned.tier);
        }

        internal override void SelectItem()
        {
            base.SelectItem();

            _selectDim.enabled = true;
        }

        internal override void DeSelectItem()
        {
            _selectDim.enabled = false;
        }
    }
}