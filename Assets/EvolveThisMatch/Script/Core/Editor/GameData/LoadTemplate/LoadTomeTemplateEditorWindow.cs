using System;
using System.Collections.Generic;
using EvolveThisMatch.Core;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
#if UNITY_EDITOR
    public class LoadTomeTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            Dictionary<int, TomeTemplate> templateDic = new Dictionary<int, TomeTemplate>();
            var guids = AssetDatabase.FindAssets("t:TomeTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<TomeTemplate>(path);
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
                        AssetDatabase.RenameAsset(assetPath, $"Tome_{newName}");
                    }

                    // 아이템 설명
                    template.SetDescription(csvDic["아이템 설명"][i]);

                    // 초기값
                    if (int.TryParse(csvDic["초기값"][i], out var initCoin))
                    {
                        template.SetInitValue(initCoin);
                    }

                    // 필요 코인
                    if (int.TryParse(csvDic["필요 코인"][i], out var needCoin))
                    {
                        template.SetNeedCoin(needCoin);
                    }

                    // 쿨타임
                    if (float.TryParse(csvDic["쿨타임"][i], out var cooldownTime))
                    {
                        template.SetCooldownTime(cooldownTime);
                    }

                    // 지연시간
                    if (float.TryParse(csvDic["지연시간"][i], out var delay))
                    {
                        template.SetDelay(delay);
                    }

                    // 유닛 타입
                    if (Enum.TryParse<EUnitType>(csvDic["유닛 타입"][i], out var unitType))
                    {
                        template.SetUnitType(unitType);
                    }
                    else
                    {
                        template.SetUnitType(unitType);
                    }

                    // 범위 방식
                    if (Enum.TryParse<ETomeRangeType>(csvDic["범위 방식"][i], out var rangeType))
                    {
                        template.SetRangeType(rangeType);
                    }

                    // 지연시간
                    if (float.TryParse(csvDic["범위"][i], out var range))
                    {
                        template.SetRange(range);
                    }

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<TomeTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 아이템 이름
                    newTemplate.SetDisplayName(csvDic["아이템 이름"][i]);

                    // 아이템 설명
                    newTemplate.SetDescription(csvDic["아이템 설명"][i]);

                    // 초기값
                    if (int.TryParse(csvDic["초기값"][i], out var initCoin))
                    {
                        newTemplate.SetInitValue(initCoin);
                    }
                    
                    // 필요 코인
                    if (int.TryParse(csvDic["필요 코인"][i], out var needCoin))
                    {
                        newTemplate.SetNeedCoin(needCoin);
                    }

                    // 쿨타임
                    if (float.TryParse(csvDic["쿨타임"][i], out var cooldownTime))
                    {
                        newTemplate.SetCooldownTime(cooldownTime);
                    }

                    // 지연시간
                    if (float.TryParse(csvDic["지연시간"][i], out var delay))
                    {
                        newTemplate.SetDelay(delay);
                    }

                    // 유닛 타입
                    if (Enum.TryParse<EUnitType>(csvDic["유닛 타입"][i], out var unitType))
                    {
                        newTemplate.SetUnitType(unitType);
                    }
                    else
                    {
                        newTemplate.SetUnitType(unitType);
                    }

                    // 범위 방식
                    if (Enum.TryParse<ETomeRangeType>(csvDic["범위 방식"][i], out var rangeType))
                    {
                        newTemplate.SetRangeType(rangeType);
                    }

                    // 지연시간
                    if (float.TryParse(csvDic["범위"][i], out var range))
                    {
                        newTemplate.SetRange(range);
                    }

                    string path = $"Assets/EvolveThisMatch/GameData/Item/Tome/Tome_{csvDic["아이템 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif
}