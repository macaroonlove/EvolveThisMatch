using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffByConditionAlwaysEffect : AlwaysEffect
    {
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private BuffTemplate _buff;

        public override string GetDescription()
        {
            string unitLabel = "";

            if ((_unitType & EUnitType.Agent) != 0)
                unitLabel += "아군, ";

            if ((_unitType & EUnitType.Summon) != 0)
                unitLabel += "소환수, ";

            if ((_unitType & EUnitType.Enemy) != 0)
                unitLabel += "적군, ";

            if (string.IsNullOrEmpty(unitLabel))
                unitLabel = "모든";
            else
                unitLabel = unitLabel.Substring(0, unitLabel.Length - 2);

            return $"{unitLabel} 유닛에게 무한 지속 버프 적용";
        }

        public override void Execute(Unit casterUnit)
        {
            if (casterUnit == null) return;
            if (UnitCondition(casterUnit) == false) return;
            // 조건 추가

            casterUnit.GetAbility<BuffAbility>().ApplyBuff(_buff, int.MaxValue);
        }

        private bool UnitCondition(Unit unit)
        {
            if ((_unitType & EUnitType.Agent) != 0 && unit is AgentUnit)
                return true;

            if ((_unitType & EUnitType.Summon) != 0 && unit is SummonUnit)
                return true;

            if ((_unitType & EUnitType.Enemy) != 0 && unit is EnemyUnit)
                return true;

            return false;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "유닛 타입");
            _unitType = (EUnitType)EditorGUI.EnumFlagsField(valueRect, _unitType);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "버프");
            _buff = (BuffTemplate)EditorGUI.ObjectField(valueRect, _buff, typeof(BuffTemplate), false);
        }

        public override int GetNumRows()
        {
            return 2;
        }
#endif
    }
}