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
        [SerializeField] protected T _value;

        #region MutableValue 처리
        public override void Initialize()
        {
            _mutableValue = new MutableValue();
        }
        #endregion

        public abstract string GetDescription(EffectContext context);
        public abstract T GetValue(EffectContext context);
        public abstract T GetValue(EffectContext context, EffectContext contextSub);

        #region 계산 중계
        protected int GetValue(int value, EffectContext context)
        {
            return _mutableValue.GetValue(value, context);
        }

        protected int GetValue(int value, EffectContext context, EffectContext contextSub)
        {
            return _mutableValue.GetValue(value, context, contextSub);
        }

        protected float GetValue(float value, EffectContext context)
        {
            return _mutableValue.GetValue(value, context);
        }

        protected float GetValue(float value, EffectContext context, EffectContext contextSub)
        {
            return _mutableValue.GetValue(value, context, contextSub);
        }
        #endregion

#if UNITY_EDITOR
        [SerializeField] protected int _previewScaleBase = 2;

        public virtual int GetPreviewValue(int value)
        {
            return _mutableValue.GetPreviewValue(value);
        }

        public virtual float GetPreviewValue(float value)
        {
            return _mutableValue.GetPreviewValue(value);
        }

        public override int GetNumRows() => _mutableValue.GetNumRows();

        public override void Draw(Rect rect)
        {
            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _mutableValue, "초기 값", valueRect =>
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