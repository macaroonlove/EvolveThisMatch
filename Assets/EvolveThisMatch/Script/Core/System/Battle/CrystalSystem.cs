using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전투에서 사용하는 Crystal 값을 관리하는 클래스
    /// </summary>
    public class CrystalSystem : MonoBehaviour, IBattleSystem
    {
        [SerializeField] private ObscuredIntVariable _crystalVariable;

        internal event UnityAction<int> onChangedCrystal;

        public void Initialize()
        {
            SetCrystal(100);
        }

        public void Deinitialize()
        {
        }

        internal void AddCrystal(int value)
        {
            SetCrystal(_crystalVariable.Value + value);
        }

        internal bool PayCrystal(int value)
        {
            int newCrystal = _crystalVariable.Value - value;
            if (newCrystal >= 0)
            {
                SetCrystal(newCrystal);
                return true;
            }

            return false;
        }

        private void SetCrystal(int newCrystal)
        {
            _crystalVariable.Value = newCrystal;

            onChangedCrystal?.Invoke(newCrystal);
        }
    }
}