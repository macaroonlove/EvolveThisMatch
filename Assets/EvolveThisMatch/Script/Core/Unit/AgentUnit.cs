using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentUnit : AllyUnit
    {
        private AgentBattleData _agentData;

        internal AgentTemplate template => _agentData.agentTemplate;
        internal override EMoveType moveType => template.MoveType;
        internal int level => _agentData.level;
        internal AgentRarityTemplate limit => _agentData.limit;

        internal void Initialize(AgentBattleData agentData)
        {
            _agentData = agentData;
            _id = template.id;

            base.Initialize(this);
        }

        internal override void OnDeath()
        {
            base.OnDeath();

            var allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();

            allySystem.Deregist(_agentData);
        }

        internal int GetNeedCoinToLevelUp()
        {
            return _agentData.GetNeedCoinToLevelUp();
        }

        internal void LevelUp()
        {
            _agentData.LevelUp();
        }

        internal void UpgradeLimit()
        {
            _agentData.UpgradeLimit();
        }
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEditor;

    [CustomEditor(typeof(AgentUnit))]
    public class AgentUnitEditor : UnitEditor
    {
        protected override void AddAbilityMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("공격"), false, AddAbility, typeof(AttackAbility));
            menu.AddItem(new GUIContent("피격"), false, AddAbility, typeof(HitAbility));
            menu.AddItem(new GUIContent("체력"), false, AddAbility, typeof(HealthAbility));
            menu.AddItem(new GUIContent("데미지 계산"), false, AddAbility, typeof(DamageCalculateAbility));

            menu.AddItem(new GUIContent("버프"), false, AddAbility, typeof(BuffAbility));
            menu.AddItem(new GUIContent("상태이상"), false, AddAbility, typeof(AbnormalStatusAbility));

            menu.AddItem(new GUIContent("액티브 스킬"), false, AddAbility, typeof(ActiveSkillAbility));
            menu.AddItem(new GUIContent("패시브 스킬"), false, AddAbility, typeof(PassiveSkillAbility));

            menu.AddItem(new GUIContent("추적 이동"), false, AddAbility, typeof(MoveChaseAbility));
            menu.AddItem(new GUIContent("특정 지점 이동"), false, AddAbility, typeof(MoveWayPointAbility));

            menu.AddItem(new GUIContent("객체 스폰(투사체, 덫)"), false, AddAbility, typeof(EntitySpawnAbility));
            menu.AddItem(new GUIContent("목표 찾기"), false, AddAbility, typeof(FindTargetAbility));
            menu.AddItem(new GUIContent("FX"), false, AddAbility, typeof(FXAbility));
            menu.AddItem(new GUIContent("애니메이션"), false, AddAbility, typeof(UnitAnimationAbility));

            menu.ShowAsContext();
        }
    }
}
#endif