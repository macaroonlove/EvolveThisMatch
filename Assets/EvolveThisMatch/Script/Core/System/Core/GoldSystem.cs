using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 골드를 관리하는 시스템
    /// </summary>
    public class GoldSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField] private ObscuredIntVariable _goldVariable;

        private GlobalStatusSystem _globalStatusSystem;

        internal int currentGold => _goldVariable.Value;

        internal event UnityAction<int> onChangeGold;

        public void Initialize()
        {
            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            // TODO: Template에서 시작 골드 받아오도록 수정
            SetGold(100);
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;
        }

        public void AddGold(int gold)
        {
            #region 골드 획득량 계산
            float result = gold;

            #region 추가·차감
            foreach (var effect in _globalStatusSystem.GoldGainAdditionalDataEffects)
            {
                result += effect.value;
            }
            #endregion

            #region 증가·감소
            float increase = 1;

            foreach (var effect in _globalStatusSystem.GoldGainIncreaseDataEffects)
            {
                increase += effect.value;
            }

            result *= increase;
            #endregion

            #region 상승·하락
            foreach (var effect in _globalStatusSystem.GoldGainMultiplierDataEffects)
            {
                result *= effect.value;
            }
            #endregion

            gold = (int)result;
            #endregion

            SetGold(currentGold + gold);
        }

        public bool PayGold(int value)
        {
            int newValue = currentGold - value;
            if (newValue >= 0)
            {
                SetGold(newValue);
                return true;
            }

            return false;
        }

        public bool CheckGold(int value)
        {
            return currentGold >= value;
        }

        private void SetGold(int gold)
        {
            _goldVariable.SetValue(gold);

            onChangeGold?.Invoke(_goldVariable.Value);
        }
    }
}