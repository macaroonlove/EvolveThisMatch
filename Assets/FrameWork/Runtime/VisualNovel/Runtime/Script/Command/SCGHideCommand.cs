using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SCGHideCommand : Command
    {
        private int id = -1;

        public SCGHideCommand(Episode episode)
        {
            string theme = (episode as SCGHideEpisode).theme;
            if (string.IsNullOrEmpty(theme) == false)
            {
                int.TryParse(theme, out id);
            }
        }

        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SCGHide(id);
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SCGHide(id);
            isComplete = true;
        }
    }
}