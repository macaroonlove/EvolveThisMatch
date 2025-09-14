using FrameWork.UIBinding;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIVariableListItem : UIBase, IPointerClickHandler
    {
        #region ¹ÙÀÎµù
        enum Images
        {
            Background,
            Icon,
            SelectDim,
        }
        #endregion

        private Image _selectDim;

        private UnityAction<VariableInfo> _action;

        internal VariableInfo info { get; private set; }

        internal void Initialize(VariableInfo info, UnityAction<VariableInfo> action)
        {
            this.info = info;
            _action = action;

            BindImage(typeof(Images));

            _selectDim = GetImage((int)Images.SelectDim);

            GetImage((int)Images.Background).sprite = info.variable.IconBG;
            GetImage((int)Images.Icon).sprite = info.variable.Icon;
        }

        internal void SelectItem()
        {
            _action?.Invoke(info);

            _selectDim.enabled = true;
        }

        internal void DeSelectItem()
        {
            _selectDim.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }
    }
}