using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SpeakStartCommand : Command
    {
        private SpeakStartEpisode _speakStartEpisode;

        public SpeakStartCommand(Episode episode)
        {
            _speakStartEpisode = episode as SpeakStartEpisode;
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.Speak(_speakStartEpisode.theme, _speakStartEpisode.context, OnComplete);
            while (isComplete == false)
            {
                yield return null;
            }
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.Speak(_speakStartEpisode.theme, _speakStartEpisode.context, OnComplete, true);
            isComplete = true;
        }

        private void OnComplete()
        {
            isComplete = true;
        }
    }
}