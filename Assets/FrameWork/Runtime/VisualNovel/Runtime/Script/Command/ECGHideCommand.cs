using System.Collections;

namespace FrameWork.VisualNovel
{
    public class ECGHideCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.ECGHide();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.ECGHide();
            isComplete = true;
        }
    }
}