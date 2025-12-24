using System;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class DataEffectDescriptionBinding
    {
        public string bindKey;
        public Effect effect;

        public DataEffectDescriptionBinding(Effect effect)
        {
            this.effect = effect;
            this.bindKey = "";
        }
    }
}