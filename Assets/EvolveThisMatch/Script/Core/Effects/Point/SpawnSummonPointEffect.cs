using FrameWork;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    public class SpawnSummonPointEffect : PointEffect
    {
        [SerializeField] private SummonTemplate _summon;
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;

        [SerializeField] private EActiveSkillControlType _controlType;
        [SerializeField] private ERangeType _rangeType;
        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private float _range;
        [SerializeField] private int _angle;
        [SerializeField] private TileRangeTemplate _tileRangeTemplate;

        public override string GetDescription()
        {
            return "소환수 소환 (논타겟팅)";
        }

        #region 타겟 탐색
        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            switch (_rangeType)
            {
                case ERangeType.Circle:
                    SpawnCircleTrap(casterUnit, targetVector);
                    break;
                case ERangeType.Straight:
                    SpawnStraightTrap(casterUnit, targetVector);
                    break;
                case ERangeType.Cone:
                    SpawnConeTrap(casterUnit, targetVector);
                    break;
                case ERangeType.Grid:
                    SpawnGridTrap(casterUnit, targetVector);
                    break;
            }
        }

        #region Circle
        private void SpawnCircleTrap(Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInCircle(casterUnit);
                SpawnSummon(casterUnit, finalPosition);
            }
            else
            {
                var finalPosition = GetMouseFinalPosition(casterUnit, targetVector);

                SpawnSummon(casterUnit, finalPosition);
            }
        }

        private Vector3 GetRandomFinalPositionInCircle(Unit casterUnit)
        {
            Vector2 rand = Random.insideUnitCircle * _range;

            return casterUnit.transform.position + new Vector3(rand.x, rand.y, 0f);
        }
        #endregion

        #region Straight
        private void SpawnStraightTrap(Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInStraight(casterUnit);
                SpawnSummon(casterUnit, finalPosition);
            }
            else
            {
                var maxPosition = GetMouseFinalPosition(casterUnit, targetVector);

                Vector3 finalPosition = casterUnit.transform.position + maxPosition;

                SpawnSummon(casterUnit, finalPosition);
            }
        }

        private Vector3 GetRandomFinalPositionInStraight(Unit casterUnit)
        {
            FindTargetAbility.directionMap.TryGetValue(_directionType, out var direction);

            float randomDistance = Random.Range(0f, _range);

            return casterUnit.transform.position + direction * randomDistance;
        }
        #endregion

        #region Cone
        private void SpawnConeTrap(Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInCone(casterUnit);
                SpawnSummon(casterUnit, finalPosition);
            }
            else
            {
                // 해당 방향 최대 범위에 생성
                var maxPosition = GetMouseFinalPosition(casterUnit, targetVector);

                Vector3 finalPosition = casterUnit.transform.position + maxPosition;

                SpawnSummon(casterUnit, finalPosition);
            }
        }

        private Vector3 GetRandomFinalPositionInCone(Unit casterUnit)
        {
            FindTargetAbility.directionMap.TryGetValue(_directionType, out var direction);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(-_angle / 2f, _angle / 2f));

            direction = rotation * direction;
            float randDist = Random.Range(0f, _range);

            return casterUnit.transform.position + direction.normalized * randDist;
        }
        #endregion

        #region Grid
        private void SpawnGridTrap(Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInGrid();
                SpawnSummon(casterUnit, finalPosition);
            }
            else
            {
                // 마우스 위치가 범위 내에 존재하는지 여부
                bool isInner = _tileRangeTemplate.range.Contains(new Vector2Int(Mathf.RoundToInt(targetVector.x), Mathf.RoundToInt(targetVector.z)));

                if (isInner)
                {
                    // 마우스 위치에 생성
                    SpawnSummon(casterUnit, targetVector);
                }
                else
                {
                    // 범위 내 가장 먼 위치에 생성
                    var finalPosition = GetMouseFinalPositionInGrid(casterUnit, targetVector);
                    SpawnSummon(casterUnit, finalPosition);
                }
            }
        }

        private Vector3 GetRandomFinalPositionInGrid()
        {
            Vector2Int randomOffset = _tileRangeTemplate.range[Random.Range(0, _tileRangeTemplate.range.Count)];

            float innerX = randomOffset.x + Random.Range(-0.5f, 0.5f);
            float innerY = randomOffset.y + Random.Range(-0.5f, 0.5f);

            return new Vector3(innerX, innerY, 0f);
        }

        private Vector3 GetMouseFinalPositionInGrid(Unit casterUnit, Vector3 targetVector)
        {
            Vector3 direction = (targetVector - casterUnit.transform.position).normalized;

            Vector2Int bestTile = Vector2Int.zero;
            float bestDot = float.MinValue;
            float bestDistance = float.MinValue;

            foreach (var tile in _tileRangeTemplate.range)
            {
                Vector3 worldPos = new Vector3(tile.x, tile.y, 0f);
                float distance = worldPos.magnitude;
                float dot = Vector3.Dot(worldPos.normalized, direction);

                // 방향이 더 잘 맞는 경우 우선
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDistance = distance;
                    bestTile = tile;
                }
                // 방향이 같다면 더 먼 걸 선택
                else if (Mathf.Approximately(dot, bestDot) && distance > bestDistance)
                {
                    bestDistance = distance;
                    bestTile = tile;
                }
            }

            return new Vector3(bestTile.x, bestTile.y, 0f) + direction * 0.5f;
        }
        #endregion

        #region 유틸리티
        private Vector3 GetMouseFinalPosition(Unit casterUnit, Vector3 targetVector)
        {
            Vector3 casterPos = casterUnit.transform.position;

            Vector3 direction = (targetVector - casterPos).normalized;
            float distance = Vector3.Distance(casterPos, targetVector);
            distance = Mathf.Min(distance, _range);

            return casterPos + direction * distance;
        }
        #endregion
        #endregion

        private void SpawnSummon(Unit casterUnit, Vector3 finalPosition)
        {
            var summonCreateSystem = BattleManager.Instance.GetSubSystem<SummonCreateSystem>();

            if (_isInfinity)
            {
                summonCreateSystem.CreateUnit(_summon, finalPosition, summoner: casterUnit);
            }
            else
            {
                summonCreateSystem.CreateUnit(_summon, finalPosition, _duration, casterUnit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "소환수");
            _summon = (SummonTemplate)EditorGUI.ObjectField(valueRect, _summon, typeof(SummonTemplate), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "무한 지속 여부");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);

            if (_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "지속시간");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "스킬 조작 방식");
            _controlType = (EActiveSkillControlType)EditorGUI.EnumPopup(valueRect, _controlType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "범위 타입");
            _rangeType = (ERangeType)EditorGUI.EnumPopup(valueRect, _rangeType);

            if (_rangeType == ERangeType.Grid)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "범위");
                _tileRangeTemplate = (TileRangeTemplate)EditorGUI.ObjectField(valueRect, _tileRangeTemplate, typeof(TileRangeTemplate), false);
            }
            else
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "범위");
                _range = EditorGUI.FloatField(valueRect, _range);

                if (_controlType == EActiveSkillControlType.Instant)
                {
                    labelRect.y += 20;
                    valueRect.y += 20;
                    GUI.Label(labelRect, "방향");
                    _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);

                    if (_rangeType == ERangeType.Cone)
                    {
                        labelRect.y += 20;
                        valueRect.y += 20;
                        GUI.Label(labelRect, "각도");
                        _angle = EditorGUI.IntField(valueRect, _angle);
                    }
                }
            }

            var listRect = new Rect(rect.x, labelRect.y + 40, rect.width, rect.height);
            _effectsList?.DoList(listRect);
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows() + 5;

            if (_isInfinity) rowNum++;

            if (_rangeType == ERangeType.Grid)
            {
                rowNum++;
            }
            else
            {
                rowNum++;

                if (_controlType == EActiveSkillControlType.Instant)
                {
                    rowNum++;

                    if (_rangeType == ERangeType.Cone)
                    {
                        rowNum++;
                    }
                }
            }

            return rowNum;
        }


#endif
    }
}