using FrameWork.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VInspector;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Agent", fileName = "AgentLibrary", order = 0)]
    public class AgentLibraryTemplate : ScriptableObject
    {
        public List<AgentTemplate> templates = new List<AgentTemplate>();

#if UNITY_EDITOR
        [Header("필터")]
        [Label("모든 유닛"), SerializeField] private bool _isAll;
        //[HideIf("_isAll")]
        //[Label("조건 추가"), SerializeField] private bool _isCondition;
        //[EndIf]

        [ContextMenu("필터로 AgentTemplate 찾기")]
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
                    templates.Add(template);
                }
            }

            // id순 정렬
            templates.Sort((a, b) => a.id.CompareTo(b.id));

            // 변경 사항을 저장
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}