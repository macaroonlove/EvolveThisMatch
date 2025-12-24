using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    public class ApplyTypeByAmountData : IMutableValueBindingProvider
    {
        [SerializeField] private MutableValue _mutableValue;
        [SerializeField] private EApplyType _applyType;
        [SerializeField] private float _amount;

        #region MutableValue 贸府
        public ApplyTypeByAmountData()
        {
            _mutableValue = new MutableValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_mutableValue.bindKey == bindKey)
            {
                value = _mutableValue.GetValueString(_amount, context);
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        public EApplyType applyType => _applyType;
        public float amount => _amount;

        #region 弊府扁
#if UNITY_EDITOR
        public void Draw(ref Rect rect)
        {
            EffectDrawUtility.DrawBox(ref rect, "利侩 规侥", valueRect =>
            {
                _applyType = (EApplyType)EditorGUI.EnumPopup(valueRect, _applyType);
            }, boxHeight: 40 + _mutableValue.GetHeight(), valueWidthMargin: 20);

            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _mutableValue, "蔼", valueRect =>
            {
                _amount = EditorGUI.FloatField(valueRect, _amount);
            });
        }

        public float GetNumRows() => 2.5f + _mutableValue.GetNumRows();
#endif
        #endregion
    }
}