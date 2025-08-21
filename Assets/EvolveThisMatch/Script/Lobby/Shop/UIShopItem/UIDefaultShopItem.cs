using EvolveThisMatch.Save;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Lobby
{
    public class UIDefaultShopItem : UIShopItem, IPointerClickHandler
    {
        private UnityAction<ShopSaveData.ShopItem> _onSelect;

        internal void Show(ShopSaveData.ShopCatalog shopCatalog, ShopItemData itemData, UnityAction<ShopSaveData.ShopItem> onSelect)
        {
            _onSelect = onSelect;

            base.Show(shopCatalog, itemData);

            gameObject.SetActive(true);
            transform.parent.gameObject.SetActive(true);
        }

        public override void Hide(bool isForce = false)
        {
            gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isBuyAble) return;

            _onSelect?.Invoke(_shopItem);
        }
    }
}