using System.Collections;

namespace FrameWork.VisualNovel
{
    public class GetCommand : Command
    {
        private GetEpisode _getEpisode;

        public GetCommand(Episode episode)
        {
            _getEpisode = episode as GetEpisode;
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.Get(_getEpisode.itemType, _getEpisode.itemAmount, _getEpisode.name);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            isComplete = true;
        }
    }
}