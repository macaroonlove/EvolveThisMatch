using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class RandomShopData : ShopData
    {
        [SerializeField] private int _slotCount;

        public override List<ShopItemData> GetItems()
        {
            int count = Mathf.Min(_slotCount, _shopItems.Count);

            List<ShopItemData> tempList = new List<ShopItemData>(_shopItems);
            List<ShopItemData> result = new List<ShopItemData>();

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, tempList.Count);
                result.Add(tempList[index]);
                tempList.RemoveAt(index);
            }

            return result;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            rect.y += base.GetHeight() - 30;

            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            // ¼­ºêÅÇ ÀÌ¸§
            GUI.Label(labelRect, "½½·Ô °³¼ö");
            _slotCount = EditorGUI.IntField(valueRect, _slotCount);
        }

        public override float GetHeight()
        {
            return base.GetHeight() + 20;
        }
#endif
    }
}