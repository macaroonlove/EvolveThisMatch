using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class CommandGroupStartEpisode : Episode
    {
        public override CommandType command => CommandType.CommandGroup_Start;
    }
}