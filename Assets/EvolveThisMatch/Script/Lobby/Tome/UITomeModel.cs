using EvolveThisMatch.Core;
using EvolveThisMatch.Save;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITomeModel
    {
        private EffectContext _effectContext;

        public UITomeModel()
        {
            _effectContext = new EffectContext();
        }

        public TomeInfoViewState BuildInfoViewState(TomeTemplate template, ItemSaveData.Tome owned)
        {
            _effectContext.tomeSaveData = owned;

            string rangeText = "-";
            if (template.rangeType == ETomeRangeType.All)
            {
                rangeText = "ÀüÃ¼";
            }
            else if (template.rangeType == ETomeRangeType.Circle)
            {
                rangeText = $"¿ø ({template.range})";
            }

            return new TomeInfoViewState
            (
                template.displayName,
                template.description.Replace("{value}", $"{template.GetValue("value", _effectContext)}"),
                template.sprite,
                template.needCoin,
                template.cooldownTime,
                rangeText
            );
        }
    }
}