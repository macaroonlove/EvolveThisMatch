using FrameWork;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class InstantPointEffect : PointEffect
    {
        [SerializeField] private ETarget _targetType;
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private ERangeType _rangeType;

        [SerializeField] private EActiveSkillControlType _controlType;
        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private float _range;
        [SerializeField] private float _assistantRange;

        [SerializeField] private int _numberOfTarget;

        [SerializeField] private FX _targetFX;

        public override string GetDescription()
        {
            return "즉시 (논타겟팅)";
        }

        #region 타겟 탐색
        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

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

            switch (_rangeType)
            {
                case ERangeType.Circle:
                    GetTargetInCircle(casterUnit, maxCount);
                    break;
                case ERangeType.Straight:
                    GetTargetStraight(casterUnit, targetVector, maxCount);
                    break;
                case ERangeType.Cone:
                    GetTargetCone(casterUnit, targetVector, maxCount);
                    break;
                case ERangeType.Line:
                    GetTargetGrid(casterUnit, maxCount);
                    break;
                default:
                    GetAllTarget(casterUnit);
                    break;
            }
        }

        private void GetAllTarget(Unit casterUnit)
        {
            var targets = casterUnit.GetAbility<FindTargetAbility>().FindAllTarget(_unitType);

            foreach (var target in targets)
            {
                SkillImpact(casterUnit, target);

                ExecuteTargetFX(target);
            }
        }

        private void GetTargetInCircle(Unit casterUnit, int maxCount)
        {
            var targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInCircle(_range, _unitType, maxCount);

            foreach (var target in targets)
            {
                SkillImpact(casterUnit, target);

                ExecuteTargetFX(target);
            }
        }

        private void GetTargetStraight(Unit casterUnit, Vector3 targetVector, int maxCount)
        {
            List<Unit> targets;
            if (_controlType == EActiveSkillControlType.Instant)
            {
                targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInStraight(_directionType, _range, _assistantRange, _unitType, maxCount);
            }
            else
            {
                var direction = (casterUnit.transform.position - targetVector).normalized;
                targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInStraight(direction, _range, _assistantRange, _unitType, maxCount);
            }

            foreach (var target in targets)
            {
                SkillImpact(casterUnit, target);

                ExecuteTargetFX(target);
            }
        }

        private void GetTargetCone(Unit casterUnit, Vector3 targetVector, int maxCount)
        {
            List<Unit> targets;
            if (_controlType == EActiveSkillControlType.Instant)
            {
                targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInCone(_directionType, _range, (int)_assistantRange, _unitType, maxCount);
            }
            else
            {
                var direction = (casterUnit.transform.position - targetVector).normalized;
                targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInCone(direction, _range, (int)_assistantRange, _unitType, maxCount);
            }

            foreach (var target in targets)
            {
                SkillImpact(casterUnit, target);

                ExecuteTargetFX(target);
            }
        }

        private void GetTargetGrid(Unit casterUnit, int maxCount)
        {
            //var targets = casterUnit.GetAbility<FindTargetAbility>().FindTargetInGrid(_tileRangeTemplate.range, _unitType, maxCount);

            //foreach (var target in targets)
            //{
            //    SkillImpact(casterUnit, target);

            //    ExecuteTargetFX(target);
            //}
        }
        #endregion

        #region FX
        private void ExecuteTargetFX(Unit target)
        {
            if (_targetFX != null)
            {
                _targetFX.Play(target);
            }
        }
        #endregion

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "대상자 FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            labelRect.y += 40;
            valueRect.y += 40;
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
                    GUI.Label(labelRect, "스킬 조작 방식");
                    _controlType = (EActiveSkillControlType)EditorGUI.EnumPopup(valueRect, _controlType);

                    if (_controlType == EActiveSkillControlType.Instant)
                    {
                        labelRect.y += 20;
                        valueRect.y += 20;
                        GUI.Label(labelRect, "방향");
                        _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);
                    }

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
                    GUI.Label(labelRect, "스킬 조작 방식");
                    _controlType = (EActiveSkillControlType)EditorGUI.EnumPopup(valueRect, _controlType);

                    if (_controlType == EActiveSkillControlType.Instant)
                    {
                        labelRect.y += 20;
                        valueRect.y += 20;
                        GUI.Label(labelRect, "방향");
                        _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);
                    }

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
                    _range = EditorGUI.FloatField(valueRect, _range);
                    _range = (int)Mathf.Clamp(_range, 0, 4);
                }
            }

            if (_targetType == ETarget.NumTargetInRange)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "감지할 유닛의 수");
                _numberOfTarget = EditorGUI.IntField(valueRect, _numberOfTarget);
            }

            var listRect = new Rect(rect.x, labelRect.y + 40, rect.width, rect.height);
            _effectsList?.DoList(listRect);
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows() + 3;

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
#endif
    }
}