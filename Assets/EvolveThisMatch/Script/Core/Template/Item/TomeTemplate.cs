using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Item/Tome", fileName = "Tome", order = 0)]
    public class TomeTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] private Sprite _sprite;

        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;

        [HideInInspector, SerializeField] private int _needCoin;
        [HideInInspector, SerializeField] private float _cooldownTime;
        [HideInInspector, SerializeField] private float _delay;
        
        [HideInInspector, SerializeField] private ETomeRangeType _rangeType;
        [HideInInspector, SerializeField] private float _range;
        [HideInInspector, SerializeField] private EUnitType _unitType;
        // 조건 추가

        [HideInInspector, SerializeField] private FX _itemFX;
        [HideInInspector, SerializeField] private FX _afterDelayFX;

        [HideInInspector]
        public List<Effect> effects = new List<Effect>();

        #region 프로퍼티
        public Sprite sprite => _sprite;

        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;

        public int needCoin => _needCoin;
        public float cooldownTime => _cooldownTime;
        public float delay => _delay;
        
        public ETomeRangeType rangeType => _rangeType;
        public float range => _range;
        public EUnitType unitType => _unitType;

        public FX itemFX => _itemFX;
        public FX afterDelayFX => _afterDelayFX;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        public void SetDisplayName(string name) => _displayName = name;
        internal void SetDescription(string desc) => _description = desc;
        internal void SetNeedCoin(int needCoin) => _needCoin = needCoin;
        internal void SetCooldownTime(float cooldownTime) => _cooldownTime = cooldownTime;
        internal void SetDelay(float delay) => _delay = delay;
        internal void SetUnitType(EUnitType unitType) => _unitType = unitType;
        internal void SetRangeType(ETomeRangeType rangeType) => _rangeType = rangeType;
        internal void SetRange(float range) => _range = range;
        #endregion
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using System;
    using EvolveThisMatch.Core;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(TomeTemplate)), CanEditMultipleObjects]
    public class TomeTemplateEditor : EffectEditor
    {
        private TomeTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _id;
        private SerializedProperty _displayName;
        private SerializedProperty _description;
        private SerializedProperty _needCoin;
        private SerializedProperty _cooldownTime;
        private SerializedProperty _delay;
        private SerializedProperty _rangeType;
        private SerializedProperty _range;
        private SerializedProperty _unitType;
        private SerializedProperty _itemFX;
        private SerializedProperty _afterDelayFX;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as TomeTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            _needCoin = serializedObject.FindProperty("_needCoin");
            _cooldownTime = serializedObject.FindProperty("_cooldownTime");
            _delay = serializedObject.FindProperty("_delay");
            _rangeType = serializedObject.FindProperty("_rangeType");
            _range = serializedObject.FindProperty("_range");
            _unitType = serializedObject.FindProperty("_unitType");
            _itemFX = serializedObject.FindProperty("_itemFX");
            _afterDelayFX = serializedObject.FindProperty("_afterDelayFX");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            
            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(96), GUILayout.Height(96));
            
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("식별번호", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("아이템 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("아이템 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("필요 코인", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_needCoin, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("쿨타임", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_cooldownTime, GUIContent.none);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("지연 시간", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_delay, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("유닛 타입", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_unitType, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_unitType.enumValueFlag != (int)EUnitType.None)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("범위 방식", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_rangeType, GUIContent.none);
                GUILayout.EndHorizontal();

                if (_rangeType.enumValueIndex == (int)ERangeType.Circle)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("범위", GUILayout.Width(192));
                    EditorGUILayout.PropertyField(_range, GUIContent.none);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("아이템 발동 시, FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_itemFX, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_delay.floatValue > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("아이템 발동 지연 시간 이후, FX", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_afterDelayFX, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            _effectsList?.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #region EffectList
        private void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            if (_target.unitType == EUnitType.None)
            {
                menu.AddItem(new GUIContent("Int 변수 변경"), false, CreateEffectCallback, typeof(ChangeIntVariableGlobalEffect));
                menu.AddItem(new GUIContent("Float 변수 변경"), false, CreateEffectCallback, typeof(ChangeFloatVariableGlobalEffect));
                menu.AddItem(new GUIContent("특정 그룹의 유닛에게 버프 적용"), false, CreateEffectCallback, typeof(BuffByConditionGlobalEffect));
                menu.AddItem(new GUIContent("전역 상태 적용"), false, CreateEffectCallback, typeof(GlobalStatusGlobalEffect));
            }
            else
            {
                menu.AddItem(new GUIContent("대상 유닛들에게 데미지 적용"), false, CreateEffectCallback, typeof(DamageTomeEffect));
                menu.AddItem(new GUIContent("대상 유닛들에게 회복 적용"), false, CreateEffectCallback, typeof(HealTomeEffect));
                menu.AddItem(new GUIContent("대상 유닛들에게 보호막 적용"), false, CreateEffectCallback, typeof(ShieldTomeEffect));
                menu.AddItem(new GUIContent("대상 유닛들에게 버프 적용"), false, CreateEffectCallback, typeof(BuffTomeEffect));
                menu.AddItem(new GUIContent("대상 유닛들에게 상태이상 적용"), false, CreateEffectCallback, typeof(AbnormalStatusTomeEffect));
            }

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Tome Effects", _target.effects,
                (rect, x) =>
                {
                },
                (x) =>
                {
                    _currentEffect = x;
                },
                () =>
                {
                    InitMenu_Effects();
                },
                (x) =>
                {
                    DestroyImmediate(_currentEffect, true);
                    _currentEffect = null;
                    EditorUtility.SetDirty(target);
                });

            _effectsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _target.effects[index];

                if (element != null)
                {
                    rect.y += 2;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    var label = element.GetDescription();
                    EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

                    DrawScript(element, rect);

                    rect.y += 5;
                    rect.y += EditorGUIUtility.singleLineHeight;

                    element.Draw(rect);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(element);
                    }
                }
            };

            _effectsList.elementHeightCallback = (index) =>
            {
                var element = _target.effects[index];
                return element.GetHeight();
            };
        }

        private void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                _target.effects.Add(effect);

                var template = target as TomeTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
        #endregion
    }
}
#endif