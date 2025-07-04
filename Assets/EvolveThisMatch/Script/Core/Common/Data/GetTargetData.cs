using FrameWork;
using FrameWork.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    public class GetTargetData
    {
        #region 필드
        [SerializeField, Label("타겟 탐색 방식")] private EGetTargetFilter _getTargetFilter;

        [EnumCondition("_getTargetFilter", (int)EGetTargetFilter.Attackable)]
        [SerializeField, Label("공격 방식")] private EAttackType _attackType;
        
        [SerializeField, Label("대상")] private ETarget _targetType;

        [EnumCondition("_targetType", (int)ETarget.OneTargetInRange, (int)ETarget.NumTargetInRange, (int)ETarget.AllTargetInRange, (int)ETarget.AllTarget)]
        [SerializeField, Label("유닛 타입")] private EUnitType _unitType;

        [EnumCondition("_targetType", (int)ETarget.OneTargetInRange, (int)ETarget.NumTargetInRange, (int)ETarget.AllTargetInRange)]
        [SerializeField, Label("범위 타입")] private ERangeType _rangeType;

        [EnumCondition("_targetType", (int)ETarget.OneTargetInRange, (int)ETarget.NumTargetInRange, (int)ETarget.AllTargetInRange)]
        [EnumCondition("_rangeType", (int)ERangeType.Straight, (int)ERangeType.Cone)]
        [SerializeField, Label("방향")] private EDirectionType _directionType;

        [EnumCondition("_targetType", (int)ETarget.OneTargetInRange, (int)ETarget.NumTargetInRange, (int)ETarget.AllTargetInRange)]
        [EnumCondition("_rangeType", (int)ERangeType.Circle, (int)ERangeType.Straight, (int)ERangeType.Cone, (int)ERangeType.Line)]
        [SerializeField, Label("범위")] private float _range;

        [EnumCondition("_targetType", (int)ETarget.OneTargetInRange, (int)ETarget.NumTargetInRange, (int)ETarget.AllTargetInRange)]
        [EnumCondition("_rangeType", (int)ERangeType.Straight, (int)ERangeType.Cone)]
        [SerializeField, Label("보조 범위")] private float _assistantRange;

        [EnumCondition("_targetType", (int)ETarget.NumTargetInRange)]
        [SerializeField, Label("공격할 적의 수")] private int _numberOfTarget;
        #endregion

        public List<Unit> GetTarget(Unit casterUnit)
        {
            if (_targetType == ETarget.Myself) return new List<Unit> { casterUnit };

            var findTargetAbility = casterUnit.GetAbility<FindTargetAbility>();

            if (_targetType == ETarget.AllTarget)
            {
                switch (_getTargetFilter)
                {
                    case EGetTargetFilter.Attackable:
                        return findTargetAbility.FindAllAttackableTarget(_unitType, _attackType);
                    case EGetTargetFilter.Healable:
                        return findTargetAbility.FindAllHealableTarget(_unitType);
                    default:
                        return findTargetAbility.FindAllTarget(_unitType);
                }
            }
            else
            {
                int maxCount = _numberOfTarget;

                switch (_targetType)
                {
                    case ETarget.OneTargetInRange:
                        maxCount = 1;
                        break;
                    case ETarget.AllTargetInRange:
                        maxCount = int.MaxValue;
                        break;
                }

                switch (_getTargetFilter)
                {
                    case EGetTargetFilter.Attackable:
                        return GetAttackableTarget(findTargetAbility, maxCount);
                    case EGetTargetFilter.Healable:
                        return GetHealableTarget(findTargetAbility, maxCount);
                    default:
                        return GetTarget(findTargetAbility, maxCount);
                }
            }
        }

        #region 타겟 탐색
        private List<Unit> GetTarget(FindTargetAbility findTargetAbility, int maxCount)
        {
            switch (_rangeType)
            {
                case ERangeType.Circle:
                    return findTargetAbility.FindTargetInCircle(_range, _unitType, maxCount);
                case ERangeType.Straight:
                    return findTargetAbility.FindTargetInStraight(_directionType, _range, _assistantRange, _unitType, maxCount);
                case ERangeType.Cone:
                    return findTargetAbility.FindTargetInCone(_directionType, _range, (int)_assistantRange, _unitType, maxCount);
                case ERangeType.Line:
                    return null;
                default:
                    return findTargetAbility.FindAllTarget(_unitType);
            }
        }

        private List<Unit> GetAttackableTarget(FindTargetAbility findTargetAbility, int maxCount)
        {
            switch (_rangeType)
            {
                case ERangeType.Circle:
                    return findTargetAbility.FindAttackableTargetInCircle(_range, _unitType, _attackType, maxCount);
                case ERangeType.Straight:
                    return findTargetAbility.FindAttackableTargetInStraight(_directionType, _range, _assistantRange, _unitType, _attackType, maxCount);
                case ERangeType.Cone:
                    return findTargetAbility.FindAttackableTargetInCone(_directionType, _range, (int)_assistantRange, _unitType, _attackType, maxCount);
                case ERangeType.Line:
                    return null;
                default:
                    return findTargetAbility.FindAllAttackableTarget(_unitType, _attackType);
            }
        }

        private List<Unit> GetHealableTarget(FindTargetAbility findTargetAbility, int maxCount)
        {
            switch (_rangeType)
            {
                case ERangeType.Circle:
                    return findTargetAbility.FindHealableTargetInCircle(_range, _unitType, maxCount);
                case ERangeType.Straight:
                    return findTargetAbility.FindHealableTargetInStraight(_directionType, _range, _assistantRange, _unitType, maxCount);
                case ERangeType.Cone:
                    return findTargetAbility.FindHealableTargetInCone(_directionType, _range, (int)_assistantRange, _unitType, maxCount);
                case ERangeType.Line:
                    return null;
                default:
                    return findTargetAbility.FindAllHealableTarget(_unitType);
            }
        }
        #endregion

        public Rect Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "타겟 탐색 방식");
            _getTargetFilter = (EGetTargetFilter)EditorGUI.EnumPopup(valueRect, _getTargetFilter);

            if (_getTargetFilter == EGetTargetFilter.Attackable)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "공격 방식");
                _attackType = (EAttackType)EditorGUI.EnumPopup(valueRect, _attackType);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "대상");
            _targetType = (ETarget)EditorGUI.EnumPopup(valueRect, _targetType);

            if (_targetType != ETarget.Myself)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "유닛 타입");
                _unitType = (EUnitType)EditorGUI.EnumFlagsField(valueRect, _unitType);
            }

            if (_targetType != ETarget.Myself && _targetType != ETarget.AllTarget)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "범위 타입");
                _rangeType = (ERangeType)EditorGUI.EnumPopup(valueRect, _rangeType);

                if (_rangeType == ERangeType.Circle)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _range = EditorGUI.FloatField(valueRect, _range);
                }
                else if (_rangeType == ERangeType.Straight)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "방향");
                    _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위(세로)");
                    _range = EditorGUI.FloatField(valueRect, _range);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "너비(가로)");
                    _assistantRange = EditorGUI.FloatField(valueRect, _assistantRange);
                }
                else if (_rangeType == ERangeType.Cone)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "방향");
                    _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _range = EditorGUI.FloatField(valueRect, _range);

                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "각도");
                    _assistantRange = EditorGUI.IntField(valueRect, (int)_assistantRange);
                }
                else if (_rangeType == ERangeType.Line)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "범위");
                    _range = (int)EditorGUI.FloatField(valueRect, _range);
                }
            }

            if (_targetType == ETarget.NumTargetInRange)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "감지할 유닛의 수");
                _numberOfTarget = EditorGUI.IntField(valueRect, _numberOfTarget);
            }

            rect.y = labelRect.y + 40;
            return rect;
        }

        public int GetNumRows(int rowNum)
        {
            rowNum += 4;

            if (_getTargetFilter == EGetTargetFilter.Attackable)
            {
                rowNum++;
            }

            if (_targetType != ETarget.Myself)
            {
                rowNum++;
            }

            if (_targetType != ETarget.Myself && _targetType != ETarget.AllTarget)
            {
                rowNum++;

                if (_rangeType == ERangeType.Circle || _rangeType == ERangeType.Line)
                {
                    rowNum++;
                }
                else if (_rangeType == ERangeType.Straight || _rangeType == ERangeType.Cone)
                {
                    rowNum += 3;
                }
            }

            if (_targetType == ETarget.NumTargetInRange)
            {
                rowNum++;
            }

            return rowNum;
        }
    }
}