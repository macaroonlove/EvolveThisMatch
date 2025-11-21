using CodeStage.AntiCheat.ObscuredTypes;
using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UIStagePanel : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            EnterStageButton,
        }
        enum Dropdowns
        {
            CategoryDropdown,
        }
        enum Texts
        {
            StageName,
        }
        #endregion

        private TMP_Dropdown _categoryDropdown;
        private TextMeshProUGUI _stageName;

        [SerializeField] private WaveLibraryTemplate _waveLibraryTemplates;

        private LobbyWaveSystem _lobbyWaveSystem;
        private UIChapterItem[] _chapterItems;
        private UIStageItem[] _stageItems;
        private UIStageEnemyItem[] _stageEnemyItems;

        private UIStageItem _currentStageItem;
        private StageData _stageData;
        private int _currentCategory;
        private int _currentChapter;
        private int _currentStage;

        private UnityAction _onClose;

        private const string _stageKey = "CurrentStage";

        protected override void Initialize()
        {
            _chapterItems = GetComponentsInChildren<UIChapterItem>();
            _stageItems = GetComponentsInChildren<UIStageItem>();
            _stageEnemyItems = GetComponentsInChildren<UIStageEnemyItem>();

            BindDropdown(typeof(Dropdowns));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));

            _stageName = GetText((int)Texts.StageName);
            _categoryDropdown = GetDropdown((int)Dropdowns.CategoryDropdown);
            InitializeCategory();

            for (int i = 0; i < _chapterItems.Length; i++)
            {
                _chapterItems[i].Initialize(i, ChangeChapter);
            }

            for (int i = 0; i < _stageItems.Length; i++)
            {
                int idx = i;
                _stageItems[i].Initialize((currentStage) => ChangeStage(idx, currentStage));
            }

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.EnterStageButton).onClick.AddListener(EnterStage);
        }

        private void Start()
        {
            _lobbyWaveSystem = BattleManager.Instance.GetSubSystem<LobbyWaveSystem>();

            _stageData = LoadStageData();

            SyncStageData();

            var waveTemplate = _waveLibraryTemplates.categorys[_currentCategory].chapters[_currentChapter].waves[_currentStage];
            _lobbyWaveSystem.ChangeWave(waveTemplate);
        }

        #region 스테이지 데이터
        private StageData LoadStageData()
        {
            if (!PlayerPrefs.HasKey(_stageKey))
            {
                return new StageData(0, 0, 0);
            }

            string json = PlayerPrefs.GetString(_stageKey);
            return JsonUtility.FromJson<StageData>(json);
        }

        private void SaveStageData()
        {
            _stageData.SetStageData(_currentCategory, _currentChapter, _currentStage);

            string json = JsonUtility.ToJson(_stageData);
            PlayerPrefs.SetString(_stageKey, json);
            PlayerPrefs.Save();
        }

        private void SyncStageData()
        {
            _currentCategory = _stageData.category;
            _currentChapter = _stageData.chapter;
            _currentStage = _stageData.stage;
        }

        private bool MatchStageData(int category, int chapter, int stage)
        {
            return _stageData.category == category && _stageData.chapter == chapter && _stageData.stage == stage;
        }
        #endregion

        internal void Show(UnityAction onClose)
        {
            _onClose = onClose;

            SyncStageData();

            _categoryDropdown.value = _currentCategory;

            ShowCategory();
            ShowChapter();

            _chapterItems[_currentChapter].Select();
            ChangeStage(_currentStage, _stageItems[_currentStage]);

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
        private void ShowChapter()
        {
            for (int i = 0; i < _stageItems.Length; i++)
            {
                bool isActiveStage = MatchStageData(_currentCategory, _currentChapter, i);
                _stageItems[i].Show(_chapterItems[_currentChapter].waveChapter.waves[i], isActiveStage);
            }
        }

        private void ChangeChapter(int chapter)
        {
            _currentChapter = chapter;

            ShowChapter();
            ChangeStage(0, _stageItems[0]);
        }
        #endregion

        #region 스테이지
        private void ChangeStage(int currentStage, UIStageItem currentStageItem)
        {
            _currentStage = currentStage;

            _currentStageItem?.DeSelect();
            _currentStageItem = currentStageItem;
            currentStageItem.Select();

            ShowStageInfo(currentStageItem.waveTemplate);
        }

        private void ShowStageInfo(WaveTemplate waveTemplate)
        {
            _stageName.text = $"<size=32>{waveTemplate.stage}</size>\n{waveTemplate.displayName}";

            foreach (var item in _stageEnemyItems)
            {
                item.Hide();
            }

            if (waveTemplate.waveInfo.Count > 0)
            {
                // 중복 방지
                HashSet<EnemyData> uniqueEnemies = new HashSet<EnemyData>();

                foreach (var info in waveTemplate.waveInfo)
                {
                    if (info.spawnCount > 0)
                    {
                        var enemyData = waveTemplate.GetEnemyData(info.rarity);
                        if (enemyData != null)
                            uniqueEnemies.Add(enemyData);
                    }
                }

                // 초기화하기
                int index = 0;
                foreach (var enemy in uniqueEnemies)
                {
                    if (index >= _stageEnemyItems.Length) break;

                    _stageEnemyItems[index].Show(enemy);
                    index++;
                }
            }
        }

        private void EnterStage()
        {
            _lobbyWaveSystem.ChangeWave(_currentStageItem.waveTemplate);
            SaveStageData();

            Hide();
        }
        #endregion

        private void Hide()
        {
            Hide(true);
            _onClose?.Invoke();
        }
    }

    [System.Serializable]
    public class StageData
    {
        [SerializeField] private ObscuredInt _category;
        [SerializeField] private ObscuredInt _chapter;
        [SerializeField] private ObscuredInt _stage;

        public int category => _category;
        public int chapter => _chapter;
        public int stage => _stage;

        public StageData(int category, int chapter, int stage)
        {
            _category = category;
            _chapter = chapter;
            _stage = stage;
        }

        public void SetStageData(int category, int chapter, int stage)
        {
            _category = category;
            _chapter = chapter;
            _stage = stage;
        }
    }
}