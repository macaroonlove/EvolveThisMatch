using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MutableValue
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private string _bindKey;
        [SerializeField] private float _scaleFactor;
        [SerializeField] private EEffectScaleBase _scaleBase;

        public string bindKey => _bindKey;

        #region Int
        public virtual string GetValueString(int value, EffectContext context)
        {
            return GetValue(value, context, null).ToString();
        }

        public virtual int GetValue(int value, EffectContext context)
        {
            return GetValue(value, context, null);
        }

        public virtual int GetValue(int value, EffectContext context, EffectContext contextSub)
        {
            if (!_enabled) return value;

            int scaleBase = GetScaleBase(context, contextSub);
            return (int)(value + scaleBase * _scaleFactor);
        }
        #endregion

        #region Float
        public virtual string GetValueString(float value, EffectContext context)
        {
            return GetValue(value, context, null).ToString("N0");
        }

        public virtual float GetValue(float value, EffectContext context)
        {
            return GetValue(value, context, null);
        }

        public virtual float GetValue(float value, EffectContext context, EffectContext contextSub)
        {
            if (!_enabled) return value;

            int scaleBase = GetScaleBase(context, contextSub);
            return value + scaleBase * _scaleFactor;
        }
        #endregion

        protected int GetScaleBase(EffectContext context, EffectContext contextSub)
        {
            int scaleBase = context.GetScaleValue(_scaleBase);
            if (scaleBase == -2 && contextSub != null) scaleBase = contextSub.GetScaleValue(_scaleBase);
            if (scaleBase == -2) scaleBase = 1;

            return scaleBase;
        }

#if UNITY_EDITOR
        [SerializeField] private int _previewScaleBase = 2;

        public int GetPreviewValue(int value)
        {
            return (int)(value + (_previewScaleBase * _scaleFactor));
        }

        public float GetPreviewValue(float value)
        {
            return value + (_previewScaleBase * _scaleFactor);
        }

        public void ForceDisable() => _enabled = false;
        public int GetHeight() => 20 * GetNumberRows() + (_enabled ? 4 : 0);
        public int GetNumRows() => GetNumberRows() + (_enabled ? 1 : 0);
        private int GetNumberRows() => _enabled ? 4 : 1;

        public void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "가변값 사용 여부", valueRect =>
            {
                _enabled = EditorGUI.Toggle(valueRect, _enabled);
            }, valueWidthMargin: 140);

            if (_enabled)
            {
                var bindTextRect = new Rect(rect.width - 140, rect.y - 20, 160, EditorGUIUtility.singleLineHeight);
                _bindKey = EditorGUI.TextField(bindTextRect, _bindKey);

                EffectDrawUtility.DrawRow(ref rect, "증가 계수", valueRect =>
                {
                    _scaleFactor = EditorGUI.FloatField(valueRect, _scaleFactor);
                });

                EffectDrawUtility.DrawRow(ref rect, "기준값", valueRect =>
                {
                    _scaleBase = (EEffectScaleBase)EditorGUI.EnumPopup(valueRect, _scaleBase);
                });

                EffectDrawUtility.DrawRow(ref rect, "기준값 미리보기", valueRect =>
                {
                    _previewScaleBase = EditorGUI.IntField(valueRect, _previewScaleBase);
                });

                EditorGUI.DrawRect(new Rect(rect.x, rect.y + 2, rect.width, 1.2f), Color.gray6);
            }
        }
#endif
    }
}
