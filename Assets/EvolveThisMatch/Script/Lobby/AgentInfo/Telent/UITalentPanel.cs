using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = FrameWork.PlayFabExtensions.Random;

namespace EvolveThisMatch.Lobby
{
    public class UITalentPanel : UIBase
    {
        #region 바인딩
        enum Texts
        {
            ChangeTalentText,
        }
        enum Buttons
        {
            ChangeTalentButton,
            OpenTalentFilterButton,
        }
        enum CanvasGroups
        {
            Dim,
        }
        #endregion

        private TextMeshProUGUI _changeTalentText;
        private Button _changeTalentButton;
        private Button _openTalentFilterButton;
        private CanvasGroupController _dim;
        private UITalentItem[] _items;

        private CurrencySystem _currencySystem;
        private AgentSaveData.Agent _owned;
        private TalentSaveData _talentSaveData;
        private UnityAction _action;
        private UnityAction _openFilterPanel;

        private bool _isStopResetting;
        private int _cachedPowderCount;
        private float[] _rarityProbabilities;

        private Dictionary<int, Random> _rngs = new Dictionary<int, Random>();

        #region 초기화
        protected override void Initialize()
        {
            _items = GetComponentsInChildren<UITalentItem>();

            BindText(typeof(Texts));
            BindButton(typeof(Buttons));
            BindCanvasGroupController(typeof(CanvasGroups));

            _changeTalentText = GetText((int)Texts.ChangeTalentText);
            _changeTalentButton = GetButton((int)Buttons.ChangeTalentButton);
            _openTalentFilterButton = GetButton((int)Buttons.OpenTalentFilterButton);
            _dim = GetCanvasGroupController((int)CanvasGroups.Dim);

            _isStopResetting = false;

            _changeTalentButton.onClick.AddListener(ChangeTalent);
            _openTalentFilterButton.onClick.AddListener(() => _openFilterPanel?.Invoke());

            InitializeTalentProbabilities();
        }

        private async void InitializeTalentProbabilities()
        {
            await UniTask.WaitUntil(() => SaveManager.Instance.agentData.isLoaded);

            var data = AgentSaveDataTemplate.talentTitleData;
            _rarityProbabilities = new float[5];
            _rarityProbabilities[0] = data.mythRarity;
            _rarityProbabilities[1] = data.legendRarity;
            _rarityProbabilities[2] = data.epicRarity;
            _rarityProbabilities[3] = data.rareRarity;
            _rarityProbabilities[4] = data.commonRarity;
        }
        #endregion

        internal void Show(AgentSaveData.Agent owned, UnityAction action, UnityAction openFilterPanel)
        {
            _owned = null;

            if (owned == null) return;

            if (owned.tier <= 2)
            {
                _dim.Show(true);
                return;
            }

            _dim.Hide(true);

            _owned = owned;
            _action = action;
            _openFilterPanel = openFilterPanel;

            var agentData = SaveManager.Instance.agentData;
            _talentSaveData = agentData.GetTalentSaveData(owned.id);

            for (int i = 0; i < _items.Length; i++)
            {
                int idx = i;

                _items[idx].Show(_talentSaveData.finalTalent[idx], (isOn) =>
                {
                    var lockEntry = new TalentSaveData.LockHistory
                    {
                        order = _talentSaveData.rollCount,
                        index = idx,
                        isLock = isOn
                    };
                    _talentSaveData.lockHistory.Add(lockEntry);

                    agentData.SaveTalentLocalData();

                    UpdateUI();
                });
            }

            UpdateUI();
            CheckEnoughPowder();
        }

        internal void ClearRNG()
        {
            _rngs.Clear();
        }

        private Random GetRng(int unitId, int seed)
        {
            if (!_rngs.TryGetValue(unitId, out var rng))
            {
                rng = new Random(seed);
                _rngs[unitId] = rng;
            }
            return rng;
        }

        #region 재능 돌리기
        private void ChangeTalent()
        {
            // 재설정이 불가능하다면
            if (!PayPowder()) return;

            foreach (var item in _items)
            {
                TryChangeTalentItem(item);
            }

            ApplyRollCount();
            SaveManager.Instance.agentData.SaveTalentLocalData();

            _action?.Invoke();
        }

        internal async void ChangeTalent(int rarity, List<int> talents)
        {
            _changeTalentText.text = "중지";
            _isStopResetting = false;

            while (!_isStopResetting)
            {
                // 재설정이 불가능하다면
                if (!PayPowder()) break;

                bool isBreak = false;

                foreach (var item in _items)
                {
                    if (TryChangeTalentItem(item, rarity, talents))
                    {
                        isBreak = true;
                    }
                }

                ApplyRollCount();

                if (isBreak) break;

                await UniTask.Yield();
            }

            SaveManager.Instance.agentData.SaveTalentLocalData();

            UpdateUI();
            _action?.Invoke();
        }

        private void ApplyRollCount()
        {
            _talentSaveData.rollCount++;

            if (_talentSaveData.rollCount % 50 == 0)
            {
                SaveManager.Instance.agentData.SaveTalentLocalData();
            }
        }

        private bool TryChangeTalentItem(UITalentItem item, int rarity = -1, List<int> talents = null)
        {
            if (item.isLock) return false;

            var isSuccess = item.Resetting(GetRng(_owned.id, _owned.seed), _rarityProbabilities, rarity, talents);

            // 조건 없이 재설정
            if (rarity == -1 || talents == null) return false;

            // 조건에 만족하면 루프 중단 (true)
            return isSuccess;
        }
        #endregion

        #region Powder 계산
        private void CheckEnoughPowder()
        {
            if (_currencySystem == null) _currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();

            bool isEnough = _currencySystem.CheckCurrency(CurrencyType.Powder, _cachedPowderCount);

            _changeTalentButton.interactable = isEnough;
            _openTalentFilterButton.interactable = isEnough;
        }

        private bool PayPowder()
        {
            bool isEnough = _currencySystem.CheckCurrency(CurrencyType.Powder, _cachedPowderCount);

            if (isEnough == false)
            {
                _changeTalentButton.interactable = isEnough;
                _openTalentFilterButton.interactable = isEnough;
            }

            return isEnough;
        }
        #endregion

        internal void StopFilter()
        {
            _isStopResetting = true;
        }

        private void UpdateUI()
        {
            _changeTalentText.text = GetChangeTalentText();
        }

        internal string GetChangeTalentText()
        {
            int powderCount = 5;

            foreach (var item in _items)
            {
                if (item.isLock)
                {
                    powderCount += 5;
                }
            }

            _cachedPowderCount = powderCount;

            return $"<sprite name=Powder> {powderCount}\n개화";
        }
    }
}