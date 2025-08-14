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
    }
}