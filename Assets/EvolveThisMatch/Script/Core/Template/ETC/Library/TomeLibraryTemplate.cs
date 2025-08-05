using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Tome", fileName = "TomeLibrary", order = 0)]
    public class TomeLibraryTemplate : ScriptableObject
    {
        public List<TomeTemplate> templates = new List<TomeTemplate>();

        [ContextMenu("모든 TomeTemplate 찾기")]
        public void FindAllRefresh()
        {
            templates.Clear();
            var AssetsGUIDArray = AssetDatabase.FindAssets("t:TomeTemplate", new[] { "Assets/" });
            foreach (var guid in AssetsGUIDArray)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath(path, typeof(TomeTemplate)) as TomeTemplate;

                if (template != null)
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
    }
}