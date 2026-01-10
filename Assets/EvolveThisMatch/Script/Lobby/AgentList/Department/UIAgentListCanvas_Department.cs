using System.Collections.Generic;

namespace EvolveThisMatch.Lobby
{
    public class UIAgentListCanvas_Department : UIAgentListCanvas<UIAgentListItem_Department>
    {
        private void Start()
        {
            ChangeFilterOrder(2);

            _agentListItems[0].SelectItem();
        }

        internal void Show(List<int> deployList)
        {
            foreach (var deploy in deployList)
            {
                foreach (var item in _agentListItems)
                {
                    // 이미 배치된 유닛이면 잠금
                    if (item.template.id == deploy)
                    {
                        item.Lock();
                    }
                    else
                    {
                        item.UnLock();
                    }
                }
            }
        }
    }
}