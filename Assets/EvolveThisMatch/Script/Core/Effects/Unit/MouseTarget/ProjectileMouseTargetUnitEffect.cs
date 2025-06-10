using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ProjectileMouseTargetUnitEffect : MouseTargetUnitEffect
    {
        [SerializeField] protected GameObject _prefab;
        [SerializeField] protected ESpawnPoint _spawnPoint;

        public override string GetDescription()
        {
            return "투사체 (마우스 타겟팅)";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            casterUnit.GetAbility<EntitySpawnAbility>().SpawnProjectile(_prefab, _spawnPoint, targetUnit, (caster, target) => { SkillImpact(caster, target); });
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "투사체 프리팹");
            _prefab = (GameObject)EditorGUI.ObjectField(valueRect, _prefab, typeof(GameObject), false);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "투사체 생성 위치");
            _spawnPoint = (ESpawnPoint)EditorGUI.EnumPopup(valueRect, _spawnPoint);

            rect.y += 60;
            _effectsList?.DoList(rect);
        }

        public override int GetNumRows()
        {
            return base.GetNumRows() + 2;
        }

        protected override void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("데미지 스킬"), false, CreateEffectCallback, typeof(DamageSkillEffect));
            menu.AddItem(new GUIContent("회복 스킬"), false, CreateEffectCallback, typeof(HealSkillEffect));
            menu.AddItem(new GUIContent("보호막 스킬"), false, CreateEffectCallback, typeof(ShieldSkillEffect));
            menu.AddItem(new GUIContent("버프 스킬"), false, CreateEffectCallback, typeof(BuffSkillEffect));
            menu.AddItem(new GUIContent("상태이상 스킬"), false, CreateEffectCallback, typeof(AbnormalStatusSkillEffect));
            menu.AddItem(new GUIContent("소환수 소환 스킬"), false, CreateEffectCallback, typeof(SpawnSummonSkillEffect));
            menu.AddItem(new GUIContent("덫 스킬"), false, CreateEffectCallback, typeof(TrapUnitEffect));

            menu.ShowAsContext();
        }
#endif
    }
}