using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 골드를 관리하는 시스템
    /// </summary>
    public class CurrencySystem : MonoBehaviour, ICoreSystem
    {
        [Serializable]
        private class CurrencyData
        {
            [SerializeField] private CurrencyType _type;
            [SerializeField] private ObscuredIntVariable _variable;
            private event UnityAction<int> _onChange;

            public CurrencyType type => _type;
            public int value => _variable.Value;
            public Sprite icon => _variable.Icon;

            public void SetValue(int value)
            {
                _variable.SetValue(value);
                _onChange?.Invoke(value);
            }

            public void AddListener(UnityAction<int> listener)
            {
                _onChange += listener;
            }

            public void RemoveListener(UnityAction<int> listener)
            {
                _onChange -= listener;
            }
        }

        [SerializeField] private List<CurrencyData> _currencyList;

        private Dictionary<CurrencyType, CurrencyData> _currencies;
        private GlobalStatusSystem _globalStatusSystem;

        public void Initialize()
        {
            _currencies = _currencyList.ToDictionary(x => x.type);

            _globalStatusSystem = CoreManager.Instance.GetSubSystem<GlobalStatusSystem>();

            //SetCurrency(CurrencyType.Gold, 100);
            //SetCurrency(CurrencyType.Essence, 100);
        }

        public void Deinitialize()
        {
            _globalStatusSystem = null;

            _currencies.Clear();
        }

        #region 불러오기
        public int GetAmount(CurrencyType type)
        {
            if (_currencies.TryGetValue(type, out var data))
            {
                return data.value;
            }

            return 0;
        }

        public Sprite GetIcon(CurrencyType type)
        {
            if (_currencies.TryGetValue(type, out var data))
            {
                return data.icon;
            }

            return null;
        }
        #endregion

        public void AddCurrency(CurrencyType type, int amount)
        {
            int finalAmount = amount;

            if (type == CurrencyType.Gold)
            {
                finalAmount = CalculateGoldAmount(amount);
            }

            var newValue = GetAmount(type) + finalAmount;
            SetCurrency(type, newValue);
        }

        public bool CheckCurrency(CurrencyType type, int amount)
        {
            return GetAmount(type) >= amount;
        }

        public bool PayCurrency(CurrencyType type, int amount)
        {
            var newValue = GetAmount(type) - amount;

            if (newValue >= 0)
            {
                SetCurrency(type, newValue);
                return true;
            }

            return false;
        }

        private void SetCurrency(CurrencyType type, int value)
        {
            if (_currencies.TryGetValue(type, out var data))
            {
                data.SetValue(value);
            }
        }

        #region 이벤트
        public void AddListener(CurrencyType type, UnityAction<int> listener)
        {
            if (_currencies.TryGetValue(type, out var data))
            {
                data.AddListener(listener);
            }
        }

        public void RemoveListener(CurrencyType type, UnityAction<int> listener)
        {
            if (_currencies.TryGetValue(type, out var data))
            {
                data.RemoveListener(listener);
            }
        }
        #endregion

        #region 추가 계산
        private int CalculateGoldAmount(int amount)
        {
            float result = amount;

            // 추가·차감
            foreach (var effect in _globalStatusSystem.GoldGainAdditionalDataEffects)
            {
                result += effect.value;
            }

            // 증가·감소
            float increase = 1;
            foreach (var effect in _globalStatusSystem.GoldGainIncreaseDataEffects)
            {
                increase += effect.value;
            }
            result *= increase;

            // 상승·하락
            foreach (var effect in _globalStatusSystem.GoldGainMultiplierDataEffects)
            {
                result *= effect.value;
            }

            return (int)result;
        }
        #endregion
    }
}