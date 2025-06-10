using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.VisualNovel
{
    public class UISCGContainer : MonoBehaviour
    {
        [SerializeField] private GameObject _scgControllerPrefab;

        private Dictionary<int, UISCGController> _controllers = new Dictionary<int, UISCGController>();

        private void OnEnable()
        {
            CommandExecutor.Instance.scgShow += SCGShow;
            CommandExecutor.Instance.scgHide += SCGHide;
            CommandExecutor.Instance.scgMove += SCGMove;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.scgShow -= SCGShow;
            CommandExecutor.Instance.scgHide -= SCGHide;
            CommandExecutor.Instance.scgMove -= SCGMove;
        }

        private void SCGShow(int id, string theme, Rect position, Vector2 anchor)
        {
            if (_controllers.ContainsKey(id) == false)
            {
                var controller = Instantiate(_scgControllerPrefab, transform)?.GetComponent<UISCGController>();

                if (controller == null) return;

                _controllers.Add(id, controller);
            }

            AddressableAssetManager.Instance.GetSprite(theme, (sprite) =>
            {
                _controllers[id].Show(sprite, position, anchor);
            });
        }

        private void SCGHide(int id)
        {
            if (id == -1)
            {
                foreach (var controller in _controllers)
                {
                    controller.Value.Hide();
                }
            }
            else if (_controllers.ContainsKey(id))
            {
                _controllers[id].Hide();
            }
        }

        private void SCGMove(int id, Rect position, Vector2 anchor, float duration, int loopCount, Ease ease, bool isReturn, Ease returnEase)
        {
            if (_controllers.ContainsKey(id) == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"{id} SCG Controller가 등록되어있지 않습니다.");
#endif
                return;
            }

            _controllers[id].Move(position, anchor, duration, loopCount, ease, isReturn, returnEase);
        }
    }
}
