using System;
using System.Collections.Generic;
using EvolveThisMatch.Core;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
#if UNITY_EDITOR
    public class LoadActiveSkillTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            InitializeSkillTypeTemplates();

            Dictionary<int, ActiveSkillTemplate> templateDic = new Dictionary<int, ActiveSkillTemplate>();
            var guids = AssetDatabase.FindAssets("t:ActiveSkillTemplate");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<ActiveSkillTemplate>(path);
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
                    // 스킬 타입
                    if (_skillTypeDic.TryGetValue(csvDic["스킬 타입"][i], out var skillType))
                    {
                        template.SetSkillType(skillType);
                    }

                    // 스킬 이름
                    if (template.displayName != csvDic["스킬 이름"][i])
                    {
                        string newName = csvDic["스킬 이름"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"ActiveSkill_{newName}");
                    }

                    // 스킬 설명
                    template.SetDescription(csvDic["스킬 설명"][i]);

                    // 스킬 발동 방식
                    if (Enum.TryParse<EActiveSkillTriggerType>(csvDic["스킬 발동 방식"][i], out var triggerType))
                    {
                        template.SetActiveSkillTriggerType(triggerType);
                    }

                    // 스킬 타겟팅 방식
                    if (Enum.TryParse<EActiveSkillTargetingType>(csvDic["스킬 타겟팅 방식"][i], out var targetingType))
                    {
                        template.SetActiveSkillTargetingType(targetingType);
                    }

                    // 소모 자원
                    if (Enum.TryParse<EActiveSkillPayType>(csvDic["소모 자원"][i], out var payType))
                    {
                        template.SetActiveSkillPayType(payType);
                    }

                    // 소모량
                    if (int.TryParse(csvDic["소모량"][i], out var payAmount))
                    {
                        template.SetPayAmount(payAmount);
                    }

                    // 쿨타임
                    if (float.TryParse(csvDic["쿨타임"][i], out var cooldownTime))
                    {
                        template.SetCooldownTime(cooldownTime);
                    }

                    // 애니메이션 파라미터
                    template.SetParameterHash(csvDic["애니메이션 파라미터"][i]);

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<ActiveSkillTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 스킬 타입
                    if (_skillTypeDic.TryGetValue(csvDic["스킬 타입"][i], out var skillType))
                    {
                        template.SetSkillType(skillType);
                    }

                    // 스킬 이름
                    newTemplate.SetDisplayName(csvDic["스킬 이름"][i]);

                    // 스킬 설명
                    newTemplate.SetDescription(csvDic["스킬 설명"][i]);

                    // 스킬 발동 방식
                    if (Enum.TryParse<EActiveSkillTriggerType>(csvDic["스킬 발동 방식"][i], out var triggerType))
                    {
                        newTemplate.SetActiveSkillTriggerType(triggerType);
                    }

                    // 스킬 타겟팅 방식
                    if (Enum.TryParse<EActiveSkillTargetingType>(csvDic["스킬 타겟팅 방식"][i], out var targetingType))
                    {
                        newTemplate.SetActiveSkillTargetingType(targetingType);
                    }

                    // 소모 자원
                    if (Enum.TryParse<EActiveSkillPayType>(csvDic["소모 자원"][i], out var payType))
                    {
                        newTemplate.SetActiveSkillPayType(payType);
                    }

                    // 소모량
                    if (int.TryParse(csvDic["소모량"][i], out var payAmount))
                    {
                        newTemplate.SetPayAmount(payAmount);
                    }

                    // 쿨타임
                    if (float.TryParse(csvDic["쿨타임"][i], out var cooldownTime))
                    {
                        newTemplate.SetCooldownTime(cooldownTime);
                    }

                    // 애니메이션 파라미터
                    newTemplate.SetParameterHash(csvDic["애니메이션 파라미터"][i]);

                    string path = $"Assets/EvolveThisMatch/GameData/Skill/ActiveSkill/ActiveSkill_{csvDic["스킬 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 스킬 타입 템플릿 가져오기
        private Dictionary<string, SkillTypeTemplate> _skillTypeDic = new Dictionary<string, SkillTypeTemplate>();

        private void InitializeSkillTypeTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:SkillTypeTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skillType = AssetDatabase.LoadAssetAtPath<SkillTypeTemplate>(path);
                _skillTypeDic[skillType.type.ToString()] = skillType;
            }
        }
        #endregion
    }
#endif
}