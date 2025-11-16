using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Service
{
    // TODO: 퍼블리싱 할 때 사용 현재는 테스트 ID로 구현
    // private const string _adUnitId = "ca-app-pub-4529303677886329/5125850813";

#if !UNITY_EDITOR
    public class AdmobManager : PersistentSingleton<AdmobManager>
    {
        private RewardedAd _rewardedAd;

        private UnityAction<bool> _onRewardComplete;

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

                _rewardedAd.OnAdFullScreenContentFailed += (error) =>
                {
#if UNITY_EDITOR
                    Debug.LogError($"광고 표시 실패: {error.GetMessage()}");
#endif

                    _onRewardComplete?.Invoke(false);
                    _onRewardComplete = null;

                    // 광고 실패 시, 새 광고 로드
                    LoadRewardAd();
                };

                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    _onRewardComplete?.Invoke(true);
                    _onRewardComplete = null;

                    // 광고 닫히면 새 광고 로드
                    LoadRewardAd();
                };
            });
        }

        /// <summary>
        /// 보상형 광고 보여주기
        /// </summary>
        public void ShowRewardAd(UnityAction<bool> onComplete)
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
#endif
}