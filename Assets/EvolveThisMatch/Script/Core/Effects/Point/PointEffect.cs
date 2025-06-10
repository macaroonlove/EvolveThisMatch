using System;
using System.Collections.Generic;
using EvolveThisMatch.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 특정 지점으로 효과를 적용
    /// </summary>
    public abstract class PointEffect : Effect
    {
        [SerializeField] private List<Effect> _effects = new List<Effect>();

        public abstract void Execute(Unit casterUnit, Vector3 targetVector);

        protected void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            foreach (var effect in _effects)
            {
                if (effect is SkillEffect skillEffect)
                {
                    skillEffect.SkillImpact(casterUnit, targetUnit);
                }
                else if (effect is UnitEffect unitEffect)
                {
                    unitEffect.Execute(casterUnit, targetUnit);
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
            _effectsList = EffectEditor.SetupReorderableList("Skill Effects", _effects,
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

            menu.AddItem(new GUIContent("데미지 스킬"), false, CreateEffectCallback, typeof(DamageSkillEffect));
            menu.AddItem(new GUIContent("회복 스킬"), false, CreateEffectCallback, typeof(HealSkillEffect));
            menu.AddItem(new GUIContent("보호막 스킬"), false, CreateEffectCallback, typeof(ShieldSkillEffect));
            menu.AddItem(new GUIContent("버프 스킬"), false, CreateEffectCallback, typeof(BuffSkillEffect));
            menu.AddItem(new GUIContent("상태이상 스킬"), false, CreateEffectCallback, typeof(AbnormalStatusSkillEffect));
            menu.AddItem(new GUIContent("소환수 소환 스킬"), false, CreateEffectCallback, typeof(SpawnSummonSkillEffect));

            menu.ShowAsContext();
        }

        protected void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;

            if (effect != null)
            {
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
            int rowNum = 3;

            foreach (var effect in _effects)
            {
                rowNum += effect.GetNumRows() + 3;
            }

            return rowNum;
        }
#endif
    }
}