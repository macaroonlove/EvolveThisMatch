using FrameWork.Loading;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Core
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

            _difficultyText.text = battleData.displayName;
            _finalWaveText.text = finalWave.ToString();

            _rewardItems[0].Show(battleData.goldVariable, battleData.goldPerWave * finalWave, () => 
            {
                _rewardItems[1].Show(battleData.essenceVariable, battleData.essencePerWave * finalWave, () =>
                {
                    _rewardItems[2].Show(battleData.lootVariable, battleData.lootPerWave * finalWave, null);
                });
            });

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