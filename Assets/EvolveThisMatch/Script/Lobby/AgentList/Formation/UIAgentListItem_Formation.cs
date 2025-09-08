using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListItem_Formation : UIAgentListItem
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Level,
            DisplayName,
        }
        enum Images
        {
            Background,
            FullBody,
            SelectDim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private TextMeshProUGUI _displayName;
        private Image _background;
        private Image _fullBody;
        private Image _selectDim;
        private UIAgentTier _tierGroup;

        internal override void Initialize(UnityAction<AgentTemplate, AgentSaveData.Agent> action = null)
        {
            base.Initialize(action);

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _displayName = GetText((int)Texts.DisplayName);
            _background = GetImage((int)Images.Background);
            _fullBody = GetImage((int)Images.FullBody);
            _selectDim = GetImage((int)Images.SelectDim);

            _tierGroup = GetComponentInChildren<UIAgentTier>();
        }

        internal override void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            if (owned == null)
            {
                gameObject.SetActive(false);
                return;
            }

            base.Show(template, owned);

            _displayName.text = template.displayName;
            _fullBody.sprite = template.sprite;
            _fullBody.rectTransform.anchoredPosition = template.faceCenterPosition + new Vector2(0, -100);
            _background.sprite = template.rarity.agentInfoSprite;

            _level.text = $"Lv. {owned.level}";
            _tierGroup.Show(owned.tier);
        }

        public override void Show(bool isForce = false)
        {
            gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
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