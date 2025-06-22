using FrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EvolveThisMatch.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XNodeEditor;

namespace EvolveThisMatch.Editor
{
    public class DataSystemEditorWindow : EditorWindow
    {
        private int selectedTab = 0;
        private Vector2 contentScrollPosition;
        //private List<Texture2D> resizedTextures = new List<Texture2D>();
        private Dictionary<string, List<Texture2D>> resizedTextures = new Dictionary<string, List<Texture2D>>();

        #region 유닛
        private int selectedUnitTitle = 0;

        #region 아군 유닛
        private UnityEditor.Editor agentEditor;
        private int selectedAgentIndex = 0;
        private Vector2 agentScrollPosition;
        private List<AgentTemplate> agentTemplates = new List<AgentTemplate>();
        #endregion
        #region 소환수 유닛
        private UnityEditor.Editor summonEditor;
        private int selectedSummonIndex = 0;
        private Vector2 summonScrollPosition;
        private List<SummonTemplate> summonTemplates = new List<SummonTemplate>();
        #endregion
        #region 적군 유닛
        private UnityEditor.Editor enemyEditor;
        private int selectedEnemyIndex = 0;
        private Vector2 enemyScrollPosition;
        private List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();
        #endregion
        #endregion

        #region 상태
        private int selectedStatusTitle = 0;
        #region 버프
        private UnityEditor.Editor buffEditor;
        private int selectedBuffIndex = 0;
        private Vector2 buffScrollPosition;
        private List<BuffTemplate> buffTemplates = new List<BuffTemplate>();
        #endregion
        #region 상태이상
        private UnityEditor.Editor abnormalStatusEditor;
        private int selectedAbnormalStatusIndex = 0;
        private Vector2 abnormalStatusScrollPosition;
        private List<AbnormalStatusTemplate> abnormalStatusTemplates = new List<AbnormalStatusTemplate>();
        #endregion
        #region 전역 상태
        private UnityEditor.Editor globalStatusEditor;
        private int selectedGlobalStatusIndex = 0;
        private Vector2 globalStatusScrollPosition;
        private List<GlobalStatusTemplate> globalStatusTemplates = new List<GlobalStatusTemplate>();
        #endregion
        #endregion

        #region 스킬
        private int selectedSkillTitle = 0;
        #region 액티브 스킬
        private UnityEditor.Editor activeSkillEditor;
        private int selectedActiveSkillIndex = 0;
        private Vector2 activeSkillScrollPosition;
        private List<ActiveSkillTemplate> activeSkillTemplates = new List<ActiveSkillTemplate>();
        #endregion
        #region 패시브 스킬
        private UnityEditor.Editor passiveSkillEditor;
        private int selectedPassiveSkillIndex = 0;
        private Vector2 passiveSkillScrollPosition;
        private List<PassiveSkillTemplate> passiveSkillTemplates = new List<PassiveSkillTemplate>();
        #endregion
        #endregion

        #region 아이템
        private int selectedItemTitle = 0;
        #region 액티브 아이템
        private UnityEditor.Editor activeItemEditor;
        private int selectedActiveItemIndex = 0;
        private Vector2 activeItemScrollPosition;
        private List<ActiveItemTemplate> activeItemTemplates = new List<ActiveItemTemplate>();
        #endregion
        #region 패시브 아이템
        private UnityEditor.Editor passiveItemEditor;
        private int selectedPassiveItemIndex = 0;
        private Vector2 passiveItemScrollPosition;
        private List<PassiveItemTemplate> passiveItemTemplates = new List<PassiveItemTemplate>();
        #endregion
        #endregion

        [MenuItem("Window/게임 데이터 관리 시스템")]
        public static void Open()
        {
            var window = GetWindow<DataSystemEditorWindow>();
            window.titleContent = new GUIContent("게임 데이터 관리");
        }

        private void OnGUI()
        {
            DrawTab();
        }

        private void OnDisable()
        {
            if (agentEditor != null)
            {
                DestroyImmediate(agentEditor);
                agentEditor = null;
            }
            if (enemyEditor != null)
            {
                DestroyImmediate(enemyEditor);
                enemyEditor = null;
            }
            if (buffEditor != null)
            {
                DestroyImmediate(buffEditor);
                buffEditor = null;
            }
            if (abnormalStatusEditor != null)
            {
                DestroyImmediate(abnormalStatusEditor);
                abnormalStatusEditor = null;
            }
            if (activeSkillEditor != null)
            {
                DestroyImmediate(activeSkillEditor);
                activeSkillEditor = null;
            }
            if (passiveSkillEditor != null)
            {
                DestroyImmediate(passiveSkillEditor);
                passiveSkillEditor = null;
            }
        }

