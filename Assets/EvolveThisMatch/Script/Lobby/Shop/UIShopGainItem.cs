using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIShopGainItem : UIBase
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

        internal void Show(GainShopItemData gainItemData)
        {
            _itemInfo.text = gainItemData switch
            {
                VariableGainShopItemData variableData => ShowVariableItemData(variableData),
                UnitGainShopItemData unitData => ShowUnitItemData(unitData),
                ArtifactGainShopItemData artifactData => ShowArtifactItemData(artifactData),
                TomeGainShopItemData tomeData => ShowTomeItemData(tomeData),
                _ => ""
            };

            base.Show(true);
        }

        protected virtual string ShowVariableItemData(VariableGainShopItemData variableData)
        {
            _itemBG.sprite = variableData.variable.IconBG;
            _itemIcon.sprite = variableData.variable.Icon;

            return $"x{variableData.count}";
        }

        protected virtual string ShowUnitItemData(UnitGainShopItemData unitData)
        {
            _itemBG.sprite = unitData.agentTemplate.rarity.agentInfoSprite;
            _itemIcon.sprite = unitData.agentTemplate.sprite;

            return $"x{unitData.count}";
        }

        protected virtual string ShowArtifactItemData(ArtifactGainShopItemData artifactData)
        {
            _itemBG.sprite = _artifactBackground;
            _itemIcon.sprite = artifactData.artifactTemplate.sprite;

            return $"x{artifactData.count}";
        }

        protected virtual string ShowTomeItemData(TomeGainShopItemData tomeData)
        {
            _itemBG.sprite = _tomeBackground;
            _itemIcon.sprite = tomeData.tomeTemplate.sprite;

            return $"x{tomeData.count}";
        }
    }
}