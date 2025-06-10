using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SCGMoveCommand : Command
    {
        private SCGMoveEpisode _scgMoveEpisode;

        public SCGMoveCommand(Episode episode)
        {
            _scgMoveEpisode = episode as SCGMoveEpisode;
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SCGMove(_scgMoveEpisode.id, _scgMoveEpisode.position, _scgMoveEpisode.anchor, _scgMoveEpisode.duration, _scgMoveEpisode.loopCount, _scgMoveEpisode.ease, _scgMoveEpisode.isReturn, _scgMoveEpisode.returnEase);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SCGMove(_scgMoveEpisode.id, _scgMoveEpisode.position, _scgMoveEpisode.anchor, _scgMoveEpisode.duration, _scgMoveEpisode.loopCount, _scgMoveEpisode.ease, _scgMoveEpisode.isReturn, _scgMoveEpisode.returnEase);
            isComplete = true;
        }
    }
}