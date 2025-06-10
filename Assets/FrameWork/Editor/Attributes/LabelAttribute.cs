using UnityEngine;

namespace FrameWork.Editor
{
    public class LabelAttribute : PropertyAttribute
    {
        public string label;

        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }
}