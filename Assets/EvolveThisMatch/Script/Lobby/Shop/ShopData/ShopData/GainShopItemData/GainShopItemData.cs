using FrameWork.Editor;
using System;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class GainShopItemData
    {
        [SerializeField, Label("È¹µæ ¼ö·®")] protected int _count;

        public int count => _count;
    }
}