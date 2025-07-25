using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentTier : MonoBehaviour
    {
        private float _tierSize;

        private void Awake()
        {
            _tierSize = (transform.GetChild(0) as RectTransform).sizeDelta.x + 3;
        }

        public void Show(int count)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var tier = transform.GetChild(i).gameObject;
                tier.SetActive(i < count);
            }

            RectTransform rect = transform as RectTransform;
            Vector2 size = rect.sizeDelta;
            size.x = count * _tierSize;
            rect.sizeDelta = size;
        }
    }
}