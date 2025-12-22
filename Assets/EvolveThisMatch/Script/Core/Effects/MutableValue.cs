using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class MutableValue
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private float _scaleFactor;
        [SerializeField] private EEffectScaleBase _scaleBase;

        public virtual float GetValue(int value, EffectContext context)
        {
            if (!_enabled) return value;

            int scaleBase = context.GetScaleValue(_scaleBase);
            return value + scaleBase * _scaleFactor;
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
            });

            if (_enabled)
            {
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
