namespace EvolveThisMatch.Core
{
    public class UITypeTag : UITag
    {
        internal void Show(SkillTypeTemplate template)
        {
            _background.color = template.backgroundColor;
            _text.color = template.textColor;
            _text.text = template.displayName;

            base.Show(true);
        }
    }
}