using Cysharp.Threading.Tasks;
using FrameWork;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    public class AutoLoadSaveData
    {
        #region 로드 시, 세이브 데이터를 받아올지 여부
        private const string AutoLoadSaveDataKey = "Tools.AutoLoadSaveData";

        private static bool autoLoadSaveData
        {
            get => EditorPrefs.GetBool(AutoLoadSaveDataKey, false);
            set => EditorPrefs.SetBool(AutoLoadSaveDataKey, value);
        }

        [MenuItem("Tools/AutoLoadSaveData")]
        private static void ToggleIsEditor()
        {
            autoLoadSaveData = !autoLoadSaveData;
        }

        [MenuItem("Tools/AutoLoadSaveData", true)]
        private static bool ToggleIsEditorValidate()
        {
            Menu.SetChecked("Tools/AutoLoadSaveData", autoLoadSaveData);
            return true;
        }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private async static void Login()
        {
            if (autoLoadSaveData)
            {
                await UniTask.WaitUntil(() => PersistentLoad.isLoaded);
                await SaveManager.Instance.Load_ProfileData();
            }
        }
    }
}