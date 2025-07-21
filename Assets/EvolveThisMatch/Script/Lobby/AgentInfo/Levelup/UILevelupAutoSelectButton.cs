using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UILevelupAutoSelectButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private bool _isPress;
        private float _pressTime;
        private float _intervalTimer;

        private UnityAction _action;

        internal void Initialize(UnityAction action)
        {
            _action = action;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Transfer();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPress = true;
            _pressTime = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPress = false;
            _pressTime = 0f;
        }

        private void Update()
        {
            if (_isPress)
            {
                _pressTime += Time.deltaTime;

                // 0.3초 이후에 매 프레임 인식
                if (_pressTime >= 0.3f)
                {
                    _intervalTimer += Time.deltaTime;

                    if (_intervalTimer >= 0.1f)
                    {
                        Transfer();
                        _intervalTimer = 0;
                    }
                }
            }
        }

        private void Transfer()
        {
            _action?.Invoke();
        }
    }
}