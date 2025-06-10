using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SCGShowCommand : Command
    {
        private SCGShowEpisode _scgShowEpisode;

        public SCGShowCommand(Episode episode)
        {
            _scgShowEpisode = episode as SCGShowEpisode;
            AddressableAssetManager.Instance.GetSprite(_scgShowEpisode.theme, null);
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SCGShow(_scgShowEpisode.id, _scgShowEpisode.theme, _scgShowEpisode.position, _scgShowEpisode.anchor);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SCGShow(_scgShowEpisode.id, _scgShowEpisode.theme, _scgShowEpisode.position, _scgShowEpisode.anchor);
            isComplete = true;
        }
    }
}