using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameWork.Tooltip
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TooltipStyle _tooltipStyle;
        [SerializeField] private TipPosition _tooltipPosition;
        [SerializeField] private Vector2 _tooltipOffset;

        [HideInInspector] public TooltipData tooltipData;
        internal bool isHover = true;

        internal TooltipStyle tooltipStyle { get => _tooltipStyle; set => _tooltipStyle = value; }
        internal TipPosition tooltipPosition => _tooltipPosition;
        internal Vector2 tooltipOffset => _tooltipOffset;

#if UNITY_EDITOR
        [HideInInspector] public string prevTooltipStyle;
#endif

        private void Awake()
        {
            tooltipData.InitializeData();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (isHover == false) return;
            StartHover();
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (isHover == false) return;
            StopHover();
#endif
        }

        public void OnPointerClick(PointerEventData eventData)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            if (isHover)
            {
                StopHover();
                isHover = false;
            }
            else
            {
                StartHover();
                isHover = true;
            }
#endif
        }

        internal void StartHover()
        {
            TooltipManager.Instance.Show(this);
        }

        public void StopHover()
        {
            TooltipManager.Instance.Hide(this);
        }

        public TooltipStyle GetTooltipStyle()
        {
            return TooltipManager.Instance.GetTooltipStyle(this);
        }

        public void SetText(string parameterName, string text)
        {
            if (tooltipData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("TooltipData가 초기화되지 않았습니다.");
#endif
                return;
            }

            tooltipData.SetString(parameterName, text);
            TooltipManager.Instance.ReShow(this);
        }

        public void SetSprite(string parameterName, Sprite sprite)
        {
            if (tooltipData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("TooltipData가 초기화되지 않았습니다.");
#endif
                return;
            }

            tooltipData.SetSprite(parameterName, sprite);
            TooltipManager.Instance.ReShow(this);
        }

        public void SetAction(UnityAction action)
        {
            if (tooltipData == null)
            {
#if UNITY_EDITOR
                Debug.LogError("TooltipData가 초기화되지 않았습니다.");
#endif
                return;
            }

            tooltipData.SetAction(action);
            TooltipManager.Instance.ReShow(this);
        }
    }
}

#if UNITY_EDITOR
namespace FrameWork.Tooltip.Editor
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TooltipTrigger))]
    public class TooltipTriggerEditor : Editor
    {
        private TooltipTrigger _trigger;

        public void OnEnable()
        {
            _trigger = (TooltipTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            if (_trigger.tooltipStyle != null)
            {
                if (_trigger.prevTooltipStyle != _trigger.tooltipStyle.name)
                {
                    _trigger.tooltipData = _trigger.tooltipStyle.CreateField();

                    _trigger.prevTooltipStyle = _trigger.tooltipStyle.name;
                }

                if (_trigger.tooltipData.IsInitialize() == false)
                {
                    _trigger.tooltipData = _trigger.tooltipStyle.CreateField();
                }

                if (_trigger.tooltipData.IsInitializeData() == false)
                {
                    _trigger.tooltipData.InitializeData();
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("툴팁 데이터 설정", EditorStyles.boldLabel);

                // 문자열 데이터 처리
                var stringData = _trigger.tooltipData.getAllString;
                foreach (var key in stringData.Keys.ToList())
                {
                    EditorGUILayout.LabelField(key);
                    string newValue = EditorGUILayout.TextArea(
                        stringData[key],
                        GUILayout.Height(50),
                        GUILayout.ExpandHeight(true)
                    );
                    if (newValue != stringData[key])
                    {
                        _trigger.tooltipData.SetStringEditor(key, newValue);
                        GUI.changed = true;
                    }
                }

                // 스프라이트 데이터 처리
                var spriteData = _trigger.tooltipData.getAllSprite;
                foreach (var key in spriteData.Keys.ToList())
                {
                    Sprite newValue = (Sprite)EditorGUILayout.ObjectField(key, spriteData[key], typeof(Sprite), false);
                    if (newValue != spriteData[key])
                    {
                        _trigger.tooltipData.SetSpriteEditor(key, newValue);
                        GUI.changed = true;
                    }
                }

                EditorGUILayout.EndVertical();

                if (GUI.changed)
                {
                    _trigger.tooltipData.InitializeData();
                    EditorUtility.SetDirty(_trigger);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("툴팁 스타일을 등록해주세요.", MessageType.Info);
            }
        }
    }
}
#endif