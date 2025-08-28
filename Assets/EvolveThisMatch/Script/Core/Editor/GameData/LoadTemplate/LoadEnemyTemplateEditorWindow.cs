using EvolveThisMatch.Core;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
#if UNITY_EDITOR
    public class LoadEnemyTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            InitializeRarityTemplates();

            Dictionary<int, EnemyTemplate> templateDic = new Dictionary<int, EnemyTemplate>();
            var guids = AssetDatabase.FindAssets("t:EnemyTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<EnemyTemplate>(path);
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
                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        template.SetRarity(rarity);
                    }

                    // 유닛 이름
                    if (template.displayName != csvDic["유닛 이름"][i])
                    {
                        string newName = csvDic["유닛 이름"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"Enemy_{newName}");
                    }

                    // 이동 방식
                    if (Enum.TryParse<EMoveType>(csvDic["이동 방식"][i], out var moveType))
                    {
                        template.SetMoveType(moveType);
                    }

                    // 이동 속도
                    if (float.TryParse(csvDic["이동 속도"][i], out var moveSpeed))
                    {
                        template.SetMoveSpeed(moveSpeed);
                    }

                    // 데미지 타입
                    if (Enum.TryParse<EDamageType>(csvDic["데미지 타입"][i], out var damageType))
                    {
                        template.SetDamageType(damageType);
                    }

                    // 공격력
                    if (int.TryParse(csvDic["공격력"][i], out var atk))
                    {
                        template.SetATK(atk);
                    }

                    // 공격 간격
                    if (float.TryParse(csvDic["공격 간격"][i], out var attackTerm))
                    {
                        template.SetAttackTerm(attackTerm);
                    }

                    // 공격 사거리
                    if (float.TryParse(csvDic["공격 사거리"][i], out var attackRange))
                    {
                        template.SetAttackRange(attackRange);
                    }

                    // 물리 관통력
                    if (int.TryParse(csvDic["물리 관통력"][i], out var physicalPenetration))
                    {
                        template.SetPhysicalPenetration(physicalPenetration);
                    }

                    // 마법 관통력
                    if (int.TryParse(csvDic["마법 관통력"][i], out var magicPenetration))
                    {
                        template.SetMagicPenetration(magicPenetration);
                    }

                    // 치명타 확률
                    if (float.TryParse(csvDic["치명타 확률"][i], out var criticalHitChance))
                    {
                        template.SetCriticalHitChance(criticalHitChance);
                    }

                    // 치명타 데미지
                    if (float.TryParse(csvDic["치명타 데미지"][i], out var criticalHitDamage))
                    {
                        template.SetCriticalHitDamage(criticalHitDamage);
                    }

                    // 최대 체력
                    if (int.TryParse(csvDic["최대 체력"][i], out var maxHP))
                    {
                        template.SetMaxHP(maxHP);
                    }

                    // 방어력
                    if (int.TryParse(csvDic["방어력"][i], out var physicalResistance))
                    {
                        template.SetPhysicalResistance(physicalResistance);
                    }

                    // 마법 저항력
                    if (int.TryParse(csvDic["마법 저항력"][i], out var magicResistance))
                    {
                        template.SetMagicResistance(magicResistance);
                    }

                    // 초당 체력 회복량
                    if (int.TryParse(csvDic["초당 체력 회복량"][i], out var hpRecoveryPerSec))
                    {
                        template.SetHPRecoveryPerSec(hpRecoveryPerSec);
                    }

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<EnemyTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 등급
                    if (_rarityDic.TryGetValue(csvDic["등급"][i], out var rarity))
                    {
                        newTemplate.SetRarity(rarity);
                    }

                    // 유닛 이름
                    newTemplate.SetDisplayName(csvDic["유닛 이름"][i]);

                    // 이동 방식
                    if (Enum.TryParse<EMoveType>(csvDic["이동 방식"][i], out var moveType))
                    {
                        newTemplate.SetMoveType(moveType);
                    }

                    // 이동 속도
                    if (float.TryParse(csvDic["이동 속도"][i], out var moveSpeed))
                    {
                        newTemplate.SetMoveSpeed(moveSpeed);
                    }

                    // 데미지 타입
                    if (Enum.TryParse<EDamageType>(csvDic["데미지 타입"][i], out var damageType))
                    {
                        newTemplate.SetDamageType(damageType);
                    }

                    // 공격력
                    if (int.TryParse(csvDic["공격력"][i], out var atk))
                    {
                        newTemplate.SetATK(atk);
                    }

                    // 공격 간격
                    if (float.TryParse(csvDic["공격 간격"][i], out var attackTerm))
                    {
                        newTemplate.SetAttackTerm(attackTerm);
                    }

                    // 공격 사거리
                    if (float.TryParse(csvDic["공격 사거리"][i], out var attackRange))
                    {
                        newTemplate.SetAttackRange(attackRange);
                    }

                    // 물리 관통력
                    if (int.TryParse(csvDic["물리 관통력"][i], out var physicalPenetration))
                    {
                        newTemplate.SetPhysicalPenetration(physicalPenetration);
                    }

                    // 마법 관통력
                    if (int.TryParse(csvDic["마법 관통력"][i], out var magicPenetration))
                    {
                        newTemplate.SetMagicPenetration(magicPenetration);
                    }

                    // 치명타 확률
                    if (float.TryParse(csvDic["치명타 확률"][i], out var criticalHitChance))
                    {
                        newTemplate.SetCriticalHitChance(criticalHitChance);
                    }

                    // 치명타 데미지
                    if (float.TryParse(csvDic["치명타 데미지"][i], out var criticalHitDamage))
                    {
                        newTemplate.SetCriticalHitDamage(criticalHitDamage);
                    }

                    // 최대 체력
                    if (int.TryParse(csvDic["최대 체력"][i], out var maxHP))
                    {
                        newTemplate.SetMaxHP(maxHP);
                    }

                    // 방어력
                    if (int.TryParse(csvDic["방어력"][i], out var physicalResistance))
                    {
                        newTemplate.SetPhysicalResistance(physicalResistance);
                    }

                    // 마법 저항력
                    if (int.TryParse(csvDic["마법 저항력"][i], out var magicResistance))
                    {
                        newTemplate.SetMagicResistance(magicResistance);
                    }

                    // 초당 체력 회복량
                    if (int.TryParse(csvDic["초당 체력 회복량"][i], out var hpRecoveryPerSec))
                    {
                        newTemplate.SetHPRecoveryPerSec(hpRecoveryPerSec);
                    }

                    string path = $"Assets/EvolveThisMatch/GameData/Unit/Enemy/Enemy_{csvDic["유닛 이름"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 등급 템플릿 가져오기
        private Dictionary<string, EnemyRarityTemplate> _rarityDic = new Dictionary<string, EnemyRarityTemplate>();

        private void InitializeRarityTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:EnemyRarityTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var rarity = AssetDatabase.LoadAssetAtPath<EnemyRarityTemplate>(path);
                _rarityDic[rarity.rarity.ToString()] = rarity;
            }
        }
        #endregion
    }
#endif
}