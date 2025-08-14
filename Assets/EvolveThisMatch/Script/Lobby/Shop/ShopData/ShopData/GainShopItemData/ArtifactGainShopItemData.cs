using EvolveThisMatch.Core;
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
    }
}