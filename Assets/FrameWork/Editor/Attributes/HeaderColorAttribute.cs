using UnityEngine;

namespace FrameWork.Editor
{
    public class HeaderColorAttribute : PropertyAttribute
    {
        public string header;
        public Color color;

        public HeaderColorAttribute(string header, int red, int green, int blue)
        {
            this.header = header;
            color = new Color(red / 255f, green / 255f, blue / 255f);
        }
    }
}