using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITomeListCanvas : UIBase
    {
        #region 바인딩
        enum Objects
        {
            Content,
        }
        #endregion

        protected Transform _parent;
        protected List<UITomeListItem> _tomeListItems;
        protected List<TomeTemplate> _tomeTemplates;

        protected UnityAction<UITomeListItem> _action;

        internal virtual void Initialize(UnityAction<UITomeListItem> action = null)
        {
            _action = action;

            BindObject(typeof(Objects));

            _parent = GetObject((int)Objects.Content).transform;
        }

        internal void InitializeItem()
        {
            InitializeTomeListItem();

            for (int i = 0; i < _tomeListItems.Count; i++)
            {
                if (!_tomeListItems[i].isEquip)
                {
                    _tomeListItems[i].SelectItem();
                    break;
                }
            }
        }

        #region 리스트 아이템 생성
        private void InitializeTomeListItem()
        {
            _tomeTemplates = GameDataManager.Instance.tomeTemplates.ToList();
            int count = _tomeTemplates.Count;

            _tomeListItems = new List<UITomeListItem>(count);

            var tomeListItem = GetComponentInChildren<UITomeListItem>();

            // 나머지 프리팹 인스턴스 생성
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(tomeListItem.gameObject, _parent).GetComponent<UITomeListItem>();
                item.Initialize(i, ChangeTome);
                _tomeListItems.Add(item);
            }

            Destroy(tomeListItem.gameObject);

            RegistTomeListItem();
        }

        private void ChangeTome(UITomeListItem item)
        {
            DeSelectAllItem();

            _action?.Invoke(item);
        }

        internal void DeSelectAllItem()
        {
            // 모든 아이템 선택 취소
            foreach (var item in _tomeListItems) item.DeSelectItem();
        }

        internal void RegistTomeListItem()
        {
            var ownedTomes = GameDataManager.Instance.profileSaveData.ownedTomes;
            var equipTomes = GameDataManager.Instance.profileSaveData.equipTomes;
            int count = _tomeTemplates.Count;

            // 보유한 고서의 아이디
            var ownedTomeDic = ownedTomes.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                var template = _tomeTemplates[i];

                if (ownedTomeDic.TryGetValue(template.id, out var owned))
                {
                    // 보유한 고서
                    _tomeListItems[i].Show(template, owned);
                }
                else
                {
                    // 미보유 고서
                    _tomeListItems[i].Hide();
                }

                if (equipTomes.Contains(template.id))
                {
                    // 대여된 고서
                    _tomeListItems[i].Hide();
                }
            }
        }
        #endregion

        internal void RentTome(int index)
        {
            int finalIndex = index;
            for (int i = index + 1; i < _tomeListItems.Count; i++) 
            {
                if (!_tomeListItems[i].isEquip)
                {
                    finalIndex = i;
                    break;
                }
            }

            if (finalIndex == index)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (!_tomeListItems[i].isEquip)
                    {
                        finalIndex = i;
                        break;
                    }
                }
            }

            _tomeListItems[finalIndex].SelectItem();
        }

        internal void ReturnTome(int index)
        {
            if (index == -1) return;

            _tomeListItems[index].Show();
        }
    }
}