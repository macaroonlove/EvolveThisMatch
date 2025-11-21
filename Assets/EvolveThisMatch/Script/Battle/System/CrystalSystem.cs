using EvolveThisMatch.Core;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 전투에서 사용하는 Crystal 값을 관리하는 클래스
    /// </summary>
    public class CrystalSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private ObscuredIntVariable _crystalVariable;

        public int currentCrystal => _crystalVariable.Value;

        public event UnityAction<int> onChangedCrystal;

        public void Initialize()
        {
            SetCrystal(BattleManager.Instance.GetSubSystem<BattleWaveSystem>().waveCategory.startCrystal);

            if (BattleContext.originCrystal) AddCrystal(1);

            BattleManager.Instance.GetSubSystem<AgentReturnSystem>().onReturnCrystal += AddCrystal;
            BattleManager.Instance.GetSubSystem<EnemySystem>().onReturnCrystal += AddCrystal;
        }

        public void Deinitialize()
        {
            BattleManager.Instance.GetSubSystem<AgentReturnSystem>().onReturnCrystal -= AddCrystal;
        }

        private void AddCrystal(int value)
        {
            SetCrystal(_crystalVariable.Value + value);
        }

        public bool PayCrystal(int value)
        {
            int newCrystal = _crystalVariable.Value - value;
            if (newCrystal >= 0)
            {
                SetCrystal(newCrystal);
                return true;
            }

            return false;
        }

        public bool CheckCrystal(int value)
        {
            return _crystalVariable.Value >= value;
        }

        private void SetCrystal(int newCrystal)
        {
            _crystalVariable.Value = newCrystal;

            onChangedCrystal?.Invoke(newCrystal);
        }
    }
}