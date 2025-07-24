using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITalentPanel : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            ResettingButton,
            ResettingFilterButton,
        }
        #endregion

        private UITalentItem[] _items;

        private ProfileSaveData.Agent _owned;
        private UnityAction _action;
        private UnityAction _openFilterPanel;

        protected override void Initialize()
        {
            _items = GetComponentsInChildren<UITalentItem>();

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.ResettingButton).onClick.AddListener(Resetting);
            GetButton((int)Buttons.ResettingFilterButton).onClick.AddListener(ResettingFilterOpen);
        }

        internal void Show(ProfileSaveData.Agent owned, UnityAction action, UnityAction openFilterPanel)
        {
            if (owned == null) return;

            _owned = owned;
            _action = action;
            _openFilterPanel = openFilterPanel;

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Show(owned.talent[i]);
            }
        }

        private void Resetting()
        {
            // TODO: 재능의 가루 사용하도록 구현

            for (int i = 0; i < _items.Length; i++)
            {
                var item = _items[i];

                if (item.isLock == false)
                {
                    item.Resetting();
                }
            }

            _action?.Invoke();
        }

        private void ResettingFilterOpen()
        {
            _openFilterPanel?.Invoke();
        }

        internal async void ResettingFilter(int rarity, List<int> talents)
        {
            while (true)
            {
                bool isBreak = false;
                for (int i = 0; i < _items.Length; i++)
                {
                    var item = _items[i];

                    if (item.isLock == false)
                    {
                        var data = item.Resetting();
                        if (rarity >= ((int)data.GetRarity(item.talent.value).rarity) && talents.Contains(item.talent.id))
                        {
                            isBreak = true;
                        }
                    }
                }
                
                if (isBreak) break;
                
                await UniTask.Yield();
            }

            _action?.Invoke();
        }
    }
}