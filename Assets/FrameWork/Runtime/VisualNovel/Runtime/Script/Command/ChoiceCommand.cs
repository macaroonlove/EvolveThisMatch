using System.Collections;
using System.Collections.Generic;

namespace FrameWork.VisualNovel
{
    public class ChoiceCommand : Command
    {
        private ChoiceEpisode _choiceEpisode;

        public ChoiceCommand(Episode episode)
        {
            _choiceEpisode = episode as ChoiceEpisode;
            int choiceCount = _choiceEpisode.choiceList.Count;
            
            foreach (var template in _choiceEpisode.choiceList)
            {
                AddressableAssetManager.Instance.GetScriptableObject<ChapterTemplate>(template.nextChapter, null);
            }
        }

        internal override IEnumerator Execute()
        {
            if (_choiceEpisode.choiceList.Count == 0)
            {
                isComplete = true;
                yield break;
            }

            CommandExecutor.Instance.SetNextButtonInteractable(false);

            List<KeyValuePair<string, ChapterTemplate>> choiceList = new List<KeyValuePair<string, ChapterTemplate>>();

            foreach (var template in _choiceEpisode.choiceList)
            {
                AddressableAssetManager.Instance.GetScriptableObject<ChapterTemplate>(template.nextChapter, (nextChapter) =>
                {
                    choiceList.Add(new KeyValuePair<string, ChapterTemplate>(template.choice, nextChapter));
                });
            }

            CommandExecutor.Instance.Choice(choiceList, OnComplete);

            while (isComplete == false)
            {
                yield return null;
            }
        }

        internal override void ForceExecute()
        {

        }

        private void OnComplete()
        {
            isComplete = true;
            CommandExecutor.Instance.SetNextButtonInteractable(true);
        }
    }
}