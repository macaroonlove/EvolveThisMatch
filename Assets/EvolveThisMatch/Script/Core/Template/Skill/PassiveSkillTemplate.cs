using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Skill/Passive Skill", fileName = "PassiveSkill", order = 1)]
    public class PassiveSkillTemplate : SkillTemplate
    {
        [HideInInspector]
        public List<UnitTrigger> triggers = new List<UnitTrigger>();
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using System;
    using EvolveThisMatch.Core;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(PassiveSkillTemplate)), CanEditMultipleObjects]
    public class PassiveSkillTemplateEditor : EffectEditor
    {
        private PassiveSkillTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _id;
        private SerializedProperty _displayName;
        private SerializedProperty _description;

        private ReorderableList _triggersList;
        private UnitTrigger _currentTrigger;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as PassiveSkillTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            CreateEventTriggerList();
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
            GUILayout.Label("스킬 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬 설명", GUILayout.Width(80));
            _description.stringValue = EditorGUILayout.TextArea(_description.stringValue, GUILayout.Height(50));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            DrawEventTrigger();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        private void DrawEventTrigger()
        {
            _triggersList.DoLayoutList();

            if (_currentTrigger != null)
            {
                GUILayout.Space(20);
                _currentTrigger.Draw();
                GUILayout.Space(10);
                _effectsList?.DoLayoutList();
            }
        }

        #region TriggerList
        private void InitMenu_EffectTriggers()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("상시 적용"), false, CreateEventTriggerCallback, typeof(AlwaysUnitTrigger));
            menu.AddItem(new GUIContent("기본 공격·회복 시 적용"), false, CreateEventTriggerCallback, typeof(AttackEventUnitTrigger));
            menu.AddItem(new GUIContent("피격 시 적용"), false, CreateEventTriggerCallback, typeof(HitEventUnitTrigger));
            menu.AddItem(new GUIContent("회복을 받을 시 적용"), false, CreateEventTriggerCallback, typeof(HealEventUnitTrigger));
            menu.AddItem(new GUIContent("보호막이 파괴될 시 적용"), false, CreateEventTriggerCallback, typeof(DestroyShieldEventUnitTrigger));

            menu.ShowAsContext();
        }

        private void CreateEventTriggerList()
        {
            _triggersList = SetupReorderableList("Trigger", _target.triggers, 
                (rect, x) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetLabel());
                },
                x =>
                {
                    _currentTrigger = x;
                    CreateEffectList();
                },
                () =>
                {
                    InitMenu_EffectTriggers();
                },
                x =>
                {
                    DestroyImmediate(_currentTrigger, true);
                    _currentTrigger = null;
                });
        }

        private void CreateEventTriggerCallback(object obj)
        {
            var trigger = ScriptableObject.CreateInstance((Type)obj) as UnitTrigger;
            if (trigger != null)
            {
                trigger.hideFlags = HideFlags.HideInHierarchy;
                _target.triggers.Add(trigger);

                var template = target as PassiveSkillTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(trigger, path);
                EditorUtility.SetDirty(template);
            }
        }

        #endregion

        #region EffectList
        private void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            if (_currentTrigger is AlwaysUnitTrigger)
            {
                menu.AddItem(new GUIContent("자기 자신에게 무한 지속 버프 적용"), false, CreateEffectCallback, typeof(BuffAlwaysEffect));
            }
            else
            {
                menu.AddItem(new GUIContent("즉시 스킬"), false, CreateEffectCallback, typeof(InstantUnitEffect));
                menu.AddItem(new GUIContent("투사체 스킬"), false, CreateEffectCallback, typeof(ProjectileUnitEffect));
            }

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Passive Skill Effects", _currentTrigger.effects,
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
                var element = _currentTrigger.effects[index];

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
                var element = _currentTrigger.effects[index];
                if (element == null)
                {
                    return 20;
                }
                return element.GetHeight();
            };
        }

        private void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                _currentTrigger.effects.Add(effect);

                var template = target as PassiveSkillTemplate;
                var path = AssetDatabase.GetAssetPath(template);
                AssetDatabase.AddObjectToAsset(effect, path);
                EditorUtility.SetDirty(template);
            }
        }

        private void DestroyEffect(Effect effect)
        {
            if (effect is UnitEffect unitEffect)
            {
                unitEffect.DestroyEffect();
            }
            DestroyImmediate(_currentEffect, true);
        }
        #endregion
    }
}
#endif