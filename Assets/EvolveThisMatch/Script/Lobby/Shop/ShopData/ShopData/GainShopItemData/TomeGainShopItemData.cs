using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
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

        internal override bool CanGainShopItem(int buyCount)
        {
            return _tomeTemplate != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            SaveManager.Instance.profileData.AddTome(_tomeTemplate.id, count * buyCount);
        }
    }
}