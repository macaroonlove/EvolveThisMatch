using FrameWork.Editor;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class GainShopItemData
    {
        [SerializeField, Label("È¹µæ ¼ö·®")] protected int _count;

        public int count => _count;

        internal abstract bool CanGainShopItem(int buyCount);
        internal abstract void GainShopItem(int buyCount);
    }
}