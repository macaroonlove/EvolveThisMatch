using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class BlockSystem : MonoBehaviour, IBattleSystem
    {
        private Dictionary<Unit, BarricadeController> _blockUnits = new Dictionary<Unit, BarricadeController>();
        private BarricadeController[] _barricades;

        public int blockCount => _blockUnits.Count;

        public event UnityAction<int> onChangedBlockCount;

        private void Awake()
        {
            _barricades = GetComponentsInChildren<BarricadeController>();
        }

        public void Initialize()
        {
            
        }

        public void Deinitialize()
        {
            
        }

        internal void Regist(Unit unit)
        {
            if (_blockUnits.ContainsKey(unit)) return;

            BarricadeController barricade = FindNearestBarricade(unit);
            if (barricade == null) return;

            _blockUnits.Add(unit, barricade);
            
            barricade.Regist(unit);

            onChangedBlockCount?.Invoke(blockCount);
        }

        internal void Deregist(Unit unit)
        {
            if (_blockUnits.TryGetValue(unit, out var barricade))
            {
                _blockUnits.Remove(unit);

                barricade.Deregist(unit);

                onChangedBlockCount?.Invoke(blockCount);
            }
        }

        private BarricadeController FindNearestBarricade(Unit unit)
        {
            if (_barricades.Length == 0) return null;
            
            float unitY = unit.transform.position.y;
            unitY = Mathf.Round(unitY * 10f) * 0.1f;
            
            foreach (var barricade in _barricades)
            {
                if (barricade.ContainsRange(unitY))
                {
                    return barricade;
                }
            }

            return null;
        }

        public void BattleEnd()
        {
            // TODO: 바리케이드가 부셔지는 효과
        }
    }
}