using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class BGMPlayCommand : Command
    {
        private BGMPlayEpisode _bgmPlayEpisode;

        public BGMPlayCommand(Episode episode)
        {
            _bgmPlayEpisode = episode as BGMPlayEpisode;
            AddressableAssetManager.Instance.GetAudioClip(_bgmPlayEpisode.theme, null);
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.BGMPlay(_bgmPlayEpisode.theme);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.BGMPlay(_bgmPlayEpisode.theme);
            isComplete = true;
        }
    }
}