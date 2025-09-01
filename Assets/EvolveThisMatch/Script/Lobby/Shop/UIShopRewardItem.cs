using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopRewardItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            ItemBG,
            ItemIcon,
        }
        enum Texts
        {
            ItemInfo,
        }
        #endregion

        [SerializeField] private Sprite _artifactBackground;
        [SerializeField] private Sprite _tomeBackground;

        private Image _itemBG;
        private Image _itemIcon;
        protected TextMeshProUGUI _itemInfo;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _itemBG = GetImage((int)Images.ItemBG);
            _itemIcon = GetImage((int)Images.ItemIcon);
            _itemInfo = GetText((int)Texts.ItemInfo);
        }

        internal void Show(Reward reward)
        {
            switch (reward.type)
            {
                case "Profile":
                    ShowVariableItemData(reward);
                    break;
                case "Agent":
                    ShowUnitItemData(reward);
                    break;
            };

            base.Show(true);
        }

        private async void ShowVariableItemData(Reward reward)
        {
            var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(reward.key);

            if (variable != null)
            {
                _itemBG.sprite = variable.IconBG;
                _itemIcon.sprite = variable.Icon;
                _itemInfo.text = GetItemInfoText(variable.DisplayName, reward.amount);
            }
        }

        private async void ShowUnitItemData(Reward reward)
        {
            var agent = await AddressableAssetManager.Instance.GetScriptableObject<AgentTemplate>(reward.key);

            if (agent != null)
            {
                _itemBG.sprite = agent.rarity.agentInfoSprite;
                _itemIcon.sprite = agent.sprite;
                _itemInfo.text = GetItemInfoText(agent.displayName, reward.amount);
            }
        }
        
        private async void ShowArtifactItemData(Reward reward)
        {
            var artifact = await AddressableAssetManager.Instance.GetScriptableObject<ArtifactTemplate>(reward.key);

            if (artifact != null)
            {
                _itemBG.sprite = _artifactBackground;
                _itemIcon.sprite = artifact.sprite;
                _itemInfo.text = GetItemInfoText(artifact.displayName, reward.amount);
            }
        }

        private async void ShowTomeItemData(Reward reward)
        {
            var tome = await AddressableAssetManager.Instance.GetScriptableObject<TomeTemplate>(reward.key);

            if (tome != null)
            {
                _itemBG.sprite = _tomeBackground;
                _itemIcon.sprite = tome.sprite;
                _itemInfo.text = GetItemInfoText(tome.displayName, reward.amount);
            }
        }

        protected virtual string GetItemInfoText(string displayName, int amount)
        {
            return $"x{amount}";
        }
    }
}