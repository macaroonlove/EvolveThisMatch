using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel.Editor
{
#if UNITY_EDITOR
    public class InputDialogEditorWindow : EditorWindow
    {
        private string _massageText;
        private string _inputText;
        private Action<string> _onConfirm;

        public static void Show(string title, string massage, Action<string> onConfirm)
        {
            InputDialogEditorWindow window = CreateInstance<InputDialogEditorWindow>();
            window.titleContent = new GUIContent(title);
            window._massageText = massage;
            window._onConfirm = onConfirm;
            var size = new Vector2(300, 80);
            window.minSize = size;
            window.maxSize = size;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.Label(_massageText, EditorStyles.wordWrappedLabel);
            _inputText = EditorGUILayout.TextField(_inputText);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("확인"))
            {
                _onConfirm?.Invoke(_inputText);
                Close();
            }

            if (GUILayout.Button("취소"))
            {
                Close();
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
}