using EvolveThisMatch.Save;
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

            GetButton((int)Buttons.GainReward).onClick.AddListener(GainReward);
            GetButton((int)Buttons.AdGainReward).onClick.AddListener(AdGainReward);
        }

        internal void Show(int minute, int offlineGold, int offlineLoot, UnityAction<bool> onGainReward)
        {
            // TODO: 최고 스테이지로 변경
            //_maxStageText.text = SaveManager.Instance.profileData.maxStage;

            _offlineGold = offlineGold;
            _offlineLoot = offlineLoot;
            _onGainReward = onGainReward;

            _timerText.text = $"<sprite name=timer> {minute}분";
            _goldCount.text = $"{offlineGold}";
            _lootCount.text = $"{offlineLoot}";

            base.Show(true);
        }

        private void AdGainReward()
        {
#if !UNITY_EDITOR
            AdmobManager.Instance.ShowRewardAd((isSuccess) =>
            {
                if (isSuccess)
                {
                    _onGainReward?.Invoke(true);
                }
                else
                {
                    _onGainReward?.Invoke(false);
                }
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