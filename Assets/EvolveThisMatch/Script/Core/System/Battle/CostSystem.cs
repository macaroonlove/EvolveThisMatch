using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전투에서 사용하는 Cost 값을 관리하는 클래스
    /// </summary>
    public class CostSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private ObscuredIntVariable _costVariable;

        private GlobalStatusSystem _globalStatusSystem;

        private bool _isInitializeCostSystem;
        private float _costRecoveryTimeProgress;

        #region 프로퍼티
        private bool finalIsCostRecovery
        {
            get
            {
                return _isInitializeCostSystem;
            }
        }

        private float finalCostRecoveryTime
        {
            get
            {
                float result = 1;

                #region 추가·차감
                foreach (var effect in _globalStatusSystem.CostRecoveryTimeAdditionalDataEffects)
                {
                    result -= effect.value;
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _globalStatusSystem.CostRecoveryTimeIncreaseDataEffects)
                {
                    increase += effect.value;
                }

                result /= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _globalStatusSystem.CostRecoveryTimeMultiplierDataEffects)
                {
                    result /= effect.value;
                }
                #endregion

                return result;
            }
        }
        #endregion

        internal event UnityAction<int> onChangedCost;

        public void Initialize()
        {
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            _isInitializeCostSystem = true;

            // TODO: 초기값이 있다면 수정해주기
            SetCost(100);
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;

            _isInitializeCostSystem = false;
        }

        private void Update()
        {
            if (finalIsCostRecovery == false) return;

            _costRecoveryTimeProgress += Time.deltaTime;
            if (_costRecoveryTimeProgress >= finalCostRecoveryTime)
            {
                _costRecoveryTimeProgress = 0;
                AddCost(1);
            }
        }

        internal void AddCost(int value)
        {
            SetCost(_costVariable.Value + value);
        }

        internal void PayCost(int value)
        {
            int newCost = _costVariable.Value - value;
            if (newCost >= 0)
            {
                SetCost(newCost);
            }
        }

        private void SetCost(int newCost)
        {
            _costVariable.Value = newCost;

            onChangedCost?.Invoke(newCost);
        }
    }
}