using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class BattleTest : UIBase
    {
        [SerializeField] private EnemyTemplate _enemyTemplate;

        private void Start()
        {
            BattleManager.Instance.onBattleInitialize += BattleInitialize;
        }

        private void BattleInitialize()
        {
            var damage = GetComponentInChildren<DamageTestCanvas>();
            var health = GetComponentInChildren<HealthTestCanvas>();
            var buff = GetComponentInChildren<BuffTestCanvas>();
            var abnormalStatus = GetComponentInChildren<AbnormalStatusTestCanvas>();
            var activeSkill = GetComponentInChildren<ActiveSkillTestCanvas>();
            var skillTreeTestCanvas = GetComponentInChildren<SkillTreeTestCanvas>();

            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, 0));
            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, 1));
            BattleManager.Instance.GetSubSystem<EnemySpawnSystem>().SpawnUnit(_enemyTemplate, new Vector3(3, 0, -1));

            BattleManager.Instance.GetSubSystem<UnitRayCastSystem>().onCast += (Unit unit) =>
            {
                damage?.Initialize(unit);
                health?.Initialize(unit);
                buff?.Initialize(unit);
                abnormalStatus?.Initialize(unit);
                activeSkill?.Initialize(unit);
                skillTreeTestCanvas?.Initialize(unit);
            };
        }
    }
}