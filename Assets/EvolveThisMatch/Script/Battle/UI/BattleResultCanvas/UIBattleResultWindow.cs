using EvolveThisMatch.Core;
using FrameWork.Loading;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UIBattleResultWindow : UIBase
    {
        #region 바인딩
        enum Texts
        {
            DifficultyText,
            FinalWaveText,
        }
        enum Buttons
        {
            AD,
            GoLobby,
        }
        enum Objects
        {
            RewardGroup,
        }
        #endregion

        private TextMeshProUGUI _difficultyText;
        private TextMeshProUGUI _finalWaveText;
        private Transform _rewardGroup;
        private UIRewardItem[] _rewardItems;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _difficultyText = GetText((int)Texts.DifficultyText);
            _finalWaveText = GetText((int)Texts.FinalWaveText);
            _rewardGroup = GetObject((int)Objects.RewardGroup).transform;

            InitializeRewardItem();

            GetButton((int)Buttons.AD).onClick.AddListener(AD);
            GetButton((int)Buttons.GoLobby).onClick.AddListener(GoLobby);
        }

        private void InitializeRewardItem()
        {
            _rewardItems = new UIRewardItem[3];
            var rewardItem = GetComponentInChildren<UIRewardItem>();
            _rewardItems[0] = rewardItem;

            for (int i = 1; i < 3; i++)
            {
                var item = Instantiate(rewardItem.gameObject, _rewardGroup).GetComponent<UIRewardItem>();
                _rewardItems[i] = item;
            }
        }

        internal void Show()
        {
            var battleData = GameDataManager.Instance.battleData;
            var finalWave = BattleManager.Instance.GetSubSystem<WaveSystem>().currentWaveIndex;
            var currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();

            _difficultyText.text = battleData.displayName;
            _finalWaveText.text = finalWave.ToString();

            foreach (var reward in battleData.rewardsPerWave)
            {
                int amount = reward.amount * finalWave;
                currencySystem.AddCurrency(reward.type, amount);
            }

            void ShowRewardItems(IReadOnlyList<RewardData> rewards, int index)
            {
                if (index >= rewards.Count) return;

                var reward = rewards[index];
                int totalAmount = reward.amount * finalWave;

                _rewardItems[index].Show(currencySystem.GetIcon(reward.type), totalAmount, () =>
                {
                    ShowRewardItems(rewards, index + 1);
                });
            }

            ShowRewardItems(battleData.rewardsPerWave, 0);

            base.Show();
        }

        private void AD()
        {
            // 광고 보여주기

            // 로비로 보내기
            GoLobby();
        }

        private void GoLobby()
        {
            LoadingManager.Instance.LoadScene("Lobby");
        }
    }
}