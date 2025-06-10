using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class SFXStopEpisode : Episode
    {
        public override CommandType command => CommandType.SFX_Stop;
    }
}