using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class ArtifactGainShopItemData : GainShopItemData
    {
        [SerializeField] private ArtifactTemplate _artifactTemplate;

        public ArtifactTemplate artifactTemplate => _artifactTemplate;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _artifactTemplate != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            SaveManager.Instance.itemData.AddArtifact(_artifactTemplate.id, count * buyCount);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "¾ÆÆ¼ÆÑÆ® Template");
            _artifactTemplate = (ArtifactTemplate)EditorGUI.ObjectField(valueRect, _artifactTemplate, typeof(ArtifactTemplate), false);

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