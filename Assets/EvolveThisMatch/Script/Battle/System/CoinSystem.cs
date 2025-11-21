using EvolveThisMatch.Core;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 전투에서 사용하는 Coin 값을 관리하는 클래스
    /// </summary>
    public class CoinSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private ObscuredIntVariable _coinVariable;

        public int currentCoin => _coinVariable.Value;

        public event UnityAction<int> onChangedCoin;

        public void Initialize()
        {   
            SetCoin(BattleManager.Instance.GetSubSystem<BattleWaveSystem>().waveCategory.startCoin);

            if (BattleContext.genesisCoin) AddCoin(100);

            BattleManager.Instance.GetSubSystem<EnemySystem>().onReturnCoin += AddCoin;
        }

        public void Deinitialize()
        {
        }

        private void AddCoin(int value)
        {
            SetCoin(_coinVariable.Value + value);
        }

        public bool PayCoin(int value)
        {
            int newCost = _coinVariable.Value - value;
            if (newCost >= 0)
            {
                SetCoin(newCost);
                return true;
            }
            return false;
        }

        public bool CheckCoin(int value)
        {
            return _coinVariable.Value >= value;
        }

        private void SetCoin(int newCoin)
        {
            _coinVariable.Value = newCoin;

            onChangedCoin?.Invoke(newCoin);
        }
    }
}