using EvolveThisMatch.Core;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UISkillSlot_Lobby : UISkillSlot
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Icon,
        }
        #endregion

        private Image _icon;

        protected override void Initialize()
        {
            base.Initialize();

            BindImage(typeof(Images));

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