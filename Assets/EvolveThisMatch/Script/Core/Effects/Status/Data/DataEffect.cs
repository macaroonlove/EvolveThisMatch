using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 제약조건을 걸기 위한 클래스
    /// </summary>
    public abstract class DataEffectBase : Effect { }

    [Serializable]
    public abstract class DataEffect<T> : DataEffectBase
    {
        [SerializeField] protected MutableValue _mutableValue;
        [SerializeField] private ElementalValue _elementalValue;
        [SerializeField] protected T _value;

        #region MutableValue 처리
        public override void Initialize()
        {
            _mutableValue = new MutableValue();
            _elementalValue = new ElementalValue();
        }
        #endregion

        public abstract string GetDescription(EffectContext context);
        public abstract T GetValue(EffectContext context);
        public abstract T GetValue(EffectContext context, EffectContext contextSub);

        #region 계산 중계
        protected int GetValue(int value, EffectContext context)
        {
            value = _mutableValue.GetValue(value, context);
            value = _elementalValue.GetValue(value);
            return value;
        }

        protected int GetValue(int value, EffectContext context, EffectContext contextSub)
        {
            value = _mutableValue.GetValue(value, context, contextSub);
            value = _elementalValue.GetValue(value);
            return value;
        }

        protected float GetValue(float value, EffectContext context)
        {
            value = _mutableValue.GetValue(value, context);
            value = _elementalValue.GetValue(value);
            return value;
        }

        protected float GetValue(float value, EffectContext context, EffectContext contextSub)
        {
            value = _mutableValue.GetValue(value, context, contextSub);
            value = _elementalValue.GetValue(value);
            return value;
        }
        #endregion

#if UNITY_EDITOR
        [SerializeField] protected int _previewScaleBase = 2;

        public virtual int GetPreviewValue(int value)
        {
            value = _mutableValue.GetPreviewValue(value);
            value = _elementalValue.GetPreviewValue(value);
            return value;
        }

        public virtual float GetPreviewValue(float value)
        {
            value = _mutableValue.GetPreviewValue(value);
            value = _elementalValue.GetPreviewValue(value);
            return value;
        }

        public override int GetNumRows() => _mutableValue.GetNumRows() + _elementalValue.GetNumRows();

        public override void Draw(Rect rect)
        {
            EffectDrawUtility.DrawBoxedScaledValue(ref rect, _mutableValue, _elementalValue, "초기 값", valueRect =>
            {
                if (typeof(T) == typeof(int))
                    _value = (T)(object)EditorGUI.IntField(valueRect, (int)(object)_value);
                else if (typeof(T) == typeof(float))
                    _value = (T)(object)EditorGUI.FloatField(valueRect, (float)(object)_value);
                // 이곳에 다른 타입에 대한 처리를 추가할 수 있습니다.
            });
        }
#endif
    }
}