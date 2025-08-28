namespace EvolveThisMatch.Lobby
{
    public class UIShopRewardListItem : UIShopRewardItem
    {
        protected override string GetItemInfoText(string displayName, int amount)
        {
            return $"{displayName} {base.GetItemInfoText(displayName, amount)}";
        }
    }
}