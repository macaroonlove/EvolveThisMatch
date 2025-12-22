using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public abstract class GameTrigger : ScriptableObject
    {
        public List<DataEffectDescriptionBinding> effects = new List<DataEffectDescriptionBinding>();

        public abstract string GetLabel();

#if UNITY_EDITOR
        public virtual void Draw() { }
#endif
    }
}