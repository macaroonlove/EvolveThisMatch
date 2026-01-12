using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.Editor;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using System.Collections.Generic;
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

        [Header("전역")]
        [SerializeField, Label("고서 아이템 프리팹")] private GameObject _prefab;

        [Header("이벤트")]
        [SerializeField, Label("고서 데이터 변경 시")] protected GameEvent _tomeDataChangedGameEvent;

        protected Transform _parent;
        protected List<UITomeListItem> _items;

        private UITomeListPresenter _presenter;

        public event UnityAction onRefresh;
        public event UnityAction<TomeTemplate, ItemSaveData.Tome> onSelected;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            _parent = GetObject((int)Objects.Content).transform;

            var model = new UITomeListModel();
            _presenter = new UITomeListPresenter(this, model);
        }

        public void InitializeTomeListItem(int count, UnityAction<int> onSelect)
        {
            _items = new List<UITomeListItem>(count);

            for (int i = 0; i < count; i++)
            {
                int index = i;
                var item = Instantiate(_prefab, _parent).GetComponent<UITomeListItem>();
                item.Initialize(() => onSelect?.Invoke(index));
                _items.Add(item);
            }

            _tomeDataChangedGameEvent.AddListener(() => onRefresh?.Invoke());
        }

        public void RenderItem(int index, TomeListItemViewState state)
        {
            _items[index].Render(state);
        }

        public void SelectItem(int index)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].Select(i == index);
            }
        }

        public void OnSelected(TomeTemplate template, ItemSaveData.Tome owned)
        {
            onSelected?.Invoke(template, owned);
        }

        public void ShowItem(int id)
        {
            _presenter.Show(id);
        }

        public void HideItem(int id)
        {
            _presenter.Hide(id);
        }
    }
}