using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Templates/Unit/Skin", fileName = "Skin_", order = 0)]
    public class SkinTemplate : ScriptableObject
    {
        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private SkinRarityTemplate _rarity;
        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;
        [HideInInspector, SerializeField] private Sprite _faceSprite;
        [HideInInspector, SerializeField] private Vector2 _faceCenterPosition;

        public AssetReference lobbyResource;
        public AssetReference battleResource;

        private SkinLobbyTemplate _lobbyTemplate;
        private SkinBattleTemplate _battleTemplate;

        private AsyncOperationHandle<SkinLobbyTemplate> _lobbyHandle;
        private AsyncOperationHandle<SkinBattleTemplate> _battleHandle;

        #region 프로퍼티
        public int id => _id;
        public SkinRarityTemplate rarity => _rarity;
        public string displayName => _displayName;
        public string description => _description;
        public Sprite faceSprite => _faceSprite;
        public Vector2 faceCenterPosition => _faceCenterPosition;

        public SkinLobbyTemplate lobbyTemplate => _lobbyTemplate;
        public SkinBattleTemplate battleTemplate => _battleTemplate;
        #endregion

        #region 값 변경 메서드
        public void SetDisplayName(string name) => _displayName = name;
        #endregion

        #region Load
        public async UniTask LoadSkinLobbyTemplate()
        {
            _lobbyHandle = Addressables.LoadAssetAsync<SkinLobbyTemplate>(lobbyResource);
            await _lobbyHandle.ToUniTask();

            if (_lobbyHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _lobbyTemplate = _lobbyHandle.Result;
            }
            else
            {
                Addressables.Release(_lobbyHandle);
                _lobbyHandle = default;
                _lobbyTemplate = null;
            }
        }

        public async UniTask LoadSkinBattleTemplate()
        {
            _battleHandle = Addressables.LoadAssetAsync<SkinBattleTemplate>(battleResource);
            await _battleHandle.ToUniTask();

            if (_battleHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _battleTemplate = _battleHandle.Result;
            }
            else
            {
                Addressables.Release(_battleHandle);
                _battleHandle = default;
                _battleTemplate = null;
            }
        }
        #endregion

        #region Release
        public void ReleaseSkinLobbyTemplate()
        {
            if (_lobbyHandle.IsValid())
            {
                Addressables.Release(_lobbyHandle);
                _lobbyHandle = default;
                _lobbyTemplate = null;
            }
        }

        public void ReleaseSkinBattleTemplate()
        {
            if (_battleHandle.IsValid())
            {
                Addressables.Release(_battleHandle);
                _battleHandle = default;
                _battleTemplate = null;
            }
        }
        #endregion

#if UNITY_EDITOR
        #region 게임 데이터 윈도우에 보여질 Editor
        public void Draw(Rect rect)
        {
            SerializedObject serializedObject = new SerializedObject(this);

            var labelRect = new Rect(rect.x + 110, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 210, rect.y, rect.width - 210, rect.height);

            _faceSprite = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, 100, 100), _faceSprite, typeof(Sprite), false);

            GUI.Label(labelRect, "스킨 식별번호");
            _id = EditorGUI.IntField(valueRect, _id);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "스킨 이름");
            _displayName = EditorGUI.TextField(valueRect, _displayName);

            labelRect.y += 20;
            valueRect.y += 20;
            valueRect.height = 60;
            GUI.Label(labelRect, "스킨 설명");
            _description = EditorGUI.TextArea(valueRect, _description);
            
            labelRect.x = rect.x;
            labelRect.y += 80;
            valueRect.y += 80;
            labelRect.width = 210;
            valueRect.height = rect.height;
            GUI.Label(labelRect, "이미지 시너지 위치");
            _faceCenterPosition = EditorGUI.Vector2Field(valueRect, GUIContent.none, _faceCenterPosition);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "로비 리소스");
            SerializedProperty lobbyProperty = serializedObject.FindProperty("lobbyResource");
            EditorGUI.PropertyField(valueRect, lobbyProperty, GUIContent.none);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "전투 리소스");
            SerializedProperty battleProperty = serializedObject.FindProperty("battleResource");
            EditorGUI.PropertyField(valueRect, battleProperty, GUIContent.none);
        }
        #endregion
#endif
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEditor;

    [CustomEditor(typeof(SkinTemplate)), CanEditMultipleObjects]
    public class SkinTemplateEditor : Editor
    {
        private SkinTemplate _target;

        private void OnEnable()
        {
            _target = target as SkinTemplate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            _target.Draw(rect);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif