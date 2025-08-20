using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public abstract class GainShopItemData
    {
        [SerializeField] protected int _count;

        public int count => _count;

        internal abstract bool CanGainShopItem(int buyCount);
        internal abstract void GainShopItem(int buyCount);

#if UNITY_EDITOR
        public virtual void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            // ¼­ºêÅÇ ÀÌ¸§
            GUI.Label(labelRect, "È¹µæ ¼ö·®");
            _count = EditorGUI.IntField(valueRect, _count);
        }

        public virtual int GetHeight()
        {
            return 45;
        }
#endif
    }
}