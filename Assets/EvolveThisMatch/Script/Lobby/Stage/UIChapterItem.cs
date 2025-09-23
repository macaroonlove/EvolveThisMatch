using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIChapterItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            Label,
        }
        enum Toggles
        {
            ChapterItem,
        }
        #endregion

        private TextMeshProUGUI _label;
        private Toggle _toggle;

        private int _index;
        private UnityAction<int> _changeChapter;

        internal WaveChapter waveChapter { get; private set; }

        internal void Initialize(int index, UnityAction<int> changeChapter)
        {
            _index = index;
            _changeChapter = changeChapter;

            BindText(typeof(Texts));
            BindToggle(typeof(Toggles));

            _label = GetText((int)Texts.Label);

            _toggle = GetToggle((int)Toggles.ChapterItem);
            _toggle.onValueChanged.AddListener(ChangeChapter);
        }

        internal void Show(WaveChapter waveChapter)
        {
            this.waveChapter = waveChapter;

            _label.text = waveChapter.chapterName;
        }

        internal void Select()
        {
            _toggle.isOn = true;
        }

        private void ChangeChapter(bool isOn)
        {
            if (isOn)
            {
                _changeChapter?.Invoke(_index);
            }
        }
    }
}
