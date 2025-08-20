using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class TomeGainShopItemData : GainShopItemData
    {
        [SerializeField] private TomeTemplate _tomeTemplate;

        public TomeTemplate tomeTemplate => _tomeTemplate;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _tomeTemplate != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            SaveManager.Instance.profileData.AddTome(_tomeTemplate.id, count * buyCount);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "°í¼­ Template");
            _tomeTemplate = (TomeTemplate)EditorGUI.ObjectField(valueRect, _tomeTemplate, typeof(TomeTemplate), false);

            rect.y = labelRect.y + 20;

            base.Draw(rect);
        }

        public override int GetHeight()
        {
            int height = base.GetHeight();

            height += 20;

            return height;
        }
#endif
    }
}