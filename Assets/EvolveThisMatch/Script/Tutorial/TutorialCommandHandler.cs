using FrameWork;
using FrameWork.VisualNovel;
using EvolveThisMatch.Core;
using UnityEngine;

namespace EvolveThisMatch.Tutorial
{
    /// <summary>
    /// Tutorial에서 CommandExecutor의 콜백을 받는 클래스
    /// <para>(Core 및 Tutorial과 VisualNovel의 연동을 할 때 사용)</para>
    /// </summary>
    public class TutorialCommandHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            CommandExecutor.Instance.getItem += GetItem;
        }

        private void OnDisable()
        {
            CommandExecutor.Instance.getItem -= GetItem;
        }

        private void GetItem(ItemType itemType, int amount, string name)
        {
            switch (itemType)
            {
                case ItemType.Gold:
                    CoreManager.Instance.GetSubSystem<GoldSystem>().AddGold(amount);
                    break;
                case ItemType.ActiveItem:
                    AddressableAssetManager.Instance.GetScriptableObject<ActiveItemTemplate>(name, (template) =>
                    {
                        CoreManager.Instance.GetSubSystem<ActiveItemSystem>().AddItem(template);
                    });
                    break;
                case ItemType.PassiveItem:
                    AddressableAssetManager.Instance.GetScriptableObject<PassiveItemTemplate>(name, (template) =>
                    {
                        CoreManager.Instance.GetSubSystem<PassiveItemSystem>().AddItem(template);
                    });
                    break;
            }
        }
    }
}