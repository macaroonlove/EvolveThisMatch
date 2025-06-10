using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class AttackEventHandler : MonoBehaviour
    {
        internal UnityAction onAttack;

        public void AttackEvent()
        {
            onAttack?.Invoke();
        }
    }
}