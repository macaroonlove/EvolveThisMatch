using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.Loading;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using ScriptableObjectArchitecture;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EvolveThisMatch.Lobby
{
    public class UIBattleStartCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            BattleStartButton,
            MinusButton,
            PlusButton,
        }
        enum Toggles
        {
            GenesisCoin,
            OriginCrystal,
            FateRoneStone,
            HeroSeal,
        }
        enum Dropdowns
        {
            CategoryDropdown,
        }
        enum Texts
        {
            Description,
            RewardText,
            BattleStartText,
        }
        #endregion


        private TMP_Dropdown _categoryDropdown;
        private UIChapterItem[] _chapterItems;
        private TextMeshProUGUI _description;
        private TextMeshProUGUI _rewardText;
        private TextMeshProUGUI _battleStartText;

        private WaveLibraryTemplate _waveLibraryTemplates;
        private int _currentCategory;
        private int _currentChapter;
        private int _selectedUsabilityItem;
        private int _multiplyReward;
        private bool _isBattleAble;

        private Dictionary<int, ObscuredIntVariable> _variables = new Dictionary<int, ObscuredIntVariable>();

        protected override void Initialize()
        {
            _currentCategory = 0;
            _currentChapter = 0;
            _selectedUsabilityItem = 0;
            _multiplyReward = 0;

            _chapterItems = GetComponentsInChildren<UIChapterItem>();

            AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("GenesisCoin", (variable) => { _variables[0] = variable; });
            AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("OriginCrystal", (variable) => { _variables[1] = variable; });
            AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("FateRoneStone", (variable) => { _variables[2] = variable; });
            AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("HeroSeal", (variable) => { _variables[3] = variable; });

            AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("Action", (variable) =>
            {
                _variables[4] = variable;

                MultipleReward(1);
            });

            BindButton(typeof(Buttons));
            BindToggle(typeof(Toggles));
            BindDropdown(typeof(Dropdowns));
            BindText(typeof(Texts));

            _categoryDropdown = GetDropdown((int)Dropdowns.CategoryDropdown);
            _description = GetText((int)Texts.Description);
            _rewardText = GetText((int)Texts.RewardText);
            _battleStartText = GetText((int)Texts.BattleStartText);
        }

        private void Start()
        {
            _waveLibraryTemplates = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>().waveLibrary;

            InitializeCategory();

            for (int i = 0; i < _chapterItems.Length; i++)
            {
                _chapterItems[i].Initialize(i, ChangeChapter);
            }

            GetToggle((int)Toggles.GenesisCoin).onValueChanged.AddListener((isOn) => ChangeItemToggle(isOn, 0));
            GetToggle((int)Toggles.OriginCrystal).onValueChanged.AddListener((isOn) => ChangeItemToggle(isOn, 1));
            GetToggle((int)Toggles.FateRoneStone).onValueChanged.AddListener((isOn) => ChangeItemToggle(isOn, 2));
            GetToggle((int)Toggles.HeroSeal).onValueChanged.AddListener((isOn) => ChangeItemToggle(isOn, 3));

            GetButton((int)Buttons.PlusButton).onClick.AddListener(() => MultipleReward(1));
            GetButton((int)Buttons.MinusButton).onClick.AddListener(() => MultipleReward(-1));
            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
            GetButton((int)Buttons.BattleStartButton).onClick.AddListener(BattleStartButton);
        }

        internal void Show()
        {
            _categoryDropdown.value = _currentCategory;

            ShowCategory();

            base.Show(true);
        }

        #region 카테고리
        private void InitializeCategory()
        {
            // 드롭다운 초기화
            _categoryDropdown.ClearOptions();
            _categoryDropdown.onValueChanged.RemoveAllListeners();

            // 이벤트 새롭게 추가
            _categoryDropdown.onValueChanged.AddListener(ChangeCategory);

            // 드롭다운 새롭게 추가
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var category in _waveLibraryTemplates.categorys)
            {
                options.Add(new TMP_Dropdown.OptionData(category.title));
            }

            _categoryDropdown.AddOptions(options);
        }

        private void ShowCategory()
        {
            for (int i = 0; i < _chapterItems.Length; i++)
            {
                _chapterItems[i].Show(_waveLibraryTemplates.categorys[_currentCategory].chapters[i]);
            }
        }

        private void ChangeCategory(int category)
        {
            _currentCategory = category;

            ShowCategory();
            ChangeChapter(0);
        }
        #endregion

        #region 챕터
        private void ChangeChapter(int chapter)
        {
            _currentChapter = chapter;

            if (chapter == 0)
            {
                _description.text = "몰려드는 적들을 처치하세요.";
            }
            else
            {
                _description.text = $"적 몬스터의 전투력 및 이동속도 {chapter * 10}% 상승";
            }
        }
        #endregion

        private void ChangeItemToggle(bool isOn, int index)
        {
            if (_variables[index].Value <= 0) return;

            int bit = 1 << index;

            if (isOn) _selectedUsabilityItem |= bit;
            else _selectedUsabilityItem &= ~bit;
        }

        private void MultipleReward(int delta)
        {
            int limit = Mathf.Min(_variables[4].Value / 5, 3);
            if (limit < 1) limit = 1;

            _multiplyReward = Mathf.Clamp(_multiplyReward + delta, 1, limit);
            _isBattleAble = _variables[4].Value >= _multiplyReward * 5;

            _rewardText.text = $"보상 <size=18>×</size>{_multiplyReward}";
            if (_isBattleAble)
            {
                _battleStartText.text = $"개입 시작\n<sprite name=Action> {_multiplyReward * 5}";
            }
            else
            {
                _battleStartText.text = $"<color=#FF4B4B>개입 시작\n<sprite name=Action> {_multiplyReward * 5}</color>";
            }
        }

        private void BattleStartButton()
        {
            if (_isBattleAble == false)
            {
                UIPopupManager.Instance.ShowConfirmPopup("봉인된 서약서의 개수가 부족하여 전투를 시작할 수 없습니다.");
                return;
            }

            BattleContext.Clear();

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "BattleStart",
                FunctionParameter = new { CurrentCategory = _currentCategory, CurrentChapter = _currentChapter, SelectedUsabilityItem = _selectedUsabilityItem, MultiplyReward = _multiplyReward },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (ExecuteCloudScriptResult result) =>
                {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;

                    if ((bool)jsonResult["success"])
                    {
                        int possibleItem = Convert.ToInt32(jsonResult["possibleItem"]);

                        // 전투 상황에 기록
                        BattleContext.category = _currentCategory;
                        BattleContext.chapter = _currentChapter;
                        if ((possibleItem & (1 << 0)) != 0) BattleContext.genesisCoin = true;
                        if ((possibleItem & (1 << 1)) != 0) BattleContext.originCrystal = true;
                        if ((possibleItem & (1 << 2)) != 0) BattleContext.fateRoneStone = true;
                        if ((possibleItem & (1 << 3)) != 0) BattleContext.heroSeal = true;

                        LoadingManager.Instance.LoadScene("Battle");
                    }
                    else
                    {
                        UIPopupManager.Instance.ShowConfirmPopup(jsonResult["error"].ToString());
                    }
                },
                (PlayFabError error) =>
                {
                    DebugPlayFabError(error);
                });
        }

        #region 오류 디버그
        private void DebugPlayFabError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.ConnectionError:
                case PlayFabErrorCode.ExperimentationClientTimeout:
                    UIPopupManager.Instance.ShowConfirmPopup("네트워크 연결을 확인해주세요.", () =>
                    {
                        SceneManager.LoadScene("Login");
                    });
                    break;
                case PlayFabErrorCode.ServiceUnavailable:
                    UIPopupManager.Instance.ShowConfirmPopup("게임 서버가 불안정합니다.\n나중에 다시 접속해주세요.\n죄송합니다.", () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    });
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
#endif
                    break;
            }
        }
        #endregion
    }
}