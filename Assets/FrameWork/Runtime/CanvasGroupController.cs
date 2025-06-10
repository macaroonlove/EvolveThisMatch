using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace FrameWork
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        [SerializeField] private bool blocksRaycasts = true;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private Ease ease = Ease.OutCubic;

        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;

        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void ShowOrHide(bool isShow)
        {
            if (isShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show(bool isForce = false)
        {
            if (isForce)
            {
                DoShow();
            }
            else if (_canvasGroup.alpha != 1)
            {
                _canvasGroup.DOFade(1f, duration).SetEase(ease).OnComplete(
                () =>
                {
                    DoShow();
                }).SetUpdate(true);
            }
        }

        public void Hide(bool isForce = false)
        {
            if (isForce)
            {
                DoHide();
            }
            else
            {
                _canvasGroup.DOFade(0f, duration).SetEase(ease).OnComplete(
                () =>
                {
                    DoHide();
                }).SetUpdate(true);
            }
        }

        [ContextMenu("Show")]
        private void DoShow()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            _canvasGroup.alpha = 1.0f;
            _canvasGroup.blocksRaycasts = blocksRaycasts;
            _canvasGroup.interactable = true;
        }

        [ContextMenu("Hide")]
        private void DoHide()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            _canvasGroup.alpha = 0.0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CanvasGroupController))]
    public class CanvasGroupControllerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show"))
            {
                ((CanvasGroupController)target).Show(true);
            }
            if (GUILayout.Button("Hide"))
            {
                ((CanvasGroupController)target).Hide(true);
            }
            EditorGUILayout.EndHorizontal();
        }

    }
#endif
}