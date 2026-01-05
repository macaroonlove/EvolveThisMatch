using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UISkillSlot : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            SkillName,
            SkillDescription,
        }
        enum Images
        {
            Icon,
        }
        #endregion

        private TextMeshProUGUI _skillName;
        private TextMeshProUGUI _skillDescription;
        private Image _icon;

        private UITypeTag _typeTag;

        protected override void Initialize()
        {
            _typeTag = GetComponentInChildren<UITypeTag>();
            BindText(typeof(Texts));
            BindImage(typeof(Images));

            _skillName = GetText((int)Texts.SkillName);
            _skillDescription = GetText((int)Texts.SkillDescription);
            _icon = GetImage((int)Images.Icon);
        }

        internal void ShowSkillSlot(SkillTemplate template)
        {
            _skillName.text = template.displayName;
            _skillDescription.text = template.description;

            _icon.sprite = template.sprite;

            if (template is ActiveSkillTemplate activeSkillTemplate)
            {
                _typeTag.Show(activeSkillTemplate.skillType);
                _typeTag.gameObject.SetActive(true);
            }
            else
            {
                _typeTag.gameObject.SetActive(false);
            }

            base.Show(true);
        }
    }
}