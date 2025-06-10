using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class InstantGetTargetUnitEffect : GetTargetUnitEffect
    {
        [SerializeField] protected FX _targetFX;

        public override string GetDescription()
        {
            return "즉시 (탐색 타겟팅)";
        }

        public override void Execute(Unit casterUnit, Unit targetUnit)
        {
            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            SkillImpact(casterUnit, targetUnit);

            ExecuteTargetFX(targetUnit);
        }

        #region FX
        private void ExecuteTargetFX(Unit target)
        {
            if (_targetFX != null)
            {
                _targetFX.Play(target);
            }
        }
        #endregion

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "대상자 FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            rect.y += 40;
            rect = _getTargetData.Draw(rect);

            _effectsList?.DoList(rect);
        }

        public override int GetNumRows()
        {
            return _getTargetData.GetNumRows(base.GetNumRows());
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
