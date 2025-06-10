using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField, Label("감지 대상")] protected LayerMask _layerMask;
        [SerializeField, Label("시전자 사망시 파괴 여부")] protected bool _isDespawnCasterDie;
        [Space(10)]
        [SerializeField, Label("감지 시, 호출될 FX")] protected FX _collisionFX;

        protected UnityAction<Unit, Unit> _action;
        protected Unit _caster;

        internal virtual void Initialize(Unit caster, UnityAction<Unit, Unit> action)
        {
            _caster = caster;
            _action = action;

            if (_isDespawnCasterDie)
            {
                _caster.healthAbility.onDeath += DeSpawn;
            }
        }

        protected void DeSpawn()
        {
            if (_isDespawnCasterDie)
            {
                _caster.healthAbility.onDeath -= DeSpawn;
            }

            CoreManager.Instance.GetSubSystem<PoolSystem>().DeSpawn(gameObject);
        }

        #region FX
        protected void ExecuteTargetFX(Unit target)
        {
            if (_collisionFX != null)
            {
                _collisionFX.Play(target);
            }
        }
        #endregion
    }
}
