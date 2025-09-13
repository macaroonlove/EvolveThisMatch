using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using UnityEngine.Events;

namespace EvolveThisMatch.Lobby
{
    public class UITomeCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            CloseButton,
        }
        #endregion

        private UITomeListCanvas _tomeListCanvas;
        private UITomeInfoCanvas _tomeInfoCanvas;
        private UITomeEquipCanvas _tomeEquipCanvas;
        private UnityAction _onClose;

        protected override void Initialize()
        {
            _tomeListCanvas = GetComponentInChildren<UITomeListCanvas>();
            _tomeInfoCanvas = GetComponentInChildren<UITomeInfoCanvas>();
            _tomeEquipCanvas = GetComponentInChildren<UITomeEquipCanvas>();

            _tomeListCanvas.Initialize((UITomeListItem item) => { _tomeEquipCanvas.SelectTomeListItem(item); _tomeInfoCanvas.Show(item.template, item.owned); });
            _tomeEquipCanvas.Initialize((UITomeEquipItem item) => { _tomeInfoCanvas.Show(item.template, item.owned); }, (int index) => _tomeListCanvas.RentTome(index), (int index) => _tomeListCanvas.ReturnTome(index));
            _tomeInfoCanvas.Initialize(_tomeListCanvas.RegistTomeListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
        }

        private void Start()
        {
            _tomeEquipCanvas.InitializeItem();
            _tomeListCanvas.InitializeItem();
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            Show(true);
        }

        private void Hide()
        {
            _onClose?.Invoke();

            Hide(true);
        }
    }
}