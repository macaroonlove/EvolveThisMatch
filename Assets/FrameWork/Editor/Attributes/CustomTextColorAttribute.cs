using UnityEngine;

namespace FrameWork.Editor
{
    public class TextColorUsageAttribute : PropertyAttribute
    {
        public Color color;

        public TextColorUsageAttribute(int red, int green, int blue)
        {
            color = new Color(red / 255f, green / 255f, blue / 255f);
        }
    }
}