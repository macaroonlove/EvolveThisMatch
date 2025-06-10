using System.Collections.Generic;
using EvolveThisMatch.Core;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Editor
{
    public class LoadPassiveSkillTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            Dictionary<int, PassiveSkillTemplate> templateDic = new Dictionary<int, PassiveSkillTemplate>();
            var guids = AssetDatabase.FindAssets("t:PassiveSkillTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<PassiveSkillTemplate>(path);
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
                    if (template.displayName != csvDic["스킬 이름"][i])
                    {
                        string newName = csvDic["스킬 이름"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"PassiveSkill_{newName}");
                    }

                    // 스킬 설명
                    template.SetDescription(csvDic["스킬 설명"][i]);

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<PassiveSkillTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 스킬 이름
                    newTemplate.SetDisplayName(csvDic["스킬 이름"][i]);

                    // 스킬 설명
                    newTemplate.SetDescription(csvDic["스킬 설명"][i]);

                    string path = $"Assets/EvolveThisMatch/GameData/Skill/PassiveSkill/PassiveSkill_{csvDic["스킬 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}