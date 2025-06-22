using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill/Active Skill", fileName = "ActiveSkill", order = 0)]
    public class ActiveSkillTemplate : SkillTemplate
    {
        [HideInInspector, SerializeField] private SkillTypeTemplate _skillType;

        [HideInInspector, SerializeField] private EActiveSkillTriggerType _skillTriggerType;

        [HideInInspector, SerializeField] private EActiveSkillTargetingType _skillTargetingType;
        [HideInInspector, SerializeField] private EUnitType _unitType;
        [HideInInspector, SerializeField] private float _skillRange;

        [HideInInspector, SerializeField] private EActiveSkillPayType _skillPayType;
        [HideInInspector, SerializeField] private int _payAmount;

        [HideInInspector, SerializeField] private float _cooldownTime;

        [HideInInspector, SerializeField] private string _parameterName;
        [HideInInspector, SerializeField] private int _parameterHash;

        [HideInInspector, SerializeField] private FX _casterFX;

        [HideInInspector]
        public List<Effect> effects = new List<Effect>();

        #region 프로퍼티
        public SkillTypeTemplate skillType => _skillType;
        public EActiveSkillTriggerType skillTriggerType => _skillTriggerType;

        public EActiveSkillTargetingType skillTargetingType => _skillTargetingType;
        public EUnitType unitType => _unitType;
        public float skillRange => _skillRange;

        public EActiveSkillPayType skillPayType => _skillPayType;
        public int payAmount => _payAmount;

        public float cooldownTime => _cooldownTime;

        public int parameterHash => _parameterHash;

        public FX casterFX => _casterFX;
        #endregion

        #region 값 변경 메서드
        internal void SetSkillType(SkillTypeTemplate type) => _skillType = type;
        internal void SetActiveSkillTriggerType(EActiveSkillTriggerType triggerType) => _skillTriggerType = triggerType;
        internal void SetActiveSkillTargetingType(EActiveSkillTargetingType targetingType) => _skillTargetingType = targetingType;
        internal void SetActiveSkillPayType(EActiveSkillPayType payType) => _skillPayType = payType;
        internal void SetPayAmount(int payAmount) => _payAmount = payAmount;
        internal void SetCooldownTime(float cooldownTime) => _cooldownTime = cooldownTime;
        internal void SetParameterHash(string parameterName)
        {
            _parameterName = parameterName;
            _parameterHash = Animator.StringToHash(parameterName);
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

    [CustomEditor(typeof(ActiveSkillTemplate)), CanEditMultipleObjects]
    public class ActiveSkillTemplateEditor : EffectEditor
    {
        private ActiveSkillTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _id;
        private SerializedProperty _skillType;
        private SerializedProperty _displayName;
        private SerializedProperty _description;

        private SerializedProperty _skillTriggerType;

        private SerializedProperty _skillTargetingType;
        private SerializedProperty _unitType;
        private SerializedProperty _skillRange;

        private SerializedProperty _skillPayType;
        private SerializedProperty _payAmount;

        private SerializedProperty _cooldownTime;

        private SerializedProperty _parameterName;
        private SerializedProperty _parameterHash;
        private SerializedProperty _casterFX;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as ActiveSkillTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _skillType = serializedObject.FindProperty("_skillType");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");

            _skillTriggerType = serializedObject.FindProperty("_skillTriggerType");

            _skillTargetingType = serializedObject.FindProperty("_skillTargetingType");
            _unitType = serializedObject.FindProperty("_unitType");
            _skillRange = serializedObject.FindProperty("_skillRange");

            _skillPayType = serializedObject.FindProperty("_skillPayType");
            _payAmount = serializedObject.FindProperty("_payAmount");

            _cooldownTime = serializedObject.FindProperty("_cooldownTime");

            _parameterName = serializedObject.FindProperty("_parameterName");
            _parameterHash = serializedObject.FindProperty("_parameterHash");
            _casterFX = serializedObject.FindProperty("_casterFX");

            CreateEffectList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(110), GUILayout.Height(110));

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("식별번호", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 타입", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_skillType, GUIContent.none);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 발동 방식", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillTriggerType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 타겟팅 방식", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillTargetingType, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_skillTargetingType.intValue == (int)EActiveSkillTargetingType.MouseTargeting)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("마우스가 인식할 스킬 범위", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_skillRange, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("마우스가 인식할 유닛 타입", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_unitType, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("소모 자원", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillPayType, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_skillPayType.intValue != (int)EActiveSkillPayType.None)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("소모량", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_payAmount, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("쿨타임", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_cooldownTime, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("애니메이션 파라미터", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_parameterName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("파라미터 해시 값", GUILayout.Width(192));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_parameterHash, GUIContent.none);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            if (GUILayout.Button("해시 값 생성"))
            {
                _parameterHash.intValue = Animator.StringToHash(_parameterName.stringValue);
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 사용 시, 시전자 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_casterFX, GUIContent.none);
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

            switch (_skillTargetingType.enumValueIndex)
            {
                case (int)EActiveSkillTargetingType.InstantTargeting:
                    menu.AddItem(new GUIContent("즉시 스킬 (탐색 타겟팅)"), false, CreateEffectCallback, typeof(InstantGetTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 스킬 (탐색 타겟팅)"), false, CreateEffectCallback, typeof(ProjectileGetTargetUnitEffect));
                    break;
                case (int)EActiveSkillTargetingType.MouseTargeting:
                    menu.AddItem(new GUIContent("즉시 스킬 (마우스 타겟팅)"), false, CreateEffectCallback, typeof(InstantMouseTargetUnitEffect));
                    menu.AddItem(new GUIContent("투사체 스킬 (마우스 타겟팅)"), false, CreateEffectCallback, typeof(ProjectileMouseTargetUnitEffect));
                    break;
                case (int)EActiveSkillTargetingType.NonTargeting:
                    menu.AddItem(new GUIContent("즉시 스킬(논타겟팅)"), false, CreateEffectCallback, typeof(InstantPointEffect));
                    menu.AddItem(new GUIContent("투사체 스킬(논타겟팅)"), false, CreateEffectCallback, typeof(ProjectilePointEffect));
                    menu.AddItem(new GUIContent("덫 스킬(논타겟팅)"), false, CreateEffectCallback, typeof(TrapPointEffect));
                    menu.AddItem(new GUIContent("소환수 소환 스킬(논타겟팅)"), false, CreateEffectCallback, typeof(SpawnSummonPointEffect));
                    break;
            }

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Active Skill Effects", _target.effects,
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
                    DestroyEffect(_currentEffect);
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

                var template = target as ActiveSkillTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }

        private void DestroyEffect(Effect effect)
        {
            if (effect is PointEffect pointEffect)
            {
                pointEffect.DestroyEffect();
            }
            else if (effect is GetTargetUnitEffect unitEffect)
            {
                unitEffect.DestroyEffect();
            }
            DestroyImmediate(_currentEffect, true);
        }
        #endregion
    }
}
#endif