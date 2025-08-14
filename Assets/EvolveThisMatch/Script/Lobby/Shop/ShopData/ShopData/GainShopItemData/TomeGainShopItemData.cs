using EvolveThisMatch.Core;
using FrameWork.Editor;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class TomeGainShopItemData : GainShopItemData
    {
        [SerializeField, Label("°í¼­ Template")] private TomeTemplate _tomeTemplate;

        public TomeTemplate tomeTemplate => _tomeTemplate;
    }
}