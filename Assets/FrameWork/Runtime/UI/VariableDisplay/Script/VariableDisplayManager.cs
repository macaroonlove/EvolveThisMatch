using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UI
{
    public class VariableDisplayManager : Singleton<VariableDisplayManager>
    {
        internal UnityAction<Vector3, int> onGoldAnimation;

        public void PlayGoldAnimation(Vector3 startPositon, int itemCount = 5)
        {
            onGoldAnimation?.Invoke(startPositon, itemCount);
        }
    }
}