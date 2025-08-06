using ScriptableObjectArchitecture;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ChangeFloatVariableGlobalEffect : GlobalEffect
    {
        [SerializeField] private ObscuredFloatVariable _target;
        [SerializeField] private EOperator _operator = EOperator.Add;
        [SerializeField] private float _value;

        public override string GetDescription()
        {
            if (_target == null)
            {
                return "변수를 넣어주세요.";
            }

            switch (_operator)
            {
                case EOperator.Add:
                    return $"{_target.name}의 값에 {_value}만큼 더하기";
                case EOperator.Multiply:
                    return $"{_target.name}의 값에 {_value}만큼 곱하기";
                case EOperator.Set:
                    return $"{_target.name}의 값을 {_value}로 변경하기";
            }
            return "오류! 확인 필요";
        }

        public override void Execute(int level)
        {
            if (_target == null) return;

            float finalValue = _value + level * 0.01f;

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
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "변수");
            _target = EditorGUI.ObjectField(valueRect, _target, typeof(ObscuredFloatVariable), false) as ObscuredFloatVariable;
            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "연산자");
            _operator = (EOperator)EditorGUI.EnumPopup(valueRect, _operator);
            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "값");
            _value = EditorGUI.FloatField(valueRect, _value);
        }

        public override int GetNumRows()
        {
            return 3;
        }
#endif
    }
}
