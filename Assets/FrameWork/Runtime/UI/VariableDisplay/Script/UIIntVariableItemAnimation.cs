using DG.Tweening;
using EvolveThisMatch.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIIntVariableItemAnimation : MonoBehaviour
    {
        private enum EItemType
        {
            Gold,
        }

        [SerializeField] private EItemType _itemType;

        private PoolSystem _poolSystem;
        private GameObject _prefab;

        private void Awake()
        {
            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            
            SetPrefab();
        }

        private void SetPrefab()
        {
            _prefab = new GameObject($"{_itemType} Item");
            _prefab.hideFlags = HideFlags.HideAndDontSave;
            var image = _prefab.AddComponent<Image>();

            image.rectTransform.sizeDelta = (transform as RectTransform).sizeDelta;
        }

        private void OnEnable()
        {
            switch (_itemType)
            {
                case EItemType.Gold:
                    VariableDisplayManager.Instance.onGoldAnimation += PlayItemCollectAnimation;
                    break;
            }
            
        }

        private void OnDisable()
        {
            switch (_itemType)
            {
                case EItemType.Gold:
                    VariableDisplayManager.Instance.onGoldAnimation -= PlayItemCollectAnimation;
                    break;
            }
        }

        private void PlayItemCollectAnimation(Vector3 startPosition, int itemCount)
        {
            for (int i = 0; i < itemCount; i++)
            {
                GameObject item = _poolSystem.Spawn(_prefab, transform);
                item.transform.position = startPosition;

                Vector2 randomOffset = Random.insideUnitCircle * 200;
                Vector3 spreadPosition = startPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

                Sequence sequence = DOTween.Sequence();
                sequence.Append(item.transform.DOMove(spreadPosition, Random.Range(0.2f, 0.4f)).SetEase(Ease.OutBack));
                sequence.Append(item.transform.DOMove(transform.position, Random.Range(0.4f, 0.6f)).SetEase(Ease.InQuad));

                sequence.OnComplete(() =>
                {
                    _poolSystem.DeSpawn(item);
                });
            }
        }
    }
}