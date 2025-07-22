using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 재능의 정수를 관리하는 시스템
    /// </summary>
    public class EssenceSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField] private ObscuredIntVariable _essenceVariable;

        internal int currentEssence => _essenceVariable.Value;

        internal event UnityAction<int> onChangeEssence;

        public void Initialize()
        {
            // TODO: Template에서 시작 정수 받아오도록 수정
            SetEssence(100);
        }

        public void Deinitialize()
        {
        }

        public void AddEssence(int value)
        {
            SetEssence(currentEssence + value);
        }

        public bool PayEssence(int value)
        {
            int newValue = currentEssence - value;
            if (newValue >= 0)
            {
                SetEssence(newValue);
                return true;
            }

            return false;
        }

        public bool CheckEssence(int value)
        {
            return currentEssence >= value;
        }

        private void SetEssence(int essence)
        {
            _essenceVariable.SetValue(essence);

            onChangeEssence?.Invoke(currentEssence);
        }
    }
}