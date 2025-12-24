using EvolveThisMatch.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 어떠한 방식으로 ExecuteEffect에 전달될지 결정하는 Effect
    /// </summary>
    public abstract class DeliveryEffect : Effect
    {
        [SerializeField] protected List<Effect> _effects = new List<Effect>();

        public override IEnumerable<Effect> GetChildren() => _effects;

        protected void Resolve(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            foreach (var effect in _effects)
            {
                if (effect is UnitToUnitEffect unitToUnitEffect)
                {
                    unitToUnitEffect.Execute(effectContext, casterUnit, targetUnit);
                }
            }
        }

#if UNITY_EDITOR
        protected ReorderableList _effectsList;
        private Effect _currentEffect;

        #region EffectList
        private void OnEnable()
        {
            CreateEffectList();
        }

        private void CreateEffectList()
        {
            _effectsList = EffectEditor.SetupReorderableList("Effects", _effects,
            (rect, x) => { },
            (x) => { _currentEffect = x; },
            () => { InitMenu_Effects(); },
            (x) =>
            {
                DestroyImmediate(_currentEffect, true);
                _currentEffect = null;
                EditorUtility.SetDirty(this);
            });

            _effectsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _effects[index];
                if (element != null)
                {
                    rect.y += 2;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(rect, element.GetDescription(), EditorStyles.boldLabel);

                    EffectEditor.DrawScript(element, rect);
                    rect.y += 5 + EditorGUIUtility.singleLineHeight;

                    element.Draw(rect);

                    if (GUI.changed)
                        EditorUtility.SetDirty(element);
                }
            };

            _effectsList.elementHeightCallback = (index) => _effects[index].GetHeight();
        }

        internal void DestroyEffect()
        {
            foreach (var effect in _effects)
            {
                DestroyImmediate(effect, true);
            }
        }

        protected virtual void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("데미지 스킬"), false, CreateEffectCallback, typeof(DamageUnitToUnitEffect));
            menu.AddItem(new GUIContent("회복 스킬"), false, CreateEffectCallback, typeof(HealUnitToUnitEffect));
            menu.AddItem(new GUIContent("보호막 스킬"), false, CreateEffectCallback, typeof(ShieldUnitToUnitEffect));
            menu.AddItem(new GUIContent("버프 스킬"), false, CreateEffectCallback, typeof(BuffUnitToUnitEffect));
            menu.AddItem(new GUIContent("상태이상 스킬"), false, CreateEffectCallback, typeof(AbnormalStatusUnitToUnitEffect));
            menu.AddItem(new GUIContent("소환수 소환 스킬"), false, CreateEffectCallback, typeof(SpawnSummonUnitToUnitEffect));

            menu.ShowAsContext();
        }

        protected void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
                effect.Initialize();
                effect.hideFlags = HideFlags.HideInHierarchy;
                _effects.Add(effect);

                var path = AssetDatabase.GetAssetPath(this);
                AssetDatabase.AddObjectToAsset(effect, path);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion

        public override int GetNumRows()
        {
            int rowNum = 4;

            foreach (var effect in _effects)
            {
                rowNum += effect.GetNumRows() + 3;
            }

            return rowNum;
        }
#endif
    }
}