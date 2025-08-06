using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffByConditionGlobalEffect : GlobalEffect
    {
        [SerializeField] private EUnitType _unitType;
        [SerializeField] protected bool _isInfinity;
        [SerializeField] protected float _duration;
        [SerializeField] protected BuffTemplate _buff;

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

        public override void Execute(int level)
        {
            List<Unit> units = new List<Unit>();

            if ((_unitType & (EUnitType.Agent | EUnitType.Summon)) != 0)
            {
                units.AddRange(BattleManager.Instance.GetSubSystem<AllySystem>().GetAllAllies(_unitType));
            }

            if ((_unitType & EUnitType.Enemy) != 0)
            {
                units.AddRange(BattleManager.Instance.GetSubSystem<EnemySystem>().GetAllEnemies());
            }

            if (_isInfinity)
            {
                foreach(var unit in units)
                {
                    unit.GetAbility<BuffAbility>().ApplyBuff(_buff, int.MaxValue, level);
                }
            }
            else
            {
                foreach (var unit in units)
                {
                    unit.GetAbility<BuffAbility>().ApplyBuff(_buff, _duration, level);
                }
            }
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
            GUI.Label(labelRect, "무한지속 사용 여부");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            if (!_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "지속시간");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "버프");
            _buff = (BuffTemplate)EditorGUI.ObjectField(valueRect, _buff, typeof(BuffTemplate), false);
        }

        public override int GetNumRows()
        {
            int rowNum = 3;

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}
