using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

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

        private UnityAction _action;
        private UnityAction _openFilterPanel;

        private bool _isStopResetting;
        private int _cachedPowderCount;

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
        }

        internal void Show(AgentSaveData.Agent owned, UnityAction action, UnityAction openFilterPanel)
        {
            if (owned == null) return;

            if (owned.tier <= 2)
            {
                _dim.Show(true);
                return;
            }

            _dim.Hide(true);

            _action = action;
            _openFilterPanel = openFilterPanel;

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Show(owned.talent[i], UpdateUI);
            }

            UpdateUI();
            CheckEnoughPowder();
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

                if (isBreak) break;

                await UniTask.Yield();
            }

            UpdateUI();
            _action?.Invoke();
        }

        private bool TryChangeTalentItem(UITalentItem item, int rarity = -1, List<int> talents = null)
        {
            if (item.isLock) return false;

            var data = item.Resetting();

            // 조건 없이 재설정
            if (rarity == -1 || talents == null)
                return false;

            // 조건에 만족하면 루프 중단 (true)
            return rarity >= (int)data.GetRarity(item.talent.value).rarity && talents.Contains(item.talent.id);
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