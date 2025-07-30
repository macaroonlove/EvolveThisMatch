using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListItem_Department : UIAgentListItem
    {
        #region 바인딩
        enum Texts
        {
            Level,
        }
        enum Images
        {
            Background,
            FullBody,
            SelectDim,
        }
        #endregion

        private TextMeshProUGUI _level;
        private Image _background;
        private Image _fullBody;
        private Image _selectDim;

        internal override void Initialize(UnityAction<AgentTemplate, ProfileSaveData.Agent> action = null)
        {
            base.Initialize(action);

            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _level = GetText((int)Texts.Level);
            _background = GetImage((int)Images.Background);
            _fullBody = GetImage((int)Images.FullBody);
            _selectDim = GetImage((int)Images.SelectDim);
        }

        internal override void Show(AgentTemplate template, ProfileSaveData.Agent owned)
        {
            if (owned == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            base.Show(template, owned);
            
            _fullBody.sprite = template.sprite;
            _fullBody.rectTransform.anchoredPosition = template.faceCenterPosition + new Vector2(0, -40);
            _background.sprite = template.rarity.agentInfoSprite;
            _level.text = $"생산속도\n<color=white>+{owned.level}%</color>";
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