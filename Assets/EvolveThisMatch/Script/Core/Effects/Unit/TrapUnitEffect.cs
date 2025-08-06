using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TrapUnitEffect : UnitEffect
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;

        public override string GetDescription()
        {
            return "µ£";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit, int level)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            if (_isInfinity)
            {
                casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, targetUnit.transform.position, (caster, target) => { SkillImpact(caster, target, level); });
            }
            else
            {
                casterUnit.GetAbility<EntitySpawnAbility>().SpawnTrap(_prefab, targetUnit.transform.position, _duration, (caster, target) => { SkillImpact(caster, target, level); });
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "µ£ ÇÁ¸®ÆÕ");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "¹«ÇÑ Áö¼Ó ¿©ºÎ");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);

            if (_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "Áö¼Ó½Ã°£");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            rect.y = labelRect.y + 40;
            _effectsList?.DoList(rect);
        }

        public override int GetNumRows()
        {
            int rowNum = 2;

            if (_isInfinity) rowNum++;

            rowNum += base.GetNumRows();

            return rowNum;
        }
#endif
    }
}