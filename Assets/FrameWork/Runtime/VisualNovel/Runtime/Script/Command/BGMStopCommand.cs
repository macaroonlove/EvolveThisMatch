using FrameWork.Sound;
using System.Collections;

namespace FrameWork.VisualNovel
{
    public class BGMStopCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.BGMStop();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            CommandExecutor.Instance.BGMStop();
            isComplete = true;
        }
    }
}