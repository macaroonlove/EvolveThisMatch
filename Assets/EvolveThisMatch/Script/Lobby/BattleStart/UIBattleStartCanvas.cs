using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        [SerializeField] private WaveLibraryTemplate[] _waveLibraryTemplates;

        private TMP_Dropdown _categoryDropdown;
        private UIChapterItem[] _chapterItems;
        private TextMeshProUGUI _description;
        private TextMeshProUGUI _rewardText;
        private TextMeshProUGUI _battleStartText;

        private int _currentCategory;
        private int _currentChapter;
        private int _selectedUsabilityItem;
        private int _multiplyReward;

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
            foreach (var waveLibrary in _waveLibraryTemplates)
            {
                options.Add(new TMP_Dropdown.OptionData(waveLibrary.title));
            }

            _categoryDropdown.AddOptions(options);
        }

        private void ShowCategory()
        {
            for (int i = 0; i < _chapterItems.Length; i++)
            {
                _chapterItems[i].Show(_waveLibraryTemplates[_currentCategory].waves[i]);
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
                _description.text = $"적 몬스터의 공격력 및 이동속도 {chapter * 10}% 상승";
            }
        }
        #endregion

        private void ChangeItemToggle(bool isOn, int index)
        {
            if (_variables[index].Value <= 0) return;

            int bit = 1 << index;

            if (isOn) _selectedUsabilityItem |= bit;
            else _selectedUsabilityItem  &= ~bit;
        }

        private void MultipleReward(int delta)
        {
            int limit = Mathf.Min(_variables[4].Value / 5, 3);
            _multiplyReward = Mathf.Clamp(_multiplyReward + delta, 1, limit);

            _rewardText.text = $"보상 <size=18>×</size>{_multiplyReward}";
            _battleStartText.text = $"개입 시작\n<sprite name=Action> {_multiplyReward * 5}";
        }

        private void BattleStartButton()
        {

        }
    }
}