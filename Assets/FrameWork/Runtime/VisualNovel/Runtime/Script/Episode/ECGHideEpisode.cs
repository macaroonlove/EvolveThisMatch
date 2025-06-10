using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class ECGHideEpisode : Episode
    {
        public override CommandType command => CommandType.ECG_Hide;
    }
}