using FrameWork.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 장판형 덫
    /// </summary>
    public class AreaTrap : Trap
    {
        [Space(10)]
        [SerializeField, Label("감지 간격")] private float _tickInterval = 1f;

        private HashSet<Unit> _unitsInArea = new HashSet<Unit>();
        private List<Unit> _cachedUnits = new List<Unit>();
        private WaitForSeconds _wfs;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layerMask) != 0)
            {
                if (other.TryGetComponent(out Unit targetUnit))
                {
                    _unitsInArea.Add(targetUnit);

                    UnityAction onDeath = null;
                    
                    onDeath = () =>
                    {
                        _unitsInArea.Remove(targetUnit);
                        targetUnit.healthAbility.onDeath -= onDeath;
                    };

                    targetUnit.healthAbility.onDeath += onDeath;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & _layerMask) != 0)
            {
                if (other.TryGetComponent(out Unit targetUnit))
                {
                    _unitsInArea.Remove(targetUnit);
                }
            }
        }

        private void Awake()
        {
            _wfs = new WaitForSeconds(_tickInterval);
        }

        private void OnEnable()
        {   
            StartCoroutine(ApplyEffectRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            _unitsInArea.Clear();
        }

        private IEnumerator ApplyEffectRoutine()
        {
            while (true)
            {
                _cachedUnits.Clear();
                _cachedUnits.AddRange(_unitsInArea);

                for (int i = 0; i < _cachedUnits.Count; i++)
                {
                    var unit = _cachedUnits[i];
                    if (unit != null)
                    {
                        _action?.Invoke(_caster, unit);
                        ExecuteTargetFX(unit);
                    }
                }

                yield return _wfs;
            }
        }
    }
}
