using EvolveThisMatch.Core;
using System.Collections.Generic;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
#if UNITY_EDITOR
    public class LoadArtifactTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            Dictionary<int, ArtifactTemplate> templateDic = new Dictionary<int, ArtifactTemplate>();
            var guids = AssetDatabase.FindAssets("t:ArtifactTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<ArtifactTemplate>(path);
                templateDic[template.id] = template;
            }

            var idList = csvDic["No."];

            for (int i = 0; i < idList.Count; i++)
            {
                if (int.TryParse(idList[i], out int id) == false) continue;
                if (id < _startId || id > _endId) continue;

                // 템플릿이 존재한다면 수정
                if (templateDic.TryGetValue(id, out var template))
                {
                    // 스킬 이름
                    if (template.displayName != csvDic["아이템 이름"][i])
                    {
                        string newName = csvDic["아이템 이름"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"Artifact_{newName}");
                    }

                    // 아이템 설명
                    template.SetDescription(csvDic["아이템 설명"][i]);

                    // 초기값
                    if (int.TryParse(csvDic["초기값"][i], out var initValue))
                    {
                        template.SetInitValue(initValue);
                    }

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<ArtifactTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 아이템 이름
                    newTemplate.SetDisplayName(csvDic["아이템 이름"][i]);

                    // 아이템 설명
                    newTemplate.SetDescription(csvDic["아이템 설명"][i]);

                    // 초기값
                    if (int.TryParse(csvDic["초기값"][i], out var initValue))
                    {
                        newTemplate.SetInitValue(initValue);
                    }

                    string path = $"Assets/EvolveThisMatch/GameData/Item/Artifact/Artifact_{csvDic["아이템 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif
}