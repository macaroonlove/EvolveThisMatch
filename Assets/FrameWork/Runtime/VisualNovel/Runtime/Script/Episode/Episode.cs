using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public enum CommandType
    {
        CommandGroup_Start, // 커맨드 묶기 시작
        CommandGroup_End,   // 커맨드 묶기 종료
        Speak_Start, // 대화 시작
        Speak_End, // 대화 종료
        SCG_Show, // 캐릭터나 물체 일러스트를 보이게 하기
        SCG_Hide, // 캐릭터나 물체 일러스트를 숨기기
        SCG_Move, // 캐릭터나 물체 일러스트에 애니메이션 적용하기
        ECG_Show, // 배경 일러스트를 보이게 하기
        ECG_Hide, // 배경 일러스트를 숨기기
        BGM_Play, // 배경음 재생
        BGM_Stop, // 배경음 정지
        SFX_Play, // 효과음 재생
        SFX_Stop, // 효과음 정지
        Choice,   // 선택지
        Get,      // 아이템 획득
    }

    [Serializable]
    public abstract class Episode : ScriptableObject
    {
        public abstract CommandType command { get; }

#if UNITY_EDITOR
        public virtual void Draw(Rect rect)
        {
            var commandRect = new Rect(rect.x, rect.y + 4, 125, 20);
            EditorGUI.LabelField(commandRect, command.ToString());
        }

        public virtual int GetHeight()
        {
            return 1;
        }
#endif
    }

    [Serializable]
    public abstract class ThemeEpisode : Episode
    {
        public string theme;

        public void Initialize(string theme)
        {
            this.theme = theme;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            var themeRect = new Rect(rect.x + 130, rect.y + 4, 190, 18);
            theme = EditorGUI.TextField(themeRect, theme);
        }
#endif
    }
}