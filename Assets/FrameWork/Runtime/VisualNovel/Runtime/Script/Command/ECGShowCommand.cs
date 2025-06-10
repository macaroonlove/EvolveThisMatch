using System.Collections;

namespace FrameWork.VisualNovel
{
    public class ECGShowCommand : Command
    {
        private ECGShowEpisode _ecgShowEpisode;

        public ECGShowCommand(Episode episode)
        {
            _ecgShowEpisode = episode as ECGShowEpisode;
            AddressableAssetManager.Instance.GetSprite(_ecgShowEpisode.theme, null);
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.ECGShow(_ecgShowEpisode.theme);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.ECGShow(_ecgShowEpisode.theme);
            isComplete = true;
        }
    }
}