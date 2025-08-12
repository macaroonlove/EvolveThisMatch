using UnityEngine;

namespace FrameWork.ClickEffect
{
    public class LoadClickEffect
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeClickEffect()
        {
            GameObject prefab = Resources.Load("ClickManager") as GameObject;
            Object.Instantiate(prefab);
        }
    }
}