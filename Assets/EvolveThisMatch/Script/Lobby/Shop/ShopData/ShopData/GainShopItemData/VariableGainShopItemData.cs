using ScriptableObjectArchitecture;
using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class VariableGainShopItemData : GainShopItemData
    {
        [SerializeField] private ObscuredIntVariable _variable;

        public ObscuredIntVariable variable => _variable;

        internal override bool CanGainShopItem(int buyCount)
        {
            return _variable != null && buyCount > 0;
        }

        internal override void GainShopItem(int buyCount)
        {
            _variable.AddValue(count * buyCount);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "æ∆¿Ã≈€ SO");
            _variable = (ObscuredIntVariable)EditorGUI.ObjectField(valueRect, _variable, typeof(ObscuredIntVariable), false);

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