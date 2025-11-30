using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    [RequireComponent(typeof(Image))]
    public class UIImageSpriteToggle : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [Header("Sprites")]
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        private void Reset()
        {
            _image = GetComponent<Image>();
        }

        public void SetSprite(bool isOn)
        {
            if (_image == null) return;

            if (isOn && _onSprite != null)
                _image.sprite = _onSprite;
            else if (!isOn && _offSprite != null)
                _image.sprite = _offSprite;
        }
    }
}