using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BuffSingleUnitEffect : SingleUnitEffect, IMutableValueBindingProvider
    {
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private BuffEffectLogic _buffEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _buffEffectLogic = new BuffEffectLogic();
            _buffEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _buffEffectLogic != null && _buffEffectLogic.TryGetBindValue(bindKey, context, out value);
        }

        public override IEnumerable<Effect> GetChildren() => _buffEffectLogic.GetChildren();
        #endregion

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

            return $"시전자 유닛이 {unitLabel} 타입이라면 " + _buffEffectLogic.GetDescription();
        }

        public override void Execute(EffectContext effectContext, Unit casterUnit)
        {
            if (casterUnit == null) return;
            if (UnitCondition(casterUnit) == false) return;

            _buffEffectLogic.Execute(effectContext, casterUnit);
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
            EffectDrawUtility.DrawRow(ref rect, "유닛 타입", valueRect =>
            {
                _unitType = (EUnitType)EditorGUI.EnumFlagsField(valueRect, _unitType);
            });

            _buffEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _buffEffectLogic.GetNumRows() + 1;
        }
#endif
    }
}