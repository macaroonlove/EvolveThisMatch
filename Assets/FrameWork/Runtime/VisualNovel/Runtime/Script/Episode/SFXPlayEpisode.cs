using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SFXPlayEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SFX_Play;
    }
}