using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIStageEnemyItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            BG,
            Icon,
        }
        enum Texts
        {
            Info,
            Reward,
        }
        #endregion

        private Image _bg;
        private Image _icon;
        private TextMeshProUGUI _info;
        private TextMeshProUGUI _reward;

        private readonly Dictionary<EVariableType, string> currencyToIcon = new()
        {
            { EVariableType.Gold, "gold" },
            { EVariableType.Loot, "loot" },
        };

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));

            _bg = GetImage((int)Images.BG);
            _icon = GetImage((int)Images.Icon);
            _info = GetText((int)Texts.Info);
            _reward = GetText((int)Texts.Reward);
        }

        public void Show(EnemyData enemyData)
        {
            _bg.sprite = enemyData.template.rarity.sprite;
            _icon.sprite = enemyData.template.sprite;
            _info.text = $"<sprite name=ATK> {enemyData.atk}\n<sprite name=HP> {enemyData.hp}";

            _reward.text = "";
            foreach (var dropData in enemyData.idleDropDatas)
            {
                if (currencyToIcon.TryGetValue(dropData.type, out var icon))
                {
                    _reward.text += $"<sprite name={icon}> {dropData.amount}\n";
                }
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
