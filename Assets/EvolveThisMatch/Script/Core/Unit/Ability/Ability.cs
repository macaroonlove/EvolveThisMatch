using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 모든 능력들의 추상 클래스
    /// </summary>
    public abstract class Ability : MonoBehaviour
    {
        internal Unit unit { get; private set; }

        internal virtual void Initialize(Unit unit)
        {
            this.unit = unit;
        }

        internal virtual void Deinitialize()
        {

        }

        internal virtual void UpdateAbility()
        {

        }
    }
}