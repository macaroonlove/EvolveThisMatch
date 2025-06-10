using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ProjectilePointEffect : PointEffect
    {
        protected enum ENontargetProjectileRangeType
        {
            Straight,
            Cone,
        }

        [SerializeField] private GameObject _prefab;
        [SerializeField] private ESpawnPoint _spawnPoint;
        [SerializeField] private EActiveSkillControlType _controlType;
        [SerializeField] private ENontargetProjectileRangeType _rangeType;
        [SerializeField] private EDirectionType _directionType;
        [SerializeField] private bool _isMaxRange;
        [SerializeField] private float _range;
        [SerializeField] private float _angleStep;
        [SerializeField] private int _spawnCount;

        public override string GetDescription()
        {
            return "투사체 (논타겟팅)";
        }

        #region 타겟 탐색

        public override void Execute(Unit casterUnit, Vector3 targetVector)
        {
            if (casterUnit == null) return;

            Vector3 direction;
            float distance;

            if (_controlType == EActiveSkillControlType.Instant)
            {
                FindTargetAbility.directionMap.TryGetValue(_directionType, out direction);
                distance = _range;
            }
            else
            {
                direction = (targetVector - casterUnit.transform.position).normalized;
                distance = _isMaxRange ? _range : Mathf.Min(Vector3.Distance(casterUnit.transform.position, targetVector), _range);
            }

            switch (_rangeType)
            {
                case ENontargetProjectileRangeType.Straight:
                    SpawnStraightProjectiles(casterUnit, direction, distance);
                    break;
                case ENontargetProjectileRangeType.Cone:
                    SpawnConeProjectiles(casterUnit, direction, distance);
                    break;
            }
        }

        private void SpawnStraightProjectiles(Unit casterUnit, Vector3 direction, float distance)
        {
            Vector3 finalPosition = casterUnit.transform.position + direction * distance;
            SpawnProjectile(casterUnit, finalPosition);
        }

        private void SpawnConeProjectiles(Unit casterUnit, Vector3 direction, float distance)
        {
            var casterPos = casterUnit.transform.position;
            float maxAngle = (_spawnCount - 1) * 0.5f * _angleStep;

            for (float angle = -maxAngle; angle <= maxAngle; angle += _angleStep)
            {
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 finalDirection = rotation * direction;
                Vector3 finalPosition = casterPos + finalDirection * distance;

                SpawnProjectile(casterUnit, finalPosition);
            }
        }
        #endregion

        private void SpawnProjectile(Unit casterUnit, Vector3 finalPosition)
        {
            casterUnit.GetAbility<EntitySpawnAbility>().SpawnProjectile(_prefab, _spawnPoint, finalPosition, (caster, target) => { SkillImpact(caster, target); });
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "투사체");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "투사체 생성 위치");
            _spawnPoint = (ESpawnPoint)EditorGUI.EnumPopup(valueRect, _spawnPoint);

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "스킬 조작 방식");
            _controlType = (EActiveSkillControlType)EditorGUI.EnumPopup(valueRect, _controlType);

            if (_controlType == EActiveSkillControlType.Instant)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "방향");
                _directionType = (EDirectionType)EditorGUI.EnumPopup(valueRect, _directionType);
            }
            else if (_controlType == EActiveSkillControlType.Mouse)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "최대 범위까지 발사 여부");
                _isMaxRange = EditorGUI.Toggle(valueRect, _isMaxRange);
            }

            labelRect.y += 40;
            valueRect.y += 40;
            GUI.Label(labelRect, "범위 타입");
            _rangeType = (ENontargetProjectileRangeType)EditorGUI.EnumPopup(valueRect, _rangeType);

            if (_rangeType == ENontargetProjectileRangeType.Cone)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 사이 간격");
                _angleStep = EditorGUI.FloatField(valueRect, _angleStep);

                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "투사체 개수");
                _spawnCount = EditorGUI.IntField(valueRect, _spawnCount);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "범위");
            _range = EditorGUI.FloatField(valueRect, _range);

            var listRect = new Rect(rect.x, labelRect.y + 40, rect.width, rect.height);
            _effectsList?.DoList(listRect);
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows() + 7;

            if (_controlType == EActiveSkillControlType.Instant || _controlType == EActiveSkillControlType.Mouse)
                rowNum++;

            if (_rangeType == ENontargetProjectileRangeType.Cone)
                rowNum += 2;

            return rowNum;
        }

        protected override void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("데미지 스킬"), false, CreateEffectCallback, typeof(DamageSkillEffect));
            menu.AddItem(new GUIContent("회복 스킬"), false, CreateEffectCallback, typeof(HealSkillEffect));
            menu.AddItem(new GUIContent("보호막 스킬"), false, CreateEffectCallback, typeof(ShieldSkillEffect));
            menu.AddItem(new GUIContent("버프 스킬"), false, CreateEffectCallback, typeof(BuffSkillEffect));
            menu.AddItem(new GUIContent("상태이상 스킬"), false, CreateEffectCallback, typeof(AbnormalStatusSkillEffect));
            menu.AddItem(new GUIContent("소환수 소환 스킬"), false, CreateEffectCallback, typeof(SpawnSummonSkillEffect));
            menu.AddItem(new GUIContent("덫 스킬"), false, CreateEffectCallback, typeof(TrapUnitEffect));

            menu.ShowAsContext();
        }
#endif
    }
}
