using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GlobalStatusEffectLogic : IMutableValueBindingProvider
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private MutableValue _durationMutableValue;
        [SerializeField] private float _duration;
        [SerializeField] private bool _isProbability;
        [SerializeField] private MutableValue _probabilityMutableValue;
        [SerializeField] private int _probability;
        [SerializeField] private GlobalStatusTemplate _globalStatus;

        #region MutableValue 처리
        public void Initialize()
        {
            _durationMutableValue = new MutableValue();
            _probabilityMutableValue = new MutableValue();
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
            if (_globalStatus == null || _globalStatus.effects == null) yield break;

            foreach (var effect in _globalStatus.effects)
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
                result += $" {probability}%의 확률로 ";
            }

            if (_isInfinity)
            {
                result += "무한지속";
            }
            else
            {
                float duration = _durationMutableValue.GetPreviewValue(_duration);
                result += $"{duration}초 동안 유지되는";
            }

            return result + $" {_globalStatus.displayName} 전역 상태를 부여합니다.";
        }
        #endregion

        public void Execute(EffectContext context)
        {
            if (TryProbability(context) == false) return;

            if (_isInfinity)
            {
                CoreManager.Instance.GetSubSystem<GlobalStatusSystem>().ApplyGlobalStatus(_globalStatus, int.MaxValue, context);
            }
            else
            {
                float duration = _durationMutableValue.GetValue(_duration, context);
                CoreManager.Instance.GetSubSystem<GlobalStatusSystem>().ApplyGlobalStatus(_globalStatus, duration, context);
            }
        }

        private bool TryProbability(EffectContext context)
        {
            if (_isProbability == false) return true;

            var rand = UnityEngine.Random.Range(0, 100.0f);
            return rand <= _probabilityMutableValue.GetValue(_probability, context);
        }

        #region 그리기
#if UNITY_EDITOR
        [SerializeField] private bool _isfoldout;
        private UnityEditor.Editor _globalStatusEditor;

        public void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "무한지속 사용 여부", valueRect =>
            {
                _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            });

            if (!_isInfinity)
            {
                rect.y += 5;
                EffectDrawUtility.DrawBoxedMutableValue(ref rect, _durationMutableValue, "지속시간", valueRect =>
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
                EffectDrawUtility.DrawBoxedMutableValue(ref rect, _probabilityMutableValue, "확률", valueRect =>
                {
                    _probability = (int)EditorGUI.Slider(valueRect, _probability, 0, 100);
                });
                rect.y += 5;
            }

            EffectDrawUtility.DrawRow(ref rect, "전역 상태", valueRect =>
            {
                var newGlobalStatus = (GlobalStatusTemplate)EditorGUI.ObjectField(valueRect, _globalStatus, typeof(GlobalStatusTemplate), false);

                if (newGlobalStatus != _globalStatus)
                {
                    _globalStatus = newGlobalStatus;
                    UnityEditor.Editor.DestroyImmediate(_globalStatusEditor);
                    _globalStatusEditor = null;
                }
            });

            if (_globalStatus != null)
            {
                rect.y += 10;
                DrawBuffTemplateInspector(ref rect);
            }
        }

        private void DrawBuffTemplateInspector(ref Rect rect)
        {
            if (_globalStatusEditor == null)
                _globalStatusEditor = UnityEditor.Editor.CreateEditor(_globalStatus);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            string foldoutLabel = string.IsNullOrEmpty(_globalStatus.displayName) ? "Global Status Template" : _globalStatus.displayName;
            _isfoldout = EditorGUILayout.Foldout(_isfoldout, foldoutLabel, true);
            if (_isfoldout)
            {
                EditorGUI.indentLevel++;
                _globalStatusEditor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        public int GetNumRows()
        {
            float rowNum = 1;

            if (!_isInfinity) rowNum += 2;
            if (_isProbability) rowNum += 2;

            rowNum += _durationMutableValue.GetNumRows();
            rowNum += _probabilityMutableValue.GetNumRows();

            return (int)rowNum;
        }
#endif
        #endregion
    }
}