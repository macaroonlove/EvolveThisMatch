using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Lobby
{
    public class UIButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(_originalScale * 1.1f, 0.1f).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(_originalScale, 0.1f).SetEase(Ease.OutQuad);
        }
    }
}