using System;
using UnityEditor;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public enum ItemType
    {
        Gold,
        ActiveItem,
        PassiveItem,
    }

    [Serializable]
    public class GetEpisode : Episode
    {
        public override CommandType command => CommandType.Get;
        public ItemType itemType;
        public int itemAmount;
        public string itemName;

        public void Initialize(string theme, string context)
        {
            itemType = (ItemType)Enum.Parse(typeof(ItemType), theme);
            itemAmount = int.Parse(context);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            var elementRect = new Rect(rect.x + 130, rect.y + 4, 190, 18);

            itemType = (ItemType)EditorGUI.EnumPopup(elementRect, itemType);

            elementRect.x += 200;
            elementRect.width = rect.width - 330;

            switch (itemType) {
                case ItemType.Gold:
                    itemAmount = EditorGUI.IntField(elementRect, itemAmount);
                    break;
                case ItemType.ActiveItem:
                case ItemType.PassiveItem:
                    itemName = EditorGUI.TextField(elementRect, itemName);
                    break;
            }
        }

        public override int GetHeight()
        {
            return 1;
        }
#endif
    }
}