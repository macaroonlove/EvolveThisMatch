using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentTier : MonoBehaviour
    {
        public void Show(int count)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var tier = transform.GetChild(i).gameObject;
                tier.SetActive(i < count);
            }

            RectTransform rect = transform as RectTransform;
            Vector2 size = rect.sizeDelta;
            size.x = count * 54;
            rect.sizeDelta = size;
        }
    }
}