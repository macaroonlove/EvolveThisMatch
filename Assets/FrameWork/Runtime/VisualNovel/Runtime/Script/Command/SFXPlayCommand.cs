using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SFXPlayCommand : Command
    {
        private SFXPlayEpisode _sfxPlayEpisode;

        public SFXPlayCommand(Episode episode)
        {
            _sfxPlayEpisode = episode as SFXPlayEpisode;
            AddressableAssetManager.Instance.GetAudioClip(_sfxPlayEpisode.theme, null);
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SFXPlay(_sfxPlayEpisode.theme);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SFXPlay(_sfxPlayEpisode.theme);
            isComplete = true;
        }
    }
}