using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Core
{
    public class UnitRayCastSystem : MonoBehaviour, IBattleSystem
    {
        private Camera _camera;
        private LayerMask _layerMask;

        internal event UnityAction<Unit> onCast;

        public void Initialize()
        {
            
        }

        public void Deinitialize()
        {
            
        }

        private void Awake()
        {
            _camera = Camera.main;
            _layerMask = LayerMask.GetMask("Agent", "Enemy");
        }

        private void Update()
        {
            // 마우스 포인터가 UI위에 있다면 종료
            if (EventSystem.current.IsPointerOverGameObject()) return;

            bool isUp = false;
            Vector3 inputPosition = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0))
            {
                isUp = true;
                inputPosition = Input.mousePosition;
            }
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    isUp = true;
                    inputPosition = touch.position;
                }
            }
#endif

            if (!isUp) return;

            var ray = _camera.ScreenPointToRay(inputPosition);
            Physics.Raycast(ray, out var hitinfo, Mathf.Infinity, _layerMask);

            if (hitinfo.collider != null)
            {
                var unit = hitinfo.collider.GetComponentInParent<Unit>();
                if (unit != null)
                {
                    onCast?.Invoke(unit);
                }
            }
        }
    }
}