using FrameWork.UIBinding;

namespace FrameWork.VisualNovel
{
    public class UINextButton : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            NextButton,
        }
        #endregion

        private void Awake()
        {
            BindButton(typeof(Buttons));

            GetButton((int)Buttons.NextButton).onClick.AddListener(Next);
        }

        private void OnEnable()
        {
            CommandExecutor.Instance.onNextButtonInteractableChanged += OnNextButtonInteractableChanged;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.onNextButtonInteractableChanged -= OnNextButtonInteractableChanged;
        }

        private void OnNextButtonInteractableChanged(bool isOn)
        {
            if (isOn) Show(true);
            else Hide(true);
        }

        private void Next()
        {
            CommandExecutor.Instance.Next();
        }
    }
}