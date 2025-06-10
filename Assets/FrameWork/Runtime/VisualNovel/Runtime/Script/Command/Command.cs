using System.Collections;
using System.Collections.Generic;

namespace FrameWork.VisualNovel
{
    public abstract class Command
    {
        public static LinkedCommand operator +(Command origin, Command append)
        {
            return new LinkedCommand(new List<Command> { origin, append });
        }

        internal bool isComplete;

        internal abstract IEnumerator Execute();

        internal abstract void ForceExecute();
    }

    public class LinkedCommand : Command
    {
        private List<Command> _commands;

        public LinkedCommand(List<Command> commands)
        {
            _commands = commands;
        }

        internal override IEnumerator Execute()
        {
            foreach (var command in _commands)
            {
                yield return command.Execute();
            }
            isComplete = true;
        }

        internal override void ForceExecute()
        {
            foreach (var command in _commands)
            {
                command.ForceExecute();
            }
            isComplete = true;
        }
    }

    public class StartCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.chapterState = ChapterState.Run;
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            isComplete = true;
        }
    }

    public class EndCommand : Command
    {
        internal override IEnumerator Execute()
        {
            CommandExecutor.Instance.chapterState = ChapterState.None;
            CommandExecutor.Instance.SpeakEnd();
            CommandExecutor.Instance.SCGHide(-1);
            CommandExecutor.Instance.BGMStop();
            CommandExecutor.Instance.SFXStop();
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            isComplete = true;
        }
    }

    public class EmptyCommand : Command
    {
        internal override IEnumerator Execute()
        {
            isComplete = true;
            yield return null;
        }

        internal override void ForceExecute()
        {
            isComplete = true;
        }
    }
}