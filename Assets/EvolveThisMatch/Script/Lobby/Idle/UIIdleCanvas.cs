using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.Service;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIIdleCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            GainReward,
            AdGainReward,
        }
        enum Texts
        {
            MaxStageText,
            TimerText,
            GoldCount,
            LootCount,
        }
        #endregion

        private TextMeshProUGUI _maxStageText;
        private TextMeshProUGUI _timerText;
        private TextMeshProUGUI _goldCount;
        private TextMeshProUGUI _lootCount;

        private LobbyWaveSystem _waveSystem;
        private int _offlineGold;
        private int _offlineLoot;
        private UnityAction<bool> _onGainReward;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _maxStageText = GetText((int)Texts.MaxStageText);
            _timerText = GetText((int)Texts.TimerText);
            _goldCount = GetText((int)Texts.GoldCount);
            _lootCount = GetText((int)Texts.LootCount);

            _waveSystem = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>();

            GetButton((int)Buttons.GainReward).onClick.AddListener(GainReward);
            GetButton((int)Buttons.AdGainReward).onClick.AddListener(AdGainReward);
        }

        internal void Show(int minute, int offlineGold, int offlineLoot, UnityAction<bool> onGainReward)
        {
            var maxCategory = SaveManager.Instance.profileData.maxCategory;
            var category = _waveSystem.waveLibrary.categorys[int.Parse(maxCategory.Key)];
            int maxChapterIndex = (maxCategory.Value.MaxStage - 1) / 10;
            int maxStageIndex = (maxCategory.Value.MaxStage - 1) % 10;
            var waveTemplate = category.chapters[maxChapterIndex].waves[maxStageIndex];

            _maxStageText.text = $"<color=#FBE698>{category.title} {waveTemplate.stage}</color> {waveTemplate.displayName}";

            _offlineGold = offlineGold;
            _offlineLoot = offlineLoot;
            _onGainReward = onGainReward;

            _timerText.text = $"<sprite name=timer> {minute}분";
            _goldCount.text = $"{offlineGold.Format(4)}";
            _lootCount.text = $"{offlineLoot.Format(4)}";

            base.Show(true);
        }

        private void AdGainReward()
        {
#if !UNITY_EDITOR
            AdmobManager.Instance.ShowRewardAd((isSuccess) =>
            {
                _onGainReward?.Invoke(isSuccess);
                Hide(true);
            });
#else
            GainReward();
#endif
        }

        private void GainReward()
        {
            _onGainReward?.Invoke(false);
            Hide(true);
        }
    }
}