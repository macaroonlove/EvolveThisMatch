using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.Editor;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class ArtifactGainShopItemData : GainShopItemData
    {
        [SerializeField, Label("¾ÆÆ¼ÆÑÆ® Template")] private ArtifactTemplate _artifactTemplate;

        public ArtifactTemplate artifactTemplate => _artifactTemplate;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _artifactTemplate != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            SaveManager.Instance.profileData.AddArtifact(_artifactTemplate.id, count * buyCount);
        }
    }
}