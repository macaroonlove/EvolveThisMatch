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

        [SerializeField] private ERangeType _rangeType;
        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private float _range;
        [SerializeField] private int _angle;

        public override string GetDescription()
        {
            return "소환수 소환 (논타겟팅)";
        }

        #region 타겟 탐색
        public override void Deliver(EffectContext effectContext, Unit casterUnit, Vector3 targetVector)
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
                case ERangeType.Line:
                    SpawnLineTrap(casterUnit, targetVector);
                    break;
            }
        }

        #region Circle
        private void SpawnCircleTrap(Unit casterUnit, Vector3 targetVector)
        {
            // 범위 내 랜덤한 위치에 생성
            var finalPosition = GetRandomFinalPositionInCircle(casterUnit);
            SpawnSummon(casterUnit, finalPosition);
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
            // 범위 내 랜덤한 위치에 생성
            var finalPosition = GetRandomFinalPositionInStraight(casterUnit);
            SpawnSummon(casterUnit, finalPosition);
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
            // 범위 내 랜덤한 위치에 생성
            var finalPosition = GetRandomFinalPositionInCone(casterUnit);
            SpawnSummon(casterUnit, finalPosition);
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
        private void SpawnLineTrap(Unit casterUnit, Vector3 targetVector)
        {
            // 범위 내 랜덤한 위치에 생성
            var finalPosition = GetRandomFinalPositionInLine();
            SpawnSummon(casterUnit, finalPosition);
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
                rowNum+=2;

                if (_rangeType == ERangeType.Cone)
                {
                    rowNum++;
                }
            }

            return rowNum;
        }


#endif
    }
}