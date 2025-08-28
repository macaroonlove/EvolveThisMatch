using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Artifact", fileName = "ArtifactLibrary", order = 0)]
    public class ArtifactLibraryTemplate : ScriptableObject
    {
        public List<ArtifactTemplate> templates = new List<ArtifactTemplate>();

#if UNITY_EDITOR
        [ContextMenu("모든 ArtifactTemplate 찾기")]
        public void FindAllRefresh()
        {
            templates.Clear();
            var AssetsGUIDArray = AssetDatabase.FindAssets("t:ArtifactTemplate", new[] { "Assets/" });
            foreach (var guid in AssetsGUIDArray)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath(path, typeof(ArtifactTemplate)) as ArtifactTemplate;

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
#endif
    }
}