namespace EvolveThisMatch.Core
{
    public class UITypeTag : UITag
    {
        public void Show(ElementalTemplate template)
        {
            _background.color = template.backgroundColor;
            _text.color = template.textColor;
            _text.text = template.displayName;
        }
    }
}