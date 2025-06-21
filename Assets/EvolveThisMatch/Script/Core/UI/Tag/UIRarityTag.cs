namespace EvolveThisMatch.Core
{
    public class UIRarityTag : UITag
    {
        internal void Show(AgentRarityTemplate template)
        {
            _background.color = template.backgroundColor;
            _text.color = template.textColor;
            _text.text = template.displayName;
        }
    }
}