using TMPro;
using UnityEngine;

namespace FrameWork.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UITextColorToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _onColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color _offColor = new Color(0.133f, 0.133f, 0.133f, 1);

        private void Reset()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void SetTextColor(bool isOn)
        {
            if (_text != null)
                _text.color = isOn ? _onColor : _offColor;
        }
    }
}