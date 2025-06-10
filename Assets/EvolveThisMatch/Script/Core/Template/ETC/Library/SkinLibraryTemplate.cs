using FrameWork.Editor;
using UnityEditor;
using UnityEngine;
using VInspector;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Skin", fileName = "SkinLibrary", order = 0)]
    public class SkinLibraryTemplate : ScriptableObject
    {
        public SerializedDictionary<SkinTemplate, AgentTemplate> templates = new SerializedDictionary<SkinTemplate, AgentTemplate>();

        [Header("필터 (기본 스킨은 제외됩니다.)")]
        [Label("모든 스킨"), SerializeField] private bool _isAll;
        //[HideIf("_isAll")]
        //[Label("조건 추가"), SerializeField] private bool _isCondition;
        //[EndIf]

        [ContextMenu("필터로 SkinTemplate 찾기")]
        public void FindAllRefresh()
        {
            templates.Clear();
            var AssetsGUIDArray = AssetDatabase.FindAssets("t:AgentTemplate", new[] { "Assets/" });
            foreach (var guid in AssetsGUIDArray)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath(path, typeof(AgentTemplate)) as AgentTemplate;

                bool isInclude = _isAll;
                //isInclude |= _isCondition;

                if (template != null && isInclude)
                {
                    bool isBasic = true;
                    foreach (var skin in template.skins)
                    {
                        if (isBasic)
                        {
                            isBasic = false;
                            continue;
                        }

                        templates.Add(skin, template);
                    }
                }
            }
            // 변경 사항을 저장
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}