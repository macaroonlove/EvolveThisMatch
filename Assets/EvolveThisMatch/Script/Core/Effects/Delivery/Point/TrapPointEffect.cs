using FrameWork;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Core
{
    public class TrapPointEffect : PointEffect
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;

        [SerializeField] private EActiveSkillControlType _controlType;
        [SerializeField] private ERangeType _rangeType;
        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private float _range;
        [SerializeField] private int _angle;

        public override string GetDescription()
        {
            return "덫 (논타겟팅)";
        }

        #region 타겟 탐색
        public override void Deliver(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            switch (_rangeType)
            {
                case ERangeType.Circle:
                    SpawnCircleTrap(effectContext, casterUnit, targetVector);
                    break;
                case ERangeType.Straight:
                    SpawnStraightTrap(effectContext, casterUnit, targetVector);
                    break;
                case ERangeType.Cone:
                    SpawnConeTrap(effectContext, casterUnit, targetVector);
                    break;
                case ERangeType.Line:
                    SpawnLineTrap(effectContext, casterUnit, targetVector);
                    break;
            }
        }

        #region Circle
        private void SpawnCircleTrap(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInCircle(casterUnit);
                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
            else
            {
                var finalPosition = GetMouseFinalPosition(casterUnit, targetVector);

                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
        }

        private Vector3 GetRandomFinalPositionInCircle(Unit casterUnit)
        {
            Vector2 rand = Random.insideUnitCircle * _range;

            return casterUnit.transform.position + new Vector3(rand.x, rand.y, 0f);
        }
        #endregion

        #region Straight
        private void SpawnStraightTrap(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInStraight(casterUnit);
                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
            else
            {
                var maxPosition = GetMouseFinalPosition(casterUnit, targetVector);

                Vector3 finalPosition = casterUnit.transform.position + maxPosition;

                SpawnTrap(effectContext, casterUnit, finalPosition);
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
        private void SpawnConeTrap(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInCone(casterUnit);
                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
            else
            {
                // 해당 방향 최대 범위에 생성
                var maxPosition = GetMouseFinalPosition(casterUnit, targetVector);

                Vector3 finalPosition = casterUnit.transform.position + maxPosition;

                SpawnTrap(effectContext, casterUnit, finalPosition);
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

        #region Line
        private void SpawnLineTrap(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
        {
            if (_controlType == EActiveSkillControlType.Instant)
            {
                // 범위 내 랜덤한 위치에 생성
                var finalPosition = GetRandomFinalPositionInLine();
                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
            else
            {
                // 범위 내 가장 먼 위치에 생성
                var finalPosition = GetFinalPositionInLine(casterUnit, targetVector);
                SpawnTrap(effectContext, casterUnit, finalPosition);
            }
        }

        private Vector3 GetRandomFinalPositionInLine()
        {
            var attackRangeRenderer = BattleManager.Instance.GetSubSystem<AttackRangeRenderer>();

            var tiles = attackRangeRenderer.tiles;
            var line = tiles[Random.Range(0, tiles.Count)];
            
            Vector2 randomTile = line[Random.Range(0, line.Count)].transform.position;
            Vector2 offset = new Vector2(Random.Range(-0.65f, 0.65f), Random.Range(-0.65f, 0.65f));

            Vector2 final = randomTile + offset;

            return new Vector3(final.x, final.y, 0f);
        }

        private Vector3 GetFinalPositionInLine(Unit casterUnit, Vector3 targetVector)
        {
            Vector3 direction = (targetVector - casterUnit.transform.position).normalized;

            var attackRangeRenderer = BattleManager.Instance.GetSubSystem<AttackRangeRenderer>();
            var tiles = attackRangeRenderer.tiles;

            Vector2 bestTile = Vector2.zero;
            float bestDot = float.MinValue;
            float bestDistance = float.MinValue;

            foreach (var lines in tiles)
            {
                foreach (var tile in lines)
                {
                    Vector2 tilePos = tile.transform.position;
                    Vector3 worldPos = new Vector3(tilePos.x, tilePos.y, 0f);
                    float distance = worldPos.magnitude;
                    float dot = Vector3.Dot(worldPos.normalized, direction);

                    // 방향이 더 잘 맞는 경우 우선
                    if (dot > bestDot)
                    {
                        bestDot = dot;
                        bestDistance = distance;
                        bestTile = tilePos;
                    }
                    // 방향이 같다면 더 먼 걸 선택
                    else if (Mathf.Approximately(dot, bestDot) && distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestTile = tilePos;
                    }
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

        private void SpawnTrap(EffectContext effectContext, Unit casterUnit, Vector3 finalPosition)
        {
            if (_isInfinity)
            {
                casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, finalPosition, (caster, target) => { Resolve(effectContext, caster, target); });
            }
            else
            {
                casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, finalPosition, _duration, (caster, target) => { Resolve(effectContext, caster, target); });
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "덫 프리팹");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

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

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "범위");
            _range = EditorGUI.FloatField(valueRect, _range);

            if (_rangeType == ERangeType.Line)
            {
                _range = (int)Mathf.Clamp(_range, 1, 4);
            }

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

            var listRect = new Rect(rect.x, labelRect.y + 40, rect.width, rect.height);
            _effectsList?.DoList(listRect);
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows() + 5;

            if (_isInfinity) rowNum++;

            if (_rangeType == ERangeType.Line)
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