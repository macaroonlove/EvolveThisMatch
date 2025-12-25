using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AbnormalStatusByConditionGlobalEffect : NoParamEffect, IMutableValueBindingProvider
    {
        [SerializeField] private EUnitType _unitType;
        [SerializeField] private AbnormalStatusEffectLogic _abnormalStatusEffectLogic;

        #region MutableValue 처리
        public override void Initialize()
        {
            _abnormalStatusEffectLogic = new AbnormalStatusEffectLogic();
            _abnormalStatusEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _abnormalStatusEffectLogic != null && _abnormalStatusEffectLogic.TryGetBindValue(bindKey, context, out value);
        }

        public override IEnumerable<Effect> GetChildren() => _abnormalStatusEffectLogic.GetChildren();
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

            return $"{unitLabel} 유닛에게 " + _abnormalStatusEffectLogic.GetDescription();
        }

        public override void Execute(EffectContext effectContext)
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

            foreach (var unit in units)
            {
                _abnormalStatusEffectLogic.Execute(effectContext, unit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "유닛 타입", valueRect =>
            {
                _unitType = (EUnitType)EditorGUI.EnumFlagsField(valueRect, _unitType);
            });

            _abnormalStatusEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _abnormalStatusEffectLogic.GetNumRows() + 1;
        }
#endif
    }
}
