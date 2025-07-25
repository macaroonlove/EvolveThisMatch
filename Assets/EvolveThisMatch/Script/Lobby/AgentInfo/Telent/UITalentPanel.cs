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

        protected override async void Initialize()
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

            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            _currencySystem = CoreManager.Instance.GetSubSystem<CurrencySystem>();
        }

        internal void Show(ProfileSaveData.Agent owned, UnityAction action, UnityAction openFilterPanel)
        {
            if (owned == null) return;

            if (owned.tier <= 1)
            {
                _dim.Show(true);
                return;
            }

            _dim.Hide(true);

            _action = action;
            _openFilterPanel = openFilterPanel;

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Show(owned.talent[i], () => SetChangeTalentText());
            }

            SetChangeTalentText();

            bool isAble = _currencySystem.CheckCurrency(CurrencyType.Powder, _cachedPowderCount);

            _changeTalentButton.interactable = isAble;
            _openTalentFilterButton.interactable = isAble;
        }

        private void ChangeTalent()
        {
            // 재설정이 불가능하다면
            if (IsAbleResetting() == false) return;

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

            while (_isStopResetting == false)
            {
                // 재설정이 불가능하다면
                if (IsAbleResetting() == false) break;

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

            SetChangeTalentText();
            _action?.Invoke();
        }

        private bool IsAbleResetting()
        {
            if (_currencySystem.PayCurrency(CurrencyType.Powder, _cachedPowderCount) == false)
            {
                _changeTalentButton.interactable = false;
                _openTalentFilterButton.interactable = false;

                return false;
            }

            return true;
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

        internal void StopFilter()
        {
            _isStopResetting = true;
        }

        private void SetChangeTalentText()
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