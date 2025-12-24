using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Item/Artifact", fileName = "Artifact", order = 1)]
    public class ArtifactTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] private Sprite _sprite;

        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private string _displayName;
        [HideInInspector, SerializeField] private string _description;

        [HideInInspector, SerializeField] private FX _casterFX;

        [HideInInspector]
        public List<GameTrigger> triggers = new List<GameTrigger>();

        #region 프로퍼티
        public Sprite sprite => _sprite;

        public int id => _id;
        public string displayName => _displayName;
        public string description => _description;

        public FX casterFX => _casterFX;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        public void SetDisplayName(string name) => _displayName = name;
        internal void SetDescription(string desc) => _description = desc;
        #endregion

        #region BindKey로 값 String 받아오기
        public string GetValue(string bindKey, EffectContext context)
        {
            foreach (var trigger in triggers)
            {
                foreach (var effect in trigger.effects)
                {
                    if (TryGetBindValueRecursive(effect, bindKey, context, out var value))
                    {
                        return value;
                    }
                }
            }

            return "Error";
        }

        private bool TryGetBindValueRecursive(Effect effect, string bindKey, EffectContext context, out string value)
        {
            // 자기 자신 검사
            if (effect is IMutableValueBindingProvider provider && provider.TryGetBindValue(bindKey, context, out value))
            {
                return true;
            }

            // 자식에 또 Effect 리스트가 존재하면 검사
            foreach (var child in effect.GetChildren())
            {
                if (TryGetBindValueRecursive(child, bindKey, context, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }
        #endregion
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using System;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(ArtifactTemplate))]
    public class ArtifactTemplateEditor : EffectEditor
    {
        private ArtifactTemplate _target;

        private SerializedProperty _sprite;
        private SerializedProperty _id;
        private SerializedProperty _displayName;
        private SerializedProperty _description;
        private SerializedProperty _casterFX;

        private ReorderableList _triggersList;
        private GameTrigger _currentTrigger;

        private ReorderableList _effectsList;
        private Effect _currentEffect;

        private void OnEnable()
        {
            _target = target as ArtifactTemplate;

            _sprite = serializedObject.FindProperty("_sprite");
            _id = serializedObject.FindProperty("_id");
            _displayName = serializedObject.FindProperty("_displayName");
            _description = serializedObject.FindProperty("_description");
            _casterFX = serializedObject.FindProperty("_casterFX");

            CreateEventTriggerList();

            if (_triggersList.count > 0)
            {
                _triggersList.index = 0;
                _triggersList.onSelectCallback?.Invoke(_triggersList);
                _triggersList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (isActive)
                    {
                        EditorGUI.DrawRect(rect, new Color(0.173f, 0.365f, 0.529f, 1f));
                    }
                };
            }
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
                _currentTrigger.Draw();

                GUILayout.Space(10);
                if (_currentTrigger is UnitEventGameTrigger)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("유닛 아이템 발동 시, 시전자 FX", GUILayout.Width(192));
                    EditorGUILayout.PropertyField(_casterFX, GUIContent.none);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);
                _effectsList?.DoLayoutList();
            }
        }

        #region TriggerList
        private void InitMenu_EffectTriggers()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("획득 시"), false, CreateEventTriggerCallback, typeof(GetGameTrigger));
            menu.AddItem(new GUIContent("전투 시 상시 적용"), false, CreateEventTriggerCallback, typeof(AlwaysGameTrigger));
            menu.AddItem(new GUIContent("특정 글로벌 이벤트 발생 시"), false, CreateEventTriggerCallback, typeof(GlobalEventGameTrigger));
            menu.AddItem(new GUIContent("특정 유닛 이벤트 발생 시"), false, CreateEventTriggerCallback, typeof(UnitEventGameTrigger));

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
            var trigger = ScriptableObject.CreateInstance((Type)obj) as GameTrigger;
            if (trigger != null)
            {
                trigger.hideFlags = HideFlags.HideInHierarchy;
                _target.triggers.Add(trigger);

                var template = target as ArtifactTemplate;
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

            bool isUnitTrigger = _currentTrigger is UnitEventGameTrigger;
            bool isAlwaysTrigger = _currentTrigger is AlwaysGameTrigger;
            bool isGlobalTrigger = _currentTrigger is GlobalEventGameTrigger;

            if (isAlwaysTrigger)
            {
                menu.AddItem(new GUIContent("유닛 소환 시, 유닛이 특정 조건을 성립한다면 버프 적용"), false, CreateEffectCallback, typeof(BuffSingleUnitEffect));
                menu.AddItem(new GUIContent("유닛 소환 시, 유닛이 특정 조건을 성립한다면 상태이상 적용"), false, CreateEffectCallback, typeof(AbnormalStatusSingleUnitEffect));
            }
            else
            {
                menu.AddItem(new GUIContent("Int 변수 변경"), false, CreateEffectCallback, typeof(ChangeIntVariableNoParamEffect));
                menu.AddItem(new GUIContent("Float 변수 변경"), false, CreateEffectCallback, typeof(ChangeFloatVariableNoParamEffect));
            }

            if (isGlobalTrigger)
            {
                menu.AddItem(new GUIContent("특정 그룹의 유닛에게 버프 적용"), false, CreateEffectCallback, typeof(BuffByConditionNoParamEffect));
                menu.AddItem(new GUIContent("특정 그룹의 유닛에게 상태이상 적용"), false, CreateEffectCallback, typeof(AbnormalStatusByConditionGlobalEffect));
                menu.AddItem(new GUIContent("전역 상태 적용"), false, CreateEffectCallback, typeof(GlobalStatusNoParamEffect));
            }

            if (isUnitTrigger)
            {
                menu.AddItem(new GUIContent("즉시 효과"), false, CreateEffectCallback, typeof(InstantUnitEffect));
                menu.AddItem(new GUIContent("투사체 효과"), false, CreateEffectCallback, typeof(ProjectileUnitEffect));
            }

            menu.ShowAsContext();
        }

        private void CreateEffectList()
        {
            _effectsList = SetupReorderableList("Artifact Effects", _currentTrigger.effects,
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
                var effect = _currentTrigger.effects[index];

                if (effect != null)
                {
                    rect.y += 2;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    var label = effect.GetDescription();
                    EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

                    DrawScript(effect, rect);

                    rect.y += 5;
                    rect.y += EditorGUIUtility.singleLineHeight;

                    effect.Draw(rect);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(effect);
                    }
                }
            };

            _effectsList.elementHeightCallback = (index) =>
            {
                var effect = _currentTrigger.effects[index];

                if (effect == null)
                {
                    return 20;
                }
                return effect.GetHeight();
            };
        }

        private void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
                effect.Initialize();
                effect.hideFlags = HideFlags.HideInHierarchy;
                _currentTrigger.effects.Add(effect);

                var template = target as ArtifactTemplate;
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