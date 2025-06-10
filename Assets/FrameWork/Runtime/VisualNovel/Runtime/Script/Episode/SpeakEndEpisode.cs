using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SpeakEndEpisode : Episode
    {
        public override CommandType command => CommandType.Speak_End;
    }
}