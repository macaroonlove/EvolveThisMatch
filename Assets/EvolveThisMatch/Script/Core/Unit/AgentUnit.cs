using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AgentUnit : AllyUnit
    {
        public AgentBattleData agentData { get; private set; }
        internal DeployAbility deployAbility { get; private set; }

        public AgentTemplate template => agentData.agentTemplate;
        public int level => agentData.level;
        public AgentRarityTemplate limit => agentData.limit;

        public void Initialize(AgentBattleData agentData)
        {
            this.agentData = agentData;
            id = template.id;

            base.Initialize(this);
            deployAbility = GetAbility<DeployAbility>();
        }

        public void Deinitialize()
        {
            base.Deinitialize();
        }

        internal override void OnDeath()
        {
            deployAbility.ReturnSortie().Forget();
        }

        public int GetNeedCoinToLevelUp()
        {
            return agentData.GetNeedCoinToLevelUp();
        }

        public void LevelUp()
        {
            agentData.LevelUp();
        }

        public void UpgradeLimit()
        {
            agentData.UpgradeLimit();
        }

        public void DestinyRecast()
        {
            agentData.DestinyRecast();
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
            menu.AddItem(new GUIContent("경계선까지 이동"), false, AddAbility, typeof(MoveBoundaryAbility));

            menu.AddItem(new GUIContent("배치 능력"), false, AddAbility, typeof(DeployAbility));

            menu.AddItem(new GUIContent("객체 스폰(투사체, 덫)"), false, AddAbility, typeof(EntitySpawnAbility));
            menu.AddItem(new GUIContent("목표 찾기"), false, AddAbility, typeof(FindTargetAbility));
            menu.AddItem(new GUIContent("FX"), false, AddAbility, typeof(FXAbility));
            menu.AddItem(new GUIContent("애니메이션"), false, AddAbility, typeof(UnitAnimationAbility));

            menu.ShowAsContext();
        }
    }
}
#endif