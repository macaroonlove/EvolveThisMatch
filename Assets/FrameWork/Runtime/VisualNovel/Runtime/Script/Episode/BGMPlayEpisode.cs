using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class BGMPlayEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.BGM_Play;
    }
}