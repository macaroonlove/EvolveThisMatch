using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class ElementalValue
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private float _scaleFactor;
        [SerializeField] private ElementalTemplate _elementalType;

        public EElementalType elementalType => _elementalType.type;

        #region Int
        public virtual int GetValue(int value)
        {
            if (!_enabled) return value;

            float multiplier = 1f + GetElementalLevel() * _scaleFactor;
            return Mathf.RoundToInt(value * multiplier);
        }
        #endregion

        #region Float
        public virtual float GetValue(float value)
        {
            if (!_enabled) return value;

            float multiplier = 1f + GetElementalLevel() * _scaleFactor;
            return value * multiplier;
        }
        #endregion

        private int GetElementalLevel()
        {
            return BattleManager.Instance.GetSubSystem<ElementalSystem>().GetLevel(_elementalType);
        }

#if UNITY_EDITOR
        [SerializeField] private int _previewElementalLevel = 1;

        public int GetPreviewValue(int value)
        {
            float multiplier = 1f + _previewElementalLevel * _scaleFactor;
            return Mathf.RoundToInt(value * multiplier);
        }

        public float GetPreviewValue(float value)
        {
            float multiplier = 1f + _previewElementalLevel * _scaleFactor;
            return value * multiplier;
        }

        public void ForceDisable() => _enabled = false;
        public int GetHeight() => 20 * GetNumberRows() + (_enabled ? 4 : 0);
        public int GetNumRows() => GetNumberRows() + (_enabled ? 1 : 0);
        private int GetNumberRows() => _enabled ? 4 : 1;

        public void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "속성 사용 여부", valueRect =>
            {
                _enabled = EditorGUI.Toggle(valueRect, _enabled);
            }, valueWidthMargin: 140);

            if (_enabled)
            {
                EffectDrawUtility.DrawRow(ref rect, "증가 계수", valueRect =>
                {
                    _scaleFactor = EditorGUI.FloatField(valueRect, _scaleFactor);
                });

                EffectDrawUtility.DrawRow(ref rect, "속성", valueRect =>
                {
                    _elementalType = (ElementalTemplate)EditorGUI.ObjectField(valueRect, _elementalType, typeof(ElementalTemplate), false);
                });

                EffectDrawUtility.DrawRow(ref rect, "속성 레벨 미리보기", valueRect =>
                {
                    _previewElementalLevel = EditorGUI.IntField(valueRect, _previewElementalLevel);
                });

                EditorGUI.DrawRect(new Rect(rect.x, rect.y + 2, rect.width, 1.2f), Color.gray6);
            }
        }
#endif
    }
}
