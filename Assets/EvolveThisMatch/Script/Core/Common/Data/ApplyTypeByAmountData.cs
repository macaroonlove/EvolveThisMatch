using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    public class ApplyTypeByAmountData
    {
        [SerializeField] private MutableValue _mutableValue;
        [SerializeField] private EApplyType _applyType;
        [SerializeField] private float _amount;

        public ApplyTypeByAmountData()
        {
            _mutableValue = new MutableValue();
        }

        public EApplyType applyType => _applyType;
        public float amount => _amount;

        #region 그리기
#if UNITY_EDITOR
        public void Draw(ref Rect rect)
        {
            EffectDrawUtility.DrawBox(ref rect, "적용 방식", valueRect =>
            {
                _applyType = (EApplyType)EditorGUI.EnumPopup(valueRect, _applyType);
            }, boxHeight: 40 + _mutableValue.GetHeight(), valueWidthMargin: 20);

            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _mutableValue, "값", valueRect =>
            {
                _amount = EditorGUI.FloatField(valueRect, _amount);
            });
        }

        public float GetNumRows() => 2.5f + _mutableValue.GetNumRows();
#endif
        #endregion
    }

    [System.Serializable]
    public class ApplyType_TargetOnlyByAmountData
    {
        public EApplyType_TargetOnly applyType;
        public float amount;
    }
}