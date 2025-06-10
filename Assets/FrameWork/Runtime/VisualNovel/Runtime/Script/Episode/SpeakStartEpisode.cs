using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SpeakStartEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.Speak_Start;
        public string context;

        public void Initialize(string theme, string context)
        {
            base.Initialize(theme);

            this.context = context;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            context = EditorGUI.TextArea(new Rect(rect.x + 330, rect.y + 4, rect.width - 330, 60), context);
        }

        public override int GetHeight()
        {
            return 3;
        }
#endif
    }
}