using TMPro;
using UnityEngine;

namespace FrameWork.Tooltip
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(TooltipTrigger))]
    public class TMProLinkTooltip : MonoBehaviour
    {
        [SerializeField] private TooltipTemplate _template;
        [SerializeField] private bool _isChangeColor;
        [SerializeField] private Color _hoverColor;

        private TextMeshProUGUI _text;
        private TooltipTrigger _trigger;

        private Vector3 _lastMousePosition;
        private int _cachedLinkIndex = -1;
        private string _originalText;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _trigger = GetComponent<TooltipTrigger>();
            _trigger.isHover = false;
        }

        private void OnDisable()
        {
            if (PersistentLoad.isLoaded)
            {
                _trigger?.StopHover();
            }
        }

        private void LateUpdate()
        {
            Vector3 mousePos = Input.mousePosition;

            // 변경 없으면 아무것도 하지 않음
            if (_lastMousePosition == mousePos && _cachedLinkIndex == -1)
                return;

            _lastMousePosition = mousePos;

            // 마우스가 텍스트 영역 안에 없다면
            if (TMP_TextUtilities.IsIntersectingRectTransform(_text.rectTransform, mousePos, null) == false)
            {
                ResetTooltip();
                return;
            }

            // 몇 번째 link태그인지 구하기
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, mousePos, null);

            // 링크를 벗어난 경우
            if (linkIndex != _cachedLinkIndex)
            {
                ResetTooltip();

                // 마우스가 링크 위에 없을 경우
                if (linkIndex == -1) return;

                _cachedLinkIndex = linkIndex;

                // 색상 변경
                if (_isChangeColor) SetColor();

                // 툴팁 처리
                ShowTooltip(linkIndex);
            }
        }

        #region Show/Hide
        private void ShowTooltip(int linkIndex)
        {
            if (_trigger == null) return;

            string linkID = _text.textInfo.linkInfo[linkIndex].GetLinkID();
            LinkTooltip linkTooltip = _template.linkTooptips.Find(x => x.linkName == linkID);

            if (linkTooltip == null) return;

            if (linkTooltip.data.IsInitializeData() == false) linkTooltip.data.InitializeData();

            _trigger.tooltipStyle = linkTooltip.style;
            _trigger.tooltipData = linkTooltip.data;
            _trigger.StartHover();
        }

        private void ResetTooltip()
        {
            if (_cachedLinkIndex == -1) return;

            if (_isChangeColor) ResetColor();

            _trigger?.StopHover();
            _cachedLinkIndex = -1;
        }
        #endregion

        #region 색상 적용
        private void SetColor()
        {
            if (string.IsNullOrEmpty(_originalText))
                _originalText = _text.text;

            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[_cachedLinkIndex];

            int startIndex = GetRichTextIndex(_originalText, linkInfo.linkTextfirstCharacterIndex) + 9 + linkInfo.linkIdLength;
            int length = linkInfo.linkTextLength + 7;
            string before = _originalText.Substring(0, startIndex);
            string linkText = _originalText.Substring(startIndex, length);
            string after = _originalText.Substring(startIndex + length);

            // Color 태그를 입히기
            string coloredLinkText = $"<color=#{ColorUtility.ToHtmlStringRGBA(_hoverColor)}>{linkText}</color>";

            // 텍스트 적용
            _text.text = before + coloredLinkText + after;
        }
        
        /// <summary>
        /// 랜더링된 텍스트의 인덱스를 리치 텍스트(태그)를 포함한 문자열의 인덱스로 변환
        /// </summary>
        private int GetRichTextIndex(string richText, int renderTextIndex)
        {
            int richIndex = 0;
            int renderIndex = 0;
            bool insideTag = false;

            while (richIndex < richText.Length && renderIndex < renderTextIndex)
            {
                char c = richText[richIndex];

                if (c == '<')
                {
                    insideTag = true;
                }
                else if (c == '>' && insideTag)
                {
                    insideTag = false;
                    richIndex++;
                    continue;
                }

                if (!insideTag)
                {
                    renderIndex++;
                }

                richIndex++;
            }

            return richIndex;
        }

        private void ResetColor()
        {
            if (!string.IsNullOrEmpty(_originalText))
            {
                _text.text = _originalText;
                _originalText = null;
            }
        }
        #endregion
    }
}
