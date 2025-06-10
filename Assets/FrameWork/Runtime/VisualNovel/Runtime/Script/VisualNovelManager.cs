using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public class VisualNovelManager : Singleton<VisualNovelManager>
    {
        private Dictionary<string, List<Command>> _chapter = new Dictionary<string, List<Command>>();

        public void Load(ChapterTemplate template)
        {
            // 새롭게 로드된 챕터라면
            if (_chapter.ContainsKey(template.title) == false)
            {
                List<Command> commands = Convert(template);
                _chapter.Add(template.title, commands);
            }

            CommandExecutor.Instance.StartChapter(_chapter[template.title], () =>
            {
                if (template.nextChapter != null)
                {
                    Load(template.nextChapter);
                }
            });
        }

        private List<Command> Convert(ChapterTemplate template)
        {
            List<Command> commands = new List<Command>();

            Command command = new StartCommand();

            bool isGroup = false;

            foreach (var episode in template.episodes)
            {
                if (command == null)
                {
                    command = new EmptyCommand();
                }

                switch (episode.command)
                {
                    case CommandType.CommandGroup_Start:
                        isGroup = true;
                        break;
                    case CommandType.CommandGroup_End:
                        isGroup = false;
                        break;
                    case CommandType.Speak_Start:
                        command += new SpeakStartCommand(episode);
                        break;
                    case CommandType.Speak_End:
                        command += new SpeakEndCommand();
                        break;
                    case CommandType.SCG_Show:
                        command += new SCGShowCommand(episode);
                        break;
                    case CommandType.SCG_Hide:
                        command += new SCGHideCommand(episode);
                        break;
                    case CommandType.SCG_Move:
                        command += new SCGMoveCommand(episode);
                        break;
                    case CommandType.ECG_Show:
                        command += new ECGShowCommand(episode);
                        break;
                    case CommandType.ECG_Hide:
                        command += new ECGHideCommand();
                        break;
                    case CommandType.BGM_Play:
                        command += new BGMPlayCommand(episode);
                        break;
                    case CommandType.BGM_Stop:
                        command += new BGMStopCommand();
                        break;
                    case CommandType.SFX_Play:
                        command += new SFXPlayCommand(episode);
                        break;
                    case CommandType.SFX_Stop:
                        command += new SFXStopCommand();
                        break;
                    case CommandType.Choice:
                        command += new ChoiceCommand(episode);
                        break;
                    case CommandType.Get:
                        command += new GetCommand(episode);
                        break;
                }

                if (isGroup == false)
                {
                    commands.Add(command);
                    command = null;
                }
            }

            if (command != null)
            {
                commands.Add(command);
            }

            commands.Add(new EndCommand());

            return commands;
        }
    }
}