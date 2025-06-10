using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.VisualNovel
{
    [RequireComponent(typeof(Button))]
    public class UISkipButton : MonoBehaviour
    {
        private void Awake()
        {
            var nextButton = GetComponent<Button>();
            nextButton.onClick.AddListener(Skip);
        }

        private void Skip()
        {
            CommandExecutor.Instance.Skip();
        }
    }
}