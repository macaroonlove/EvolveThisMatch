using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.Editor;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class UnitGainShopItemData : GainShopItemData
    {
        [SerializeField, Label("РЏДж Template")] private AgentTemplate _agentTemplate;

        public AgentTemplate agentTemplate => _agentTemplate;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _agentTemplate != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            SaveManager.Instance.profileData.AddAgent(_agentTemplate.id, count * buyCount);
        }
    }
}