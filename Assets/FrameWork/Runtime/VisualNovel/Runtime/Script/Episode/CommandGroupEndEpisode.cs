using System;

namespace FrameWork.VisualNovel
{
    [Serializable]
    public class CommandGroupEndEpisode : Episode
    {
        public override CommandType command => CommandType.CommandGroup_End;
    }
}