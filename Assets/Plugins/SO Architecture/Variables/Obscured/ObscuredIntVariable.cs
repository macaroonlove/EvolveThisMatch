using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    public class ObscuredIntEvent : UnityEvent<ObscuredInt> { }

    [CreateAssetMenu(
        fileName = "ObscuredIntVariable.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Obscured/Obscuredint",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 4)]
    public class ObscuredIntVariable : BaseVariable<ObscuredInt, ObscuredIntEvent>
    {
        [SerializeField]
        private string _displayName;
        [SerializeField]
        private Sprite _icon;

        public override bool Clampable { get { return true; } }
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;

#if UNITY_EDITOR
        public void AddValueDebug(int add)
        {
            AddValue(add);
        }
#endif

        public ObscuredInt AddValue(int add)
        {
            Value = Value + add;
            return Value;
        }

        public ObscuredInt SetValue(int value)
        {
            Value = value;
            return Value;
        }

        protected override ObscuredInt ClampValue(ObscuredInt value)
        {
            if (value.CompareTo(MinClampValue) < 0)
            {
                return MinClampValue;
            }
            else if (value.CompareTo(MaxClampValue) > 0)
            {
                return MaxClampValue;
            }
            else
            {
                return value;
            }
        }
    }
}