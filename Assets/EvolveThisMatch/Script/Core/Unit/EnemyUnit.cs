using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class EnemyUnit : Unit
    {
        [SerializeField] private GlobalEvent _deathGlobalEvent;

        private bool _isIdle;

        internal EnemyData enemyData { get; private set; }

        internal void Initialize(EnemyData data, bool isIdle)
        {
            id = data.template.id;

            enemyData = data;
            _isIdle = isIdle;

            base.Initialize(this);
        }

        internal override void OnDeath()
        {
            base.OnDeath();

            var enemySystem = BattleManager.Instance.GetSubSystem<EnemySystem>();

            #region 보상 획득
            if (_isIdle)
            {
                if (enemyData.gold > 0)
                {
                    CoreManager.Instance.GetSubSystem<CurrencySystem>().AddCurrency(CurrencyType.Gold, enemyData.gold);
                }
                if (enemyData.loot > 0)
                {
                    CoreManager.Instance.GetSubSystem<CurrencySystem>().AddCurrency(CurrencyType.Loot, enemyData.loot);
                }
            }
            else
            {
                if (enemyData.coin > 0)
                {
                    BattleManager.Instance.GetSubSystem<CoinSystem>().AddCoin(enemyData.coin);
                }
                if (enemyData.crystal > 0)
                {
                    BattleManager.Instance.GetSubSystem<CrystalSystem>().AddCrystal(enemyData.crystal);
                }
            }
            #endregion

            _deathGlobalEvent?.Raise();

            enemySystem.Deregist(this);
        }
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEditor;

    [CustomEditor(typeof(EnemyUnit))]
    public class EnemyUnitEditor : UnitEditor
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

            menu.AddItem(new GUIContent("객체 스폰(투사체, 덫)"), false, AddAbility, typeof(EntitySpawnAbility));
            menu.AddItem(new GUIContent("목표 찾기"), false, AddAbility, typeof(FindTargetAbility));
            menu.AddItem(new GUIContent("FX"), false, AddAbility, typeof(FXAbility));
            menu.AddItem(new GUIContent("애니메이션"), false, AddAbility, typeof(UnitAnimationAbility));

            menu.ShowAsContext();
        }
    }
}
#endif
