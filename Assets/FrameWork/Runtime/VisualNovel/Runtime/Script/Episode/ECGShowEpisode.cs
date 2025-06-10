using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class ECGShowEpisode : ThemeEpisode
    {
        public override CommandType command => CommandType.ECG_Show;
    }
}