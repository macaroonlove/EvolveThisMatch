using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class VariableGainShopItemData : GainShopItemData
    {
        [SerializeField, Label("¾ÆÀÌÅÛ SO")] private ObscuredIntVariable _variable;

        public ObscuredIntVariable variable => _variable;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _variable != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            _variable.AddValue(count * buyCount);
        }
    }
}