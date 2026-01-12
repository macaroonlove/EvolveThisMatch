using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class BuffEffectLogic : IMutableValueBindingProvider
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private MutableValue _durationMutableValue;
        [SerializeField] private ElementalValue _durationElementalValue;
        [SerializeField] private float _duration;
        [SerializeField] private bool _isProbability;
        [SerializeField] private MutableValue _probabilityMutableValue;
        [SerializeField] private ElementalValue _probabilityElementalValue;
        [SerializeField] private int _probability;
        [SerializeField] private BuffTemplate _buff;

        #region MutableValue 처리
        public void Initialize()
        {
            _durationMutableValue = new MutableValue();
            _durationElementalValue = new ElementalValue();
            _probabilityMutableValue = new MutableValue();
            _probabilityElementalValue = new ElementalValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_durationMutableValue.bindKey == bindKey)
            {
                value = _durationMutableValue.GetValueString(_duration, context);
                return true;
            }

            if (_probabilityMutableValue.bindKey == bindKey)
            {
                value = _probabilityMutableValue.GetValueString(_probability, context);
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerable<Effect> GetChildren()
        {
            if (_buff == null || _buff.effects == null) yield break;

            foreach (var effect in _buff.effects)
            {
                if (effect != null) yield return effect;
            }
        }
        #endregion

        #region 설명
        public string GetDescription()
        {
            string result = "";

            if (_isProbability)
            {
                float probability = _probabilityMutableValue.GetPreviewValue(_probability);
                probability = _probabilityElementalValue.GetPreviewValue(probability);
                result += $" {probability}%의 확률로 ";
            }

            if (_isInfinity)
            {
                result += "무한지속";
            }
            else
            {
                float duration = _durationMutableValue.GetPreviewValue(_duration);
                duration = _durationElementalValue.GetPreviewValue(duration);
                result += $"{duration}초 동안 유지되는";
            }

            if (_buff != null)
            {
                return result + $" {_buff.displayName} 버프를 부여합니다.";
            }
            else
            {
                return result + $" 버프를 부여합니다.";
            }
        }
        #endregion

        public void Execute(EffectContext context, Unit targetUnit)
        {
            if (targetUnit == null) return;
            if (TryProbability(context) == false) return;

            if (_isInfinity)
            {
                targetUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, int.MaxValue, context);
            }
            else
            {
                float duration = _durationMutableValue.GetValue(_duration, context);
                duration = _durationElementalValue.GetValue(duration);
                targetUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, duration, context);
            }
        }

        private bool TryProbability(EffectContext context)
        {
            if (_isProbability == false) return true;

            var rand = UnityEngine.Random.Range(0, 100.0f);
            int probability = _probabilityMutableValue.GetValue(_probability, context);
            probability = _probabilityElementalValue.GetValue(probability);
            return rand <= probability;
        }

        #region 그리기
#if UNITY_EDITOR
        [SerializeField] private bool _isfoldout;
        private UnityEditor.Editor _buffEditor;

        public void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "무한지속 사용 여부", valueRect =>
            {
                _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            });

            if (!_isInfinity)
            {
                rect.y += 5;
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _durationMutableValue, _durationElementalValue, "지속시간", valueRect =>
                {
                    _duration = EditorGUI.FloatField(valueRect, _duration);
                    _duration = Mathf.Max(_duration, 0);
                });
                rect.y += 5;
            }

            rect.y += 20;
            EffectDrawUtility.DrawRow(ref rect, "확률 사용 여부", valueRect =>
            {
                _isProbability = EditorGUI.Toggle(valueRect, _isProbability);
            });

            if (_isProbability)
            {
                rect.y += 5;
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _probabilityMutableValue, _probabilityElementalValue, "확률", valueRect =>
                {
                    _probability = (int)EditorGUI.Slider(valueRect, _probability, 0, 100);
                });
                rect.y += 5;
            }

            EffectDrawUtility.DrawRow(ref rect, "버프", valueRect =>
            {
                var newBuff = (BuffTemplate)EditorGUI.ObjectField(valueRect, _buff, typeof(BuffTemplate), false);

                if (newBuff != _buff)
                {
                    _buff = newBuff;
                    UnityEditor.Editor.DestroyImmediate(_buffEditor);
                    _buffEditor = null;
                }
            });

            if (_buff != null)
            {
                rect.y += 10;
                DrawBuffTemplateInspector(ref rect);
            }
        }

        private void DrawBuffTemplateInspector(ref Rect rect)
        {
            if (_buffEditor == null)
                _buffEditor = UnityEditor.Editor.CreateEditor(_buff);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            string foldoutLabel = string.IsNullOrEmpty(_buff.displayName) ? "Buff Template" : _buff.displayName;
            _isfoldout = EditorGUILayout.Foldout(_isfoldout, foldoutLabel, true);
            if (_isfoldout)
            {
                EditorGUI.indentLevel++;
                _buffEditor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        public int GetNumRows()
        {
            float rowNum = 0;

            if (!_isInfinity) rowNum += 2;
            if (_isProbability) rowNum += 2;

            rowNum += _durationMutableValue.GetNumRows();
            rowNum += _durationElementalValue.GetNumRows();
            rowNum += _probabilityMutableValue.GetNumRows();
            rowNum += _probabilityElementalValue.GetNumRows();

            return (int)rowNum;
        }
#endif
        #endregion
    }
}