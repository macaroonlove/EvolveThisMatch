using System.Collections.Generic;
using EvolveThisMatch.Core;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
    public class LoadPassiveItemTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            InitializeRarityTemplates();

            Dictionary<int, PassiveItemTemplate> templateDic = new Dictionary<int, PassiveItemTemplate>();
            var guids = AssetDatabase.FindAssets("t:PassiveItemTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<PassiveItemTemplate>(path);
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
                        AssetDatabase.RenameAsset(assetPath, $"PassiveItem_{newName}");
                    }

                    // 아이템 설명
                    template.SetDescription(csvDic["아이템 설명"][i]);

                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        template.SetRarity(rarity);
                    }

                    // 가격
                    if (int.TryParse(csvDic["가격"][i], out var price))
                    {
                        template.SetPrice(price);
                    }

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<PassiveItemTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 아이템 이름
                    newTemplate.SetDisplayName(csvDic["아이템 이름"][i]);

                    // 아이템 설명
                    newTemplate.SetDescription(csvDic["아이템 설명"][i]);

                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        newTemplate.SetRarity(rarity);
                    }

                    // 가격
                    if (int.TryParse(csvDic["가격"][i], out var price))
                    {
                        newTemplate.SetPrice(price);
                    }

                    string path = $"Assets/EvolveThisMatch/GameData/Item/PassiveItem/PassiveItem_{csvDic["아이템 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 등급 템플릿 가져오기
        private Dictionary<string, PassiveItemRarityTemplate> _rarityDic = new Dictionary<string, PassiveItemRarityTemplate>();

        private void InitializeRarityTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:PassiveItemRarityTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var rarity = AssetDatabase.LoadAssetAtPath<PassiveItemRarityTemplate>(path);
                _rarityDic[rarity.rarity.ToString()] = rarity;
            }
        }
        #endregion
    }
}