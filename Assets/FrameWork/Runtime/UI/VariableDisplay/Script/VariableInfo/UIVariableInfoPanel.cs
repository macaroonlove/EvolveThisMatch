using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIVariableInfoPanel : UIBase
    {
        #region 바인딩
        enum Images
        {
            Background,
            Icon,
        }
        enum Texts
        {
            DisplayName,
            Count,
            DescriptionText,
        }
        enum Objects
        {
            Content,
        }
        #endregion

        private Image _background;
        private Image _icon;
        private TextMeshProUGUI _displayName;
        private TextMeshProUGUI _count;
        private TextMeshProUGUI _descriptionText;

        [SerializeField] private GameObject _prefab;
        private Transform _parent;

        private List<UIAcquisitionLocationItem> _items = new List<UIAcquisitionLocationItem>();

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindObject(typeof(Objects));

            _background = GetImage((int)Images.Background);
            _icon = GetImage((int)Images.Icon);
            _displayName = GetText((int)Texts.DisplayName);
            _count = GetText((int)Texts.Count);
            _descriptionText = GetText((int)Texts.DescriptionText);
            _parent = GetObject((int)Objects.Content).transform;
        }

        internal void Show(VariableInfo info)
        {
            _background.sprite = info.variable.IconBG;
            _icon.sprite = info.variable.Icon;
            _displayName.text = info.variable.DisplayName;
            _count.text = $"{info.variable.Value} 개";
            _descriptionText.text = info.description;

            // 모든 기존 아이템 비활성화
            foreach (var item in _items)
            {
                item.Hide(true);
            }

            // 획득 위치 표시
            for (int i = 0; i < info.acquisitionLocations.Count; i++)
            {
                var item = GetAcquisitionLocationItem(i);
                item.Show(info.acquisitionLocations[i]);
            }
        }

        private UIAcquisitionLocationItem GetAcquisitionLocationItem(int index)
        {
            // 기존에 생성된 아이템이 있으면 재사용
            if (_items.Count > index)
            {
                return _items[index];
            }

            // 없으면 새로 생성
            UIAcquisitionLocationItem newItem = Instantiate(_prefab, _parent).GetComponent<UIAcquisitionLocationItem>();
            _items.Add(newItem);

            return newItem;
        }
    }
}