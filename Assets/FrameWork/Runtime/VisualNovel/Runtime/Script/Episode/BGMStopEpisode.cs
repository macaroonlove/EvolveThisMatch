using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class BGMStopEpisode : Episode
    {
        public override CommandType command => CommandType.BGM_Stop;
    }
}