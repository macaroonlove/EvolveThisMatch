using EvolveThisMatch.Save;
using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public abstract class MutableDataEffect<T> : DataEffect<T>
    {
        [SerializeField] protected T _value;
        [SerializeField] protected float _scaleFactor;
        [SerializeField] protected EEffectScaleBase _scaleBase;
        [SerializeField] protected int _previewScaleBase = 2;

        public virtual int GetPreviewValue(int value)
        {
            return (int)(value + (_previewScaleBase * _scaleFactor));
        }

        public virtual float GetPreviewValue(float value)
        {
            return value + (_previewScaleBase * _scaleFactor);
        }

        public abstract string GetDescription(EffectContext context);
        public abstract T GetValue(EffectContext context);

#if UNITY_EDITOR
        public override int GetNumRows() => 2;

        public override void Draw(Rect rect)
        {
            DrawScaleFactor(rect);
        }

        protected virtual void DrawScaleFactor(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height);

            GUI.Label(labelRect, "증가 계수");
            _scaleFactor = EditorGUI.FloatField(valueRect, _scaleFactor);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "기준값");
            _scaleBase = (EEffectScaleBase)EditorGUI.EnumPopup(valueRect, _scaleBase);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "기준값 미리보기");
            _previewScaleBase = EditorGUI.IntField(valueRect, _previewScaleBase);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "초기 값");
            if (typeof(T) == typeof(int))
                _value = (T)(object)EditorGUI.IntField(valueRect, (int)(object)_value);
            else if (typeof(T) == typeof(float))
                _value = (T)(object)EditorGUI.FloatField(valueRect, (float)(object)_value);
            // 이곳에 다른 타입에 대한 처리를 추가할 수 있습니다.
        }
#endif
    }
}