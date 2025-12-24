using ScriptableObjectArchitecture;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ChangeIntVariableNoParamEffect : NoParamEffect, IMutableValueBindingProvider
    {
        [SerializeField] private ObscuredIntVariable _target;
        [SerializeField] private EOperator _operator = EOperator.Add;
        [SerializeField] private MutableValue _mutableValue;
        [SerializeField] private int _value;

        #region MutableValue 처리
        public override void Initialize()
        {
            _mutableValue = new MutableValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_mutableValue.bindKey == bindKey)
            {
                value = _mutableValue.GetValueString(_value, context);
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        public override string GetDescription()
        {
            if (_target == null)
            {
                return "변수를 넣어주세요.";
            }

            var value = _mutableValue.GetPreviewValue(_value);

            switch (_operator)
            {
                case EOperator.Add:
                    return $"{_target.name}의 값에 {value}만큼 더하기";
                case EOperator.Multiply:
                    return $"{_target.name}의 값에 {value}만큼 곱하기";
                case EOperator.Set:
                    return $"{_target.name}의 값을 {value}로 변경하기";
            }
            return "오류! 확인 필요";
        }

        public override void Execute(EffectContext effectContext)
        {
            if (_target == null) return;

            int finalValue = _mutableValue.GetValue(_value, effectContext);

            switch (_operator)
            {
                case EOperator.Add:
                    _target.Value += finalValue;
                    break;
                case EOperator.Multiply:
                    _target.Value *= finalValue;
                    break;
                case EOperator.Set:
                    _target.Value = finalValue;
                    break;
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "변수", valueRect =>
            {
                _target = EditorGUI.ObjectField(valueRect, _target, typeof(ObscuredIntVariable), false) as ObscuredIntVariable;
            });

            EffectDrawUtility.DrawRow(ref rect, "연산자", valueRect =>
            {
                _operator = (EOperator)EditorGUI.EnumPopup(valueRect, _operator);
            });

            EffectDrawUtility.DrawBoxedMutableValue(ref rect, _mutableValue, "값", valueRect =>
            {
                _value = EditorGUI.IntField(valueRect, _value);
            });
        }

        public override int GetNumRows()
        {
            int rowNum = 2;

            rowNum += _mutableValue.GetNumRows();

            return rowNum;
        }
#endif
    }
}
