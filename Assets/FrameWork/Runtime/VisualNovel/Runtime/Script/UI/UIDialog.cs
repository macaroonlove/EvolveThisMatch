using FrameWork.UIBinding;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.VisualNovel
{
    public class UIDialog : UIBase
    {
        #region 바인딩
        enum Texts
        {
            Speaker,
            Content,
        }
        #endregion

        private TextMeshProUGUI _speaker;
        private TextMeshProUGUI _content;

        private Coroutine _coroutine;
        private WaitForSeconds _wfs;

        protected override void Initialize()
        {
            BindText(typeof(Texts));

            _speaker = GetText((int)Texts.Speaker);
            _content = GetText((int)Texts.Content);

            _wfs = new WaitForSeconds(0.005f);
        }

        private void OnEnable()
        {
            CommandExecutor.Instance.speakStart += OnSpeakStart;
            CommandExecutor.Instance.speakEnd += OnSpeakEnd;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.speakStart -= OnSpeakStart;
            CommandExecutor.Instance.speakEnd -= OnSpeakEnd;
        }

        #region 콜백 메서드
        private void OnSpeakStart(string speaker, string content, UnityAction onComplete, bool isForce)
        {
            if (isForce)
            {
                ForceExecute(speaker, content, onComplete);
            }
            else
            {
                Execute(speaker, content, onComplete);
            }
        }

        private void OnSpeakEnd()
        {
            Hide(true);
        }
        #endregion

        private void Execute(string speaker, string content, UnityAction onComplete)
        {
            base.Show(true);

            _speaker.text = speaker;
            _coroutine = StartCoroutine(CoTyping(content, onComplete));
        }

        private IEnumerator CoTyping(string content, UnityAction onComplete)
        {
            _content.text = "";
            for (int i = 0; i < content.Length; i++)
            {
                _content.text = content.Substring(0, i + 1);
                yield return _wfs;
            }

            onComplete?.Invoke();
        }

        private void ForceExecute(string speaker, string content, UnityAction onComplete)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _speaker.text = speaker;
            _content.text = content;
            onComplete?.Invoke();
        }
    }
}