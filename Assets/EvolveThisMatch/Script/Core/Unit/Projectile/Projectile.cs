using FrameWork.Editor;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace EvolveThisMatch.Core
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Label("발사 시, 호출될 FX")] protected FX _launchFX;
        [SerializeField, Label("충돌 시, 호출될 FX")] protected FX _collisionFX;

        [Space]
        [SerializeField, Label("적을 바라볼지 여부")] protected bool _isLookTarget;

        [Space(10)]
        [SerializeField, Label("범위 적용 여부")] protected bool _isSplash;
        [ShowIf("_isSplash")]
        [SerializeField] protected GetTargetData _getTargetData;
        [EndIf]

        protected bool _isInit;
        protected UnityAction<Unit, Unit> _action;
        
        protected void DeSpawn()
        {
            CoreManager.Instance.GetSubSystem<PoolSystem>().DeSpawn(gameObject);
            _isInit = false;
        }

        #region FX
        protected void ExecuteCasterFX(Unit caster)
        {
            if (_launchFX != null)
            {
                _launchFX.Play(caster);
            }
        }

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