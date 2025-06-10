using FrameWork.Editor;
using UnityEngine;
using VInspector;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 폭발형 덫
    /// </summary>
    public class ExplosionTrap : Trap
    {
        [Space(10)]
        [SerializeField, Label("범위 적용 여부")] private bool _isSplash;
        [ShowIf("_isSplash")]
        [SerializeField] private GetTargetData _getTargetData;
        [EndIf]

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layerMask) != 0)
            {
                if (other.TryGetComponent(out Unit targetUnit))
                {
                    OnCollision(targetUnit);
                }
            }
        }

        private void OnCollision(Unit target)
        {
            if (_isSplash)
            {
                var targets = _getTargetData.GetTarget(target);

                for (int i = 0; i < targets.Count; i++)
                {
                    _action?.Invoke(_caster, targets[i]);
                }
            }
            else
            {
                _action?.Invoke(_caster, target);
            }

            ExecuteTargetFX(target);

            DeSpawn();
        }
    }
}
