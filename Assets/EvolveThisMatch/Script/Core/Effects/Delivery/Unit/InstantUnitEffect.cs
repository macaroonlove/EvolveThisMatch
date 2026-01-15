using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class InstantUnitEffect : UnitEffect
    {
        [SerializeField] protected FX _targetFX;
        [SerializeField] protected bool _isCaster;

        public override string GetDescription()
        {
            return "즉시";
        }

        public override void Deliver(EffectContext effectContext, Unit casterUnit, Unit targetUnit)
        {
            if (_isCaster) targetUnit = casterUnit;

            ExecuteTargetFX(targetUnit);

            if (casterUnit == null || targetUnit == null) return;
            if (targetUnit.isDie) return;

            Resolve(effectContext, casterUnit, targetUnit);
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

            GUI.Label(labelRect, "시전자에게 적용");
            _isCaster = EditorGUI.Toggle(valueRect, _isCaster);

            labelRect.y += 20;
            valueRect.y += 20;

            GUI.Label(labelRect, "대상자 FX");
            _targetFX = (FX)EditorGUI.ObjectField(valueRect, _targetFX, typeof(FX), false);

            rect.y += 40;
            _effectsList?.DoList(rect);
        }

        protected override void InitMenu_Effects()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("데미지 스킬"), false, CreateEffectCallback, typeof(DamageUnitToUnitEffect));
            menu.AddItem(new GUIContent("회복 스킬"), false, CreateEffectCallback, typeof(HealUnitToUnitEffect));
            menu.AddItem(new GUIContent("보호막 스킬"), false, CreateEffectCallback, typeof(ShieldUnitToUnitEffect));
            menu.AddItem(new GUIContent("버프 스킬"), false, CreateEffectCallback, typeof(BuffUnitToUnitEffect));
            menu.AddItem(new GUIContent("상태이상 스킬"), false, CreateEffectCallback, typeof(AbnormalStatusUnitToUnitEffect));
            menu.AddItem(new GUIContent("덫 스킬"), false, CreateEffectCallback, typeof(TrapUnitEffect));

            menu.ShowAsContext();
        }
#endif
    }
}
