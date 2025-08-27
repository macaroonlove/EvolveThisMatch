using EvolveThisMatch.Core;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UI
{
    public class VariableDisplayManager : Singleton<VariableDisplayManager>
    {
        private CurrencySystem _currencySystem;
        private UIIntVariableTextEffect[] _intVariables;

        protected override void Initialize()
        {   
            _intVariables = GetComponentsInChildren<UIIntVariableTextEffect>();
        }

        public void Show(CurrencyType currencyType)
        {
            if (_currencySystem == null)
            {
                _currencySystem = CoreManager.instance.GetSubSystem<CurrencySystem>();
            }
            
            var intVariable = _currencySystem.GetIntVariable(currencyType);

            Show(intVariable);
        }

        public void Show(ObscuredIntVariable intVariable)
        {
            for (int i = _intVariables.Length - 1; i >= 0; i--)
            {
                if (_intVariables[i].isEmpty)
                {
                    _intVariables[i]?.SetVariable(intVariable);
                    break;
                }
            }
        }

        public void HideAll()
        {
            foreach (var variable in _intVariables)
            {
                variable.Hide();
            }
        }

        #region 골드 애니메이션
        internal UnityAction<Vector3, int> onGoldAnimation;

        public void PlayGoldAnimation(Vector3 startPositon, int itemCount = 5)
        {
            onGoldAnimation?.Invoke(startPositon, itemCount);
        }
        #endregion
    }
}