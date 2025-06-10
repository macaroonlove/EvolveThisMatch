using FrameWork.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Status/AbnormalStatus", fileName = "AbnormalStatus", order = 1)]
    public class AbnormalStatusTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] private Sprite _sprite;

        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;

        [HideInInspector, SerializeField] private float _delay;

        [HideInInspector, SerializeField] private bool _useHitCountLimit;
        [HideInInspector, SerializeField] private int _hitCount;

        [HideInInspector]
        public List<Effect> effects = new List<Effect>();

        [HideInInspector, SerializeField] private FX _applyFX;
        [HideInInspector, SerializeField] private FX _removeFX;

        #region 프로퍼티
        public Sprite sprite => _sprite;
        public int id => 0;
        public string displayName => _displayName;
        public string description => _description;

        public float delay => _delay;

        public bool useHitCountLimit => _useHitCountLimit;
        public int hitCount => _hitCount;

        public FX applyFX => _applyFX;
        public FX removeFX => _removeFX;
        #endregion

        #region 값 변경 메서드
        public void SetDisplayName(string name)
        {
            _displayName = name;
        }
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

    [CustomEditor(typeof(AbnormalStatusTemplate)), CanEditMultipleObjects]
    public class AbnormalStatusTemplateEditor : EffectEditor
    {
        private AbnormalStatusTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _displayName;
        private SerializedProperty _description;
        private SerializedProperty _delay;
        private SerializedProperty _useHitCountLimit;
        private SerializedProperty _hitCount;
        private SerializedProperty _applyFX;
        private SerializedProperty _removeFX;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as AbnormalStatusTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            _delay = serializedObject.FindProperty("_delay");
            _useHitCountLimit = serializedObject.FindProperty("_useHitCountLimit");
            _hitCount = serializedObject.FindProperty("_hitCount");
            _applyFX = serializedObject.FindProperty("_applyFX");
            _removeFX = serializedObject.FindProperty("_removeFX");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(96), GUILayout.Height(96));

            EditorGUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("상태이상 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("상태이상 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(74));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("지연 시간", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_delay, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("피격 시, 상태이상이 해제될지", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_useHitCountLimit, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_useHitCountLimit.boolValue == true)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("피격 횟수", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_hitCount, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("상태이상 적용 시 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_applyFX, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("상태이상 해제 시 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_removeFX, GUIContent.none);
            GUILayout.EndHorizontal();

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

            menu.AddItem(new GUIContent("이동 불가"), false, CreateEffectCallback, typeof(UnableToMoveEffect));
            menu.AddItem(new GUIContent("공격 불가"), false, CreateEffectCallback, typeof(UnableToAttackEffect));
            menu.AddItem(new GUIContent("회복 불가"), false, CreateEffectCallback, typeof(UnableToHealEffect));
            menu.AddItem(new GUIContent("스킬 사용 불가"), false, CreateEffectCallback, typeof(UnableToSkillEffect));
            menu.AddItem(new GUIContent("이동속도 증감"), false, CreateEffectCallback, typeof(MoveIncreaseDataEffect));
            menu.AddItem(new GUIContent("물리 저항력 증감"), false, CreateEffectCallback, typeof(PhysicalResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("마법 저항력 증감"), false, CreateEffectCallback, typeof(MagicResistanceIncreaseDataEffect));
            menu.AddItem(new GUIContent("받는 피해량 증감"), false, CreateEffectCallback, typeof(ReceiveDamageIncreaseDataEffect));
            menu.AddItem(new GUIContent("최대 체력 비례 초당 체력 회복량"), false, CreateEffectCallback, typeof(HPRecoveryPerSecByMaxHPIncreaseDataEffect));

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Abnormal Status Effects", _target.effects,
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

                var template = target as AbnormalStatusTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }
        #endregion
    }
}
#endif