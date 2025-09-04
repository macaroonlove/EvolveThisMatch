using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Service
{
    // TODO: 퍼블리싱 할 때 사용 현재는 테스트 ID로 구현
    // private const string _adUnitId = "ca-app-pub-4529303677886329/5125850813";

    public class AdmobManager : PersistentSingleton<AdmobManager>
    {
        private RewardedAd _rewardedAd;

        private UnityAction _onRewardComplete;

        private const string _adUnitId = "ca-app-pub-3940256099942544/5224354917";

        protected override void Initialize()
        {
            MobileAds.Initialize((initStatus) =>
            {
                LoadRewardAd();
            });
        }

        /// <summary>
        /// 보상형 광고 로드
        /// </summary>
        private void LoadRewardAd()
        {
            var adRequest = new AdRequest();

            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"RewardAd 로드 실패 {error.GetMessage()}");
#endif
                    _rewardedAd = null;
                    return;
                }

                _rewardedAd = ad;

                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    _onRewardComplete?.Invoke();
                    _onRewardComplete = null;

                    // 광고 닫히면 새 광고 로드
                    LoadRewardAd();
                };
            });
        }

        /// <summary>
        /// 보상형 광고 보여주기
        /// </summary>
        public void ShowRewardAd(UnityAction onComplete)
        {
            if (_rewardedAd == null || !_rewardedAd.CanShowAd())
            {
#if UNITY_EDITOR
                Debug.LogError("RewardAd가 준비되지 않았음");
#endif
                return;
            }

            _onRewardComplete = null;

            _rewardedAd.Show((Reward reward) =>
            {
                _onRewardComplete = onComplete;
            });
        }
    }
}