using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel.Editor
{
    public class VisualNovelEditorWindow : EditorWindow
    {
        private int _selectedStoryIndex = 0;
        private Vector2 _storyScrollPosition;
        private List<string> _storyDirectories = new List<string>();
        private const string _storyRootPath = "Assets/FrameWork/Runtime/VisualNovel/Runtime/Template/";

        private UnityEditor.Editor _chapterEditor;
        private int _selectedChapterIndex = 0;
        private Vector2 _chapterScrollPosition;
        private List<ChapterTemplate> _chapterTemplates = new List<ChapterTemplate>();

        private Vector2 _contentScrollPosition;

        [MenuItem("Window/비주얼 노벨 관리 시스템")]
        public static void Open()
        {
            var window = GetWindow<VisualNovelEditorWindow>();
            window.titleContent = new GUIContent("비주얼 노벨 관리");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            DrawStory();
        }

        private void DrawStory()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("스토리 폴더 추가"))
            {
                AddStoryDirectory();
            }
            if (GUILayout.Button("스토리 폴더 삭제"))
            {
                DeleteSelectedStoryDirectory();
            }
            if (GUILayout.Button("스토리 탐색"))
            {
                LoadStoryDirectories();
            }

            DrawLine();

            _storyScrollPosition = GUILayout.BeginScrollView(_storyScrollPosition, false, true);

            var catalog = new GUIStyle(GUI.skin.button);
            catalog.alignment = TextAnchor.MiddleLeft;
            catalog.padding = new RectOffset(5, 5, 5, 5);
            catalog.margin = new RectOffset(5, 5, -2, -2);
            catalog.border = new RectOffset(0, 0, 0, 0);
            catalog.fixedWidth = GUI.skin.box.fixedWidth;
            catalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < _storyDirectories.Count; i++)
            {
                bool isSelected = (_selectedStoryIndex == i);
                string text = Path.GetFileName(_storyDirectories[i]);

                if (GUILayout.Toggle(isSelected, text, catalog))
                {
                    if (_selectedStoryIndex != i)
                    {
                        _selectedStoryIndex = i;
                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (_storyDirectories.Count > 0 && _selectedStoryIndex < _storyDirectories.Count)
            {
                DrawChapter();
                LoadChapterTemplates();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddStoryDirectory()
        {
            InputDialogEditorWindow.Show("새 스토리 추가", "스토리 이름을 입력하세요.", newFolderName =>
            {
                string newFolderPath = Path.Combine(_storyRootPath, newFolderName);

                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(_storyRootPath.TrimEnd('/'), newFolderName);
                    AssetDatabase.Refresh();
                    LoadStoryDirectories();
                }
                else
                {
                    EditorUtility.DisplayDialog("이미 존재하는 스토리 이름입니다.", "폴더를 생성하는데 실패했습니다.", "확인");
                }
            });
        }

        private void DeleteSelectedStoryDirectory()
        {
            if (_selectedStoryIndex < 0 || _selectedStoryIndex >= _storyDirectories.Count) return;

            string folderPath = _storyDirectories[_selectedStoryIndex];
            bool confirm = EditorUtility.DisplayDialog("선택된 스토리 삭제", $"정말로 '{Path.GetFileName(folderPath)}' 스토리를 삭제하시겠습니까?", "삭제", "취소");

            if (confirm)
            {
                FileUtil.DeleteFileOrDirectory(folderPath);
                AssetDatabase.Refresh();
                LoadStoryDirectories();
            }
        }

        private void LoadStoryDirectories()
        {
            _storyDirectories.Clear();

            string[] directories = Directory.GetDirectories(_storyRootPath);
            foreach (var directory in directories)
            {
                _storyDirectories.Add(directory);
            }
        }

        #region 챕터
        private void DrawChapter()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("챕터 추가"))
            {
                AddChapterTemplate();
            }
            if (GUILayout.Button("챕터 삭제"))
            {
                DeleteSelectedChapterTemplate();
            }

            DrawLine();

            _chapterScrollPosition = GUILayout.BeginScrollView(_chapterScrollPosition, false, true);

            var catalog = new GUIStyle(GUI.skin.button);
            catalog.alignment = TextAnchor.MiddleLeft;
            catalog.padding = new RectOffset(5, 5, 5, 5);
            catalog.margin = new RectOffset(5, 5, -2, -2);
            catalog.border = new RectOffset(0, 0, 0, 0);
            catalog.fixedWidth = GUI.skin.box.fixedWidth;
            catalog.fixedHeight = GUI.skin.box.fixedHeight;

            for (int i = 0; i < _chapterTemplates.Count; i++)
            {
                bool isSelected = (_selectedChapterIndex == i);

                var text = _chapterTemplates[i].title;
                text = text.Substring(0, Mathf.Min(text.Length, 13));

                if (GUILayout.Toggle(isSelected, text, catalog))
                {
                    if (_selectedChapterIndex != i)
                    {
                        _selectedChapterIndex = i;

                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (_chapterTemplates.Count > 0 && _selectedChapterIndex < _chapterTemplates.Count)
            {
                ChapterTemplate template = _chapterTemplates[_selectedChapterIndex];

                if (_chapterEditor == null || _chapterEditor.target != template)
                {
                    _chapterEditor = UnityEditor.Editor.CreateEditor(template);
                }

                _contentScrollPosition = GUILayout.BeginScrollView(_contentScrollPosition, false, false);
                _chapterEditor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddChapterTemplate()
        {
            // 템플릿 생성
            ChapterTemplate template = CreateInstance<ChapterTemplate>();
            
            // 에셋 저장
            string text = Path.GetFileName(_storyDirectories[_selectedStoryIndex]);
            string defaultPath = _storyDirectories[_selectedStoryIndex];
            string path = EditorUtility.SaveFilePanelInProject("챕터 생성", $"{text}_", "asset", "이름을 설정해주세요.", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                template.SetTitle(Path.GetFileNameWithoutExtension(path));

                AssetDatabase.CreateAsset(template, path);
                AssetDatabase.SaveAssets();
                LoadChapterTemplates();
            }
        }

        private void DeleteSelectedChapterTemplate()
        {
            if (_chapterTemplates.Count > 0)
            {
                ChapterTemplate selectedTemplate = _chapterTemplates[_selectedChapterIndex];
                string assetPath = AssetDatabase.GetAssetPath(selectedTemplate);
                _chapterTemplates.RemoveAt(_selectedChapterIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private void LoadChapterTemplates()
        {
            _chapterTemplates.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ChapterTemplate", new string[] { _storyDirectories[_selectedStoryIndex] });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ChapterTemplate template = AssetDatabase.LoadAssetAtPath<ChapterTemplate>(path);

                _chapterTemplates.Add(template);
            }
        }
        #endregion

        private void DrawLine()
        {
            GUILayout.Space(5);
            Rect rect = GUILayoutUtility.GetRect(0, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(5);
        }
    }
}
