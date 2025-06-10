using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 조건에 맞는 능력 중, 가장 우선순위가 높은 능력만 실행
    /// </summary>
    public abstract class ConditionAbility : Ability
    {
        [SerializeField] private int _priority;

        public int priority => _priority;

        /// <summary>
        /// 실행가능 여부를 반환하는 추상 메서드
        /// </summary>
        internal abstract bool IsExecute();

        /// <summary>
        /// 능력이 다시 실행될 때 호출
        /// </summary>
        internal virtual void StartAbility()
        {

        }

        /// <summary>
        /// 능력이 중지될 때 호출
        /// </summary>
        internal virtual void StopAbility()
        {

        }
    }
}