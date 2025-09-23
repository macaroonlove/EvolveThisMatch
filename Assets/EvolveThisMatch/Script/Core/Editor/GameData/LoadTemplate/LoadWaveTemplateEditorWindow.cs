using EvolveThisMatch.Core;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EvolveThisMatch.Editor
{
#if UNITY_EDITOR
    public class LoadWaveTemplateEditorWindow : LoadTemplateEditorWindow
    {
        protected override void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic)
        {
            InitializeEnemyTemplates();

            Dictionary<int, WaveTemplate> templateDic = new Dictionary<int, WaveTemplate>();
            var guids = AssetDatabase.FindAssets("t:WaveTemplate");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<WaveTemplate>(path);
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
                    // 스테이지 이름
                    if (template.displayName != csvDic["스테이지"][i])
                    {
                        string newName = csvDic["스테이지"][i];
                        template.SetDisplayName(newName);

                        string assetPath = AssetDatabase.GetAssetPath(template);
                        AssetDatabase.RenameAsset(assetPath, $"Wave_{newName}");
                    }

                    // 웨이브 시간
                    if (int.TryParse(csvDic["웨이브 시간"][i], out var waveTime))
                    {
                        template.SetWaveTime(waveTime);
                    }

                    var minion = template.GetEnemyData(EEnemyRarity.Minion);
                    var common = template.GetEnemyData(EEnemyRarity.Common);
                    var elite = template.GetEnemyData(EEnemyRarity.Elite);
                    var boss = template.GetEnemyData(EEnemyRarity.Boss);

                    int templateId = (id - 1) / 10 * 4;

                    #region 미니언
                    // 미니언 템플릿
                    minion.SetTemplate(_enemyDic[templateId]);

                    // 미니언 HP
                    if (int.TryParse(csvDic["미니언 HP"][i], out var hp))
                    {
                        minion.SetHP(hp);
                    }

                    // 미니언 ATK
                    if (int.TryParse(csvDic["미니언 ATK"][i], out var atk))
                    {
                        minion.SetATK(atk);
                    }

                    // 미니언 코인
                    if (int.TryParse(csvDic["미니언 코인"][i], out var coin))
                    {
                        minion.SetCoin(coin);
                    }

                    // 미니언 골드
                    if (int.TryParse(csvDic["미니언 골드"][i], out var gold))
                    {
                        minion.SetGold(gold);
                    }

                    // 미니언 전리품
                    if (int.TryParse(csvDic["미니언 전리품"][i], out var loot))
                    {
                        minion.SetLoot(loot);
                    }
                    #endregion

                    #region 일반
                    // 일반 템플릿
                    common.SetTemplate(_enemyDic[templateId + 1]);

                    // 일반 HP
                    if (int.TryParse(csvDic["일반 HP"][i], out hp))
                    {
                        common.SetHP(hp);
                    }

                    // 일반 ATK
                    if (int.TryParse(csvDic["일반 ATK"][i], out atk))
                    {
                        common.SetATK(atk);
                    }

                    // 일반 코인
                    if (int.TryParse(csvDic["일반 코인"][i], out coin))
                    {
                        common.SetCoin(coin);
                    }

                    // 일반 골드
                    if (int.TryParse(csvDic["일반 골드"][i], out gold))
                    {
                        common.SetGold(gold);
                    }

                    // 일반 전리품
                    if (int.TryParse(csvDic["일반 전리품"][i], out loot))
                    {
                        common.SetLoot(loot);
                    }
                    #endregion

                    #region 엘리트
                    // 엘리트 템플릿
                    elite.SetTemplate(_enemyDic[templateId + 2]);

                    // 엘리트 HP
                    if (int.TryParse(csvDic["엘리트 HP"][i], out hp))
                    {
                        elite.SetHP(hp);
                    }

                    // 엘리트 ATK
                    if (int.TryParse(csvDic["엘리트 ATK"][i], out atk))
                    {
                        elite.SetATK(atk);
                    }

                    // 엘리트 크리스탈
                    if (int.TryParse(csvDic["엘리트 크리스탈"][i], out var crystal))
                    {
                        elite.SetCrystal(crystal);
                    }
                    #endregion

                    #region 보스
                    // 보스 템플릿
                    boss.SetTemplate(_enemyDic[templateId + 3]);

                    // 보스 HP
                    if (int.TryParse(csvDic["보스 HP"][i], out hp))
                    {
                        boss.SetHP(hp);
                    }

                    // 보스 ATK
                    if (int.TryParse(csvDic["보스 ATK"][i], out atk))
                    {
                        boss.SetATK(atk);
                    }

                    // 보스 크리스탈
                    if (int.TryParse(csvDic["보스 크리스탈"][i], out crystal))
                    {
                        boss.SetCrystal(crystal);
                    }
                    #endregion

                    EditorUtility.SetDirty(template);
                }
                // 템플릿이 존재하지 않는다면 생성
                else
                {
                    var newTemplate = CreateInstance<WaveTemplate>();

                    // 식별번호
                    newTemplate.SetId(id);

                    // 스테이지 이름
                    newTemplate.SetDisplayName(csvDic["스테이지"][i]);

                    // 웨이브 시간
                    if (int.TryParse(csvDic["웨이브 시간"][i], out var waveTime))
                    {
                        newTemplate.SetWaveTime(waveTime);
                    }

                    var minion = newTemplate.GetEnemyData(EEnemyRarity.Minion);
                    var common = newTemplate.GetEnemyData(EEnemyRarity.Common);
                    var elite = newTemplate.GetEnemyData(EEnemyRarity.Elite);
                    var boss = newTemplate.GetEnemyData(EEnemyRarity.Boss);

                    int templateId = (id - 1) / 10 * 4;

                    #region 미니언
                    // 미니언 템플릿
                    minion.SetTemplate(_enemyDic[templateId]);

                    // 미니언 HP
                    if (int.TryParse(csvDic["미니언 HP"][i], out var hp))
                    {
                        minion.SetHP(hp);
                    }

                    // 미니언 ATK
                    if (int.TryParse(csvDic["미니언 ATK"][i], out var atk))
                    {
                        minion.SetATK(atk);
                    }

                    // 미니언 코인
                    if (int.TryParse(csvDic["미니언 코인"][i], out var coin))
                    {
                        minion.SetCoin(coin);
                    }

                    // 미니언 골드
                    if (int.TryParse(csvDic["미니언 골드"][i], out var gold))
                    {
                        minion.SetGold(gold);
                    }

                    // 미니언 전리품
                    if (int.TryParse(csvDic["미니언 전리품"][i], out var loot))
                    {
                        minion.SetLoot(loot);
                    }
                    #endregion

                    #region 일반
                    // 일반 템플릿
                    common.SetTemplate(_enemyDic[templateId + 1]);

                    // 일반 HP
                    if (int.TryParse(csvDic["일반 HP"][i], out hp))
                    {
                        common.SetHP(hp);
                    }

                    // 일반 ATK
                    if (int.TryParse(csvDic["일반 ATK"][i], out atk))
                    {
                        common.SetATK(atk);
                    }

                    // 일반 코인
                    if (int.TryParse(csvDic["일반 코인"][i], out coin))
                    {
                        common.SetCoin(coin);
                    }

                    // 일반 골드
                    if (int.TryParse(csvDic["일반 골드"][i], out gold))
                    {
                        common.SetGold(gold);
                    }

                    // 일반 전리품
                    if (int.TryParse(csvDic["일반 전리품"][i], out loot))
                    {
                        common.SetLoot(loot);
                    }
                    #endregion

                    #region 엘리트
                    // 엘리트 템플릿
                    elite.SetTemplate(_enemyDic[templateId + 2]);

                    // 엘리트 HP
                    if (int.TryParse(csvDic["엘리트 HP"][i], out hp))
                    {
                        elite.SetHP(hp);
                    }

                    // 엘리트 ATK
                    if (int.TryParse(csvDic["엘리트 ATK"][i], out atk))
                    {
                        elite.SetATK(atk);
                    }

                    // 엘리트 크리스탈
                    if (int.TryParse(csvDic["엘리트 크리스탈"][i], out var crystal))
                    {
                        elite.SetCrystal(crystal);
                    }
                    #endregion

                    #region 보스
                    // 보스 템플릿
                    boss.SetTemplate(_enemyDic[templateId + 3]);

                    // 보스 HP
                    if (int.TryParse(csvDic["보스 HP"][i], out hp))
                    {
                        boss.SetHP(hp);
                    }

                    // 보스 ATK
                    if (int.TryParse(csvDic["보스 ATK"][i], out atk))
                    {
                        boss.SetATK(atk);
                    }

                    // 보스 크리스탈
                    if (int.TryParse(csvDic["보스 크리스탈"][i], out crystal))
                    {
                        boss.SetCrystal(crystal);
                    }
                    #endregion

                    string path = $"Assets/EvolveThisMatch/GameData/ETC/Library/Wave/Wave_{csvDic["스테이지"][i]}.asset";
                    AssetDatabase.CreateAsset(newTemplate, path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 등급 템플릿 가져오기
        private Dictionary<int, EnemyTemplate> _enemyDic = new Dictionary<int, EnemyTemplate>();

        private void InitializeEnemyTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:EnemyTemplate");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var enemyTemplate = AssetDatabase.LoadAssetAtPath<EnemyTemplate>(path);
                _enemyDic[enemyTemplate.id] = enemyTemplate;
            }
        }
        #endregion
    }
#endif
}