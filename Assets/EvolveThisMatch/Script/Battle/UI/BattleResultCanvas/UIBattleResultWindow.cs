using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.Loading;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EvolveThisMatch.Battle
{
    public class UIBattleResultWindow : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            CategoryText,
            ChapterText,
            FinalPageText,
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

        [SerializeField] private GameObject _rewardItemPrefab;

        private TextMeshProUGUI _categoryText;
        private TextMeshProUGUI _chapterText;
        private TextMeshProUGUI _finalPageText;
        private Transform _rewardGroup;

        private BattleResultData _data;

        protected override void Initialize()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _categoryText = GetText((int)Texts.CategoryText);
            _chapterText = GetText((int)Texts.ChapterText);
            _finalPageText = GetText((int)Texts.FinalPageText);
            _rewardGroup = GetObject((int)Objects.RewardGroup).transform;

            GetButton((int)Buttons.AD).onClick.AddListener(AD);
            GetButton((int)Buttons.GoLobby).onClick.AddListener(GoLobby);
        }

        internal async void Show(BattleResultData data)
        {
            base.Show();
            _data = data;

            _categoryText.text = data.category;
            _chapterText.text = data.chapter;
            _finalPageText.text = data.finalPage;

            await ShowReward(0);

            async UniTask ShowReward(int index)
            {
                if (index >= data.rewardData.Count)
                    return;

                var rewardData = data.rewardData[index];

                var item = Instantiate(_rewardItemPrefab, _rewardGroup).GetComponent<UIRewardItem>();
                var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(rewardData.Item1);

                item.Show(variable, rewardData.Item2, async () =>
                {
                    await ShowReward(index + 1);
                });
            }
        }

        private void AD()
        {
#if !UNITY_EDITOR
            AdmobManager.Instance.ShowRewardAd((isSuccess) =>
            {
                _data?.onAgainReward(isSuccess);
                GoLobby();
            });
#else
            GoLobby();
#endif
        }

        private void GoLobby()
        {
            LoadingManager.Instance.LoadScene("Lobby");
        }
    }
}