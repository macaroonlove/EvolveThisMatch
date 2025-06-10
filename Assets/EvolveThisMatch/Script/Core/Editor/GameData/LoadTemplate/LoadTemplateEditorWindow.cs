using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace EvolveThisMatch.Editor
{
    public abstract class LoadTemplateEditorWindow : EditorWindow
    {
        protected string _sheetID;
        protected string _gid;
        protected int _startId;
        protected int _endId;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("시트 ID", GUILayout.Width(50));
            _sheetID = EditorGUILayout.TextField(_sheetID);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GID", GUILayout.Width(50));
            _gid = EditorGUILayout.TextField(_gid);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("No.", GUILayout.Width(50));
            _startId = EditorGUILayout.IntField(_startId);
            EditorGUILayout.LabelField("부터", GUILayout.Width(50));
            _endId = EditorGUILayout.IntField(_endId);
            EditorGUILayout.LabelField("까지 불러오기", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("불러오기"))
            {
                LoadCSVData();
            }
        }

        #region CSV 데이터 불러오기
        private async void LoadCSVData()
        {
            EditorUtility.DisplayProgressBar("로딩 중", "Google Sheets에서 데이터를 가져오는 중...", 0f);

            try
            {
                string csvData = await LoadCSVFromGoogleSheets();
                if (!string.IsNullOrEmpty(csvData))
                {
                    var csvDic = CSVToDictionary(csvData);
                    if (csvDic.Count > 0)
                    {
                        ConvertCSVToTemplate(csvDic);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV 데이터를 불러오는 중 오류 발생: {e.Message}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private Dictionary<string, List<string>> CSVToDictionary(string data)
        {
            var lines = data.Split("\n");
            if (lines.Length < 2) return null;

            var headers = lines[0].Trim().Split(',');
            var csvDict = new Dictionary<string, List<string>>();
            
            foreach (var header in headers)
            {
                csvDict[header] = new List<string>();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var values = SplitLine(lines[i].Trim());
                
                if (values.Count != headers.Length) continue;

                for (int j = 0; j < headers.Length; j++)
                {
                    csvDict[headers[j]].Add(values[j]);
                }
            }

            return csvDict;
        }

        private List<string> SplitLine(string line)
        {
            var matches = Regex.Matches(line, @"(?:^|,)(?:""(?<val>[^""]*)""|(?<val>[^,""]*))");

            var result = new List<string>();
            foreach (Match match in matches)
            {
                result.Add(match.Groups["val"].Value);
            }
            return result;
        }

        private async UniTask<string> LoadCSVFromGoogleSheets()
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{_sheetID}/export?format=csv&gid={_gid}&range=A1:AA{_endId + 2}"))
            {
                var operation = request.SendWebRequest().ToUniTask();

                await operation;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("스프레드시트 데이터 로드 실패: " + request.error);
                    return null;
                }
            }
        }

        protected abstract void ConvertCSVToTemplate(Dictionary<string, List<string>> csvDic);
        #endregion
    }
}