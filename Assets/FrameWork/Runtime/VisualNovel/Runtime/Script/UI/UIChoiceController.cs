using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;

namespace FrameWork.VisualNovel
{
    public class UIChoiceController : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            ChoiceText,
        }

        enum Buttons
        {
            ChoiceButton,
        }
        #endregion

        private TextMeshProUGUI _choiceText;
        private ChapterTemplate _nextChapter;
        private UnityAction onClick;

        internal void Binding()
        {
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _choiceText = GetText((int)Texts.ChoiceText);
            GetButton((int)Buttons.ChoiceButton).onClick.AddListener(OnClick);
        }

        internal void Initialize(string choiceText, ChapterTemplate nextChapter, UnityAction onClick)
        {
            _choiceText.text = choiceText;
            _nextChapter = nextChapter;
            this.onClick = onClick;
        }

        private void OnClick()
        {
            CommandExecutor.Instance.JumpChapter(_nextChapter);

            onClick?.Invoke();
        }
    }
}
