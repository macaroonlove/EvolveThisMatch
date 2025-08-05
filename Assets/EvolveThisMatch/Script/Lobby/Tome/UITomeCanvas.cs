using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;

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

        protected override void Initialize()
        {
            _tomeListCanvas = GetComponentInChildren<UITomeListCanvas>();
            _tomeInfoCanvas = GetComponentInChildren<UITomeInfoCanvas>();
            _tomeEquipCanvas = GetComponentInChildren<UITomeEquipCanvas>();

            _tomeListCanvas.Initialize((UITomeListItem item) => { _tomeEquipCanvas.SelectTomeListItem(item); _tomeInfoCanvas.Show(item.template, item.owned); });
            _tomeEquipCanvas.Initialize((UITomeEquipItem item) => { _tomeInfoCanvas.Show(item.template, item.owned); }, (int index) => _tomeListCanvas.RentTome(index), (int index) => _tomeListCanvas.ReturnTome(index));
            _tomeInfoCanvas.Initialize(_tomeListCanvas.RegistTomeListItem);

            BindButton(typeof(Buttons));

            GetButton((int)Buttons.CloseButton).onClick.AddListener(() => Hide(true));
        }

        private void Start()
        {
            _tomeEquipCanvas.InitializeItem();
            _tomeListCanvas.InitializeItem();
        }
    }
}