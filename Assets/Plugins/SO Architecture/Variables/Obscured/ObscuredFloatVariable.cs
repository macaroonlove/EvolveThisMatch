using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    public class ObscuredFloatEvent : UnityEvent<ObscuredFloat> { }

    [CreateAssetMenu(
        fileName = "ObscuredFloatVariable.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Obscured/ObscuredFloat",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 3)]
    public class ObscuredFloatVariable : BaseVariable<ObscuredFloat, ObscuredFloatEvent>
    {
        public override bool Clampable { get { return true; } }
        protected override ObscuredFloat ClampValue(ObscuredFloat value)
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
