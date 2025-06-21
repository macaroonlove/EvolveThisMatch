using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIJobTag : UITag
    {
        internal void Show(JobTemplate template)
        {
            _background.color = template.backgroundColor;
            _text.color = template.textColor;
            _text.text = template.displayName;
        }
    }
}
