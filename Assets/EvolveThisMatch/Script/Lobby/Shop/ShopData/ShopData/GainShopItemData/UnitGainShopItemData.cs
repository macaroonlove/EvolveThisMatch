using EvolveThisMatch.Core;
using FrameWork.Editor;
using ScriptableObjectArchitecture;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class UnitGainShopItemData : GainShopItemData
    {
        [SerializeField, Label("РЏДж Template")] private AgentTemplate _agentTemplate;

        public AgentTemplate agentTemplate => _agentTemplate;
    }
}