        private void DrawTab()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedTab == 0, "유닛", "Button")) selectedTab = 0;
            if (GUILayout.Toggle(selectedTab == 1, "상태", "Button")) selectedTab = 1;
            if (GUILayout.Toggle(selectedTab == 2, "스킬", "Button")) selectedTab = 2;
            if (GUILayout.Toggle(selectedTab == 3, "아이템", "Button")) selectedTab = 3;
            GUILayout.EndHorizontal();

            DrawLine();

            switch (selectedTab)
            {
                case 0:
                    DrawUnitTitle();
                    break;
                case 1:
                    DrawStatusTitle();
                    break;
                case 2:
                    DrawSkillTitle();
                    break;
                case 3:
                    DrawItemTitle();
                    break;
            }
        }

        private void DrawUnitTitle()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedUnitTitle == 0, "아군 유닛", "Button")) selectedUnitTitle = 0;
            if (GUILayout.Toggle(selectedUnitTitle == 1, "소환수 유닛", "Button")) selectedUnitTitle = 1;
            if (GUILayout.Toggle(selectedUnitTitle == 2, "적 유닛", "Button")) selectedUnitTitle = 2;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectedUnitTitle == 0) DrawAgentTab();
            else if (selectedUnitTitle == 1) DrawSummonTab();
            else DrawEnemyTab();
        }

        private void DrawStatusTitle()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedStatusTitle == 0, "버프", "Button")) selectedStatusTitle = 0;
            if (GUILayout.Toggle(selectedStatusTitle == 1, "상태이상", "Button")) selectedStatusTitle = 1;
            if (GUILayout.Toggle(selectedStatusTitle == 2, "전역 상태", "Button")) selectedStatusTitle = 2;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectedStatusTitle == 0) DrawBuffTab();
            else if (selectedStatusTitle == 1) DrawAbnormalStatusTab();
            else DrawGlobalStatusTab();
        }

        private void DrawSkillTitle()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedSkillTitle == 0, "액티브 스킬", "Button")) selectedSkillTitle = 0;
            if (GUILayout.Toggle(selectedSkillTitle == 1, "패시브 스킬", "Button")) selectedSkillTitle = 1;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectedSkillTitle == 0) DrawActiveSkillTab();
            else if (selectedSkillTitle == 1) DrawPassiveSkillTab();
        }

        private void DrawItemTitle()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedItemTitle == 0, "액티브 아이템", "Button")) selectedItemTitle = 0;
            if (GUILayout.Toggle(selectedItemTitle == 1, "패시브 아이템", "Button")) selectedItemTitle = 1;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (selectedItemTitle == 0) DrawActiveItemTab();
            else DrawPassiveItemTab();
        }

        #region 유닛
        private void DrawAgentTab()
        {
            DrawTemplateTab<AgentTemplate>(
                ref agentTemplates,
                ref selectedAgentIndex,
                ref agentScrollPosition,
                ref agentEditor,
                "아군",
                "Assets/EvolveThisMatch/GameData/Unit/Agent",
                "Agent",
                () =>
                {
                    var window = GetWindow<LoadAgentTemplateEditorWindow>();
                    window.titleContent = new GUIContent("아군 유닛 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }

        private void DrawSummonTab()
        {
            DrawTemplateTab<SummonTemplate>(
                ref summonTemplates,
                ref selectedSummonIndex,
                ref summonScrollPosition,
                ref summonEditor,
                "소환수",
                "Assets/EvolveThisMatch/GameData/Unit/Summon",
                "Summon",
                () =>
                {
                    var window = GetWindow<LoadSummonTemplateEditorWindow>();
                    window.titleContent = new GUIContent("소환수 유닛 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }

        private void DrawEnemyTab()
        {
            DrawTemplateTab<EnemyTemplate>(
                ref enemyTemplates,
                ref selectedEnemyIndex,
                ref enemyScrollPosition,
                ref enemyEditor,
                "적",
                "Assets/EvolveThisMatch/GameData/Unit/Enemy",
                "Enemy",
                () =>
                {
                    var window = GetWindow<LoadEnemyTemplateEditorWindow>();
                    window.titleContent = new GUIContent("적 유닛 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }
        #endregion

        #region 상태
        private void DrawBuffTab()
        {
            DrawTemplateTab<BuffTemplate>(
                ref buffTemplates,
                ref selectedBuffIndex,
                ref buffScrollPosition,
                ref buffEditor,
                "버프",
                "Assets/EvolveThisMatch/GameData/Status/BuffStatus",
                "BuffStatus"
            );
        }

        private void DrawAbnormalStatusTab()
        {
            DrawTemplateTab<AbnormalStatusTemplate>(
                ref abnormalStatusTemplates,
                ref selectedAbnormalStatusIndex,
                ref abnormalStatusScrollPosition,
                ref abnormalStatusEditor,
                "상태이상",
                "Assets/EvolveThisMatch/GameData/Status/AbnormalStatus",
                "AbnormalStatus"
            );
        }

        private void DrawGlobalStatusTab()
        {
            DrawTemplateTab<GlobalStatusTemplate>(
                ref globalStatusTemplates,
                ref selectedGlobalStatusIndex,
                ref globalStatusScrollPosition,
                ref globalStatusEditor,
                "전역 상태",
                "Assets/EvolveThisMatch/GameData/Status/GlobalStatus",
                "GlobalStatus"
            );
        }
        #endregion

        #region 스킬
        private void DrawActiveSkillTab()
        {
            DrawTemplateTab<ActiveSkillTemplate>(
                ref activeSkillTemplates,
                ref selectedActiveSkillIndex,
                ref activeSkillScrollPosition,
                ref activeSkillEditor,
                "액티브 스킬",
                "Assets/EvolveThisMatch/GameData/Skill/ActiveSkill",
                "ActiveSkill",
                () =>
                {
                    var window = GetWindow<LoadActiveSkillTemplateEditorWindow>();
                    window.titleContent = new GUIContent("액티브 스킬 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }

        private void DrawPassiveSkillTab()
        {
            DrawTemplateTab<PassiveSkillTemplate>(
                ref passiveSkillTemplates,
                ref selectedPassiveSkillIndex,
                ref passiveSkillScrollPosition,
                ref passiveSkillEditor,
                "패시브 스킬",
                "Assets/EvolveThisMatch/GameData/Skill/PassiveSkill",
                "PassiveSkill",
                () =>
                {
                    var window = GetWindow<LoadPassiveSkillTemplateEditorWindow>();
                    window.titleContent = new GUIContent("패시브 스킬 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }
        #endregion

        #region 아이템
        private void DrawActiveItemTab()
        {
            DrawTemplateTab<ActiveItemTemplate>(
                ref activeItemTemplates,
                ref selectedActiveItemIndex,
                ref activeItemScrollPosition,
                ref activeItemEditor,
                "액티브 아이템",
                "Assets/EvolveThisMatch/GameData/Item/ActiveItem",
                "ActiveItem",
                () =>
                {
                    var window = GetWindow<LoadActiveItemTemplateEditorWindow>();
                    window.titleContent = new GUIContent("액티브 아이템 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }

        private void DrawPassiveItemTab()
        {
            DrawTemplateTab<PassiveItemTemplate>(
                ref passiveItemTemplates,
                ref selectedPassiveItemIndex,
                ref passiveItemScrollPosition,
                ref passiveItemEditor,
                "패시브 아이템",
                "Assets/EvolveThisMatch/GameData/Item/PassiveItem",
                "PassiveItem",
                () =>
                {
                    var window = GetWindow<LoadPassiveItemTemplateEditorWindow>();
                    window.titleContent = new GUIContent("패시브 아이템 불러오기");
                    window.minSize = new Vector2(300, 100);
                    window.maxSize = new Vector2(300, 100);
                }
            );
        }
        #endregion

        #region 템플릿 관리
        private void DrawTemplateTab<T>(ref List<T> templates, ref int selectedIndex, ref Vector2 scrollPosition, ref UnityEditor.Editor editor, string titleText, string defaultPath, string assetPrefix, UnityAction action = null) where T : ScriptableObject, IDataWindowEntry
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical(GUILayout.Width(200));

            if (action != null && GUILayout.Button($"{titleText} 불러오기 창"))
            {
                action?.Invoke();
            }
            if (GUILayout.Button($"{titleText} 추가"))
            {
                AddTemplate(defaultPath, assetPrefix, ref templates);
            }
            if (GUILayout.Button($"{titleText} 삭제"))
            {
                DeleteTemplate(ref templates, ref selectedIndex);
            }
            if (GUILayout.Button($"{titleText} 탐색"))
            {
                LoadTemplates(ref templates, assetPrefix);
            }

            DrawLine();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            var catalogStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(5, 5, -2, -2),
                border = new RectOffset(0, 0, 0, 0),
                fixedWidth = GUI.skin.box.fixedWidth,
                fixedHeight = 40
            };

            for (int i = 0; i < templates.Count; i++)
            {
                bool isSelected = (selectedIndex == i);

                if (!resizedTextures.ContainsKey(assetPrefix)) LoadTexture(templates, assetPrefix);

                bool isNullTexture = resizedTextures[assetPrefix][i] == null;
                var displayName = templates[i].displayName;
                int maxLength = isNullTexture ? 18 : 13;
                string text = "  " + displayName.Substring(0, Mathf.Min(displayName.Length, maxLength));

                GUIContent content = isNullTexture ? new GUIContent(text) : new GUIContent(text, resizedTextures[assetPrefix][i]);

                if (GUILayout.Toggle(isSelected, content, catalogStyle))
                {
                    if (selectedIndex != i)
                    {
                        selectedIndex = i;
                        GUI.FocusControl(null);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (templates.Count > 0 && selectedIndex < templates.Count)
            {
                T selectedTemplate = templates[selectedIndex];

                if (editor == null || editor.target != selectedTemplate)
                {
                    editor = UnityEditor.Editor.CreateEditor(selectedTemplate);
                }

                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition, false, false);
                editor.OnInspectorGUI();
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void AddTemplate<T>(string defaultPath, string assetPrefix, ref List<T> templates) where T : ScriptableObject, IDataWindowEntry
        {
            T newTemplate = CreateInstance<T>();

            string path = EditorUtility.SaveFilePanelInProject($"{assetPrefix} 추가", $"{assetPrefix}_", "asset", "", defaultPath);
            if (!string.IsNullOrEmpty(path))
            {
                newTemplate.SetDisplayName(Path.GetFileNameWithoutExtension(path).Replace($"{assetPrefix}_", ""));

                AssetDatabase.CreateAsset(newTemplate, path);
                AssetDatabase.SaveAssets();
                LoadTemplates<T>(ref templates, assetPrefix);
            }
        }

        private void DeleteTemplate<T>(ref List<T> templates, ref int selectedIndex) where T : ScriptableObject
        {
            if (!EditorUtility.DisplayDialog("경고!", "이 템플릿을 삭제하시겠습니까?", "네", "아니요")) return;

            if (templates.Count > 0)
            {
                T selectedTemplate = templates[selectedIndex];
                string assetPath = AssetDatabase.GetAssetPath(selectedTemplate);
                templates.RemoveAt(selectedIndex);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, templates.Count - 1);
            }
        }

        private void LoadTemplates<T>(ref List<T> templates, string assetPrefix) where T : ScriptableObject, IDataWindowEntry
        {
            templates.Clear();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T template = AssetDatabase.LoadAssetAtPath<T>(path);
                templates.Add(template);
            }
            templates = templates.OrderBy(t => ((IDataWindowEntry)t).id).ToList();

            LoadTexture(templates, assetPrefix);
        }

        private void LoadTexture<T>(List<T> templates, string assetPrefix) where T : ScriptableObject, IDataWindowEntry
        {
            List<Texture2D> textures = new List<Texture2D>(templates.Count);
            for (int i = 0; i < templates.Count; i++)
            {
                var sprite = templates[i].sprite;
                textures.Add(sprite != null ? sprite.texture.ResizeTexture(30, 30) : null);
            }

            resizedTextures[assetPrefix] = textures;
        }

        private void DrawLine()
        {
            GUILayout.Space(5);
            Rect rect = GUILayoutUtility.GetRect(0, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(5);
        }
        #endregion
    }
}