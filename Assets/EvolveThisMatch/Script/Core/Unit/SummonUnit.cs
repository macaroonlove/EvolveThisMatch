using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SummonUnit : AllyUnit
    {
        private SummonTemplate _template;
        private Unit _summoner;

        internal SummonTemplate template => _template;
        internal override EMoveType moveType => _template.MoveType;

        internal void Initialize(SummonTemplate template, Unit summoner)
        {
            _id = template.id;
            _template = template;
            _summoner = summoner;

            summoner.healthAbility.onDeath += OnDeath;

            base.Initialize(this);
        }

        internal override void OnDeath()
        {
            base.OnDeath();

            var allySystem = BattleManager.Instance.GetSubSystem<AllySystem>();

            allySystem.Deregist(this);

            _summoner.healthAbility.onDeath -= OnDeath;
        }
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEditor;

    [CustomEditor(typeof(SummonUnit))]
    public class SummonUnitEditor : UnitEditor
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