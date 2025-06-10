using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class SFXStopCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.SFXStop();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.SFXStop();
            isComplete = true;
        }
    }
}