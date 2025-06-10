using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SCGHideEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.SCG_Hide;
    }
}