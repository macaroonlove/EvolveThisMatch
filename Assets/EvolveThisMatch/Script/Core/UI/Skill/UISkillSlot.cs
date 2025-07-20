using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public abstract class UISkillSlot : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            SkillName,
            SkillDescription,
        }
        #endregion

        protected TextMeshProUGUI _skillName;
        protected TextMeshProUGUI _skillDescription;
        
        protected UITypeTag _typeTag;

        protected override void Initialize()
        {
            _typeTag = GetComponentInChildren<UITypeTag>();

            BindText(typeof(Texts));

            _skillName = GetText((int)Texts.SkillName);
            _skillDescription = GetText((int)Texts.SkillDescription);
        }
    }
}