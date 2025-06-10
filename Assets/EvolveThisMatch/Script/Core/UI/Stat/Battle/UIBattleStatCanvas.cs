using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class UIBattleStatCanvas : MonoBehaviour
    {
        private UnitRayCastSystem _unitRayCastSystem;

        private List<IBattleStat> _stats = new List<IBattleStat>();

        private void Awake()
        {
            BattleManager.Instance.onBattleInitialize += Initialize;
            BattleManager.Instance.onBattleDeinitialize += Deinitialize;

            _stats = GetComponentsInChildren<IBattleStat>().ToList();
            BattleManager.Instance.onBattleManagerDestroy += Unsubscribe;
        }

        private void Unsubscribe()
        {
            _stats.Clear();

            if (BattleManager.Instance == null) return;

            BattleManager.Instance.onBattleInitialize -= Initialize;
            BattleManager.Instance.onBattleDeinitialize -= Deinitialize;
            BattleManager.Instance.onBattleManagerDestroy -= Unsubscribe;
        }

        private void Initialize()
        {
            _unitRayCastSystem = BattleManager.Instance.GetSubSystem<UnitRayCastSystem>();

            _unitRayCastSystem.onCast += ShowInfomation;
        }

        private void Deinitialize()
        {
            _unitRayCastSystem.onCast -= ShowInfomation;

            _unitRayCastSystem = null;
        }

        private void ShowInfomation(Unit unit)
        {
            foreach (var stat in _stats)
            {
                stat.Initialize(unit);
            }
        }
    }
}