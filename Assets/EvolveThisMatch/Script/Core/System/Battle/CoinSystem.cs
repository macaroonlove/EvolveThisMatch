using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전투에서 사용하는 Coin 값을 관리하는 클래스
    /// </summary>
    public class CoinSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private ObscuredIntVariable _coinVariable;

        internal int currentCoin => _coinVariable.Value;

        internal event UnityAction<int> onChangedCoin;

        public void Initialize()
        {
            SetCoin(10000);
        }

        public void Deinitialize()
        {
        }

        internal void AddCoin(int value)
        {
            SetCoin(_coinVariable.Value + value);
        }

        internal bool PayCoin(int value)
        {
            int newCost = _coinVariable.Value - value;
            if (newCost >= 0)
            {
                SetCoin(newCost);
                return true;
            }
            return false;
        }

        private void SetCoin(int newCoin)
        {
            _coinVariable.Value = newCoin;

            onChangedCoin?.Invoke(newCoin);
        }
    }
}