using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SpeakEndCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SpeakEnd();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SpeakEnd();
            isComplete = true;
        }
    }
}