using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class GlobalStatusNoParamEffect : NoParamEffect, IMutableValueBindingProvider
    {
        [SerializeField] private GlobalStatusEffectLogic _globalStatusEffectLogic;

        #region MutableValue 贸府
        public override void Initialize()
        {
            _globalStatusEffectLogic = new GlobalStatusEffectLogic();
            _globalStatusEffectLogic.Initialize();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            value = null;

            return _globalStatusEffectLogic != null && _globalStatusEffectLogic.TryGetBindValue(bindKey, context, out value);
        }
        #endregion

        public override string GetDescription()
        {
            return "傈开 惑怕 利侩";
        }

        public override void Execute(EffectContext effectContext)
        {
            _globalStatusEffectLogic.Execute(effectContext);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            _globalStatusEffectLogic.Draw(rect);
        }

        public override int GetNumRows()
        {
            return _globalStatusEffectLogic.GetNumRows();
        }
#endif
    }
}