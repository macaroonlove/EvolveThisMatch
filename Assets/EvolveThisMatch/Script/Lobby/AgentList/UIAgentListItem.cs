using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EvolveThisMatch.Lobby
{
    public abstract class UIAgentListItem : UIBase, IPointerClickHandler
    {
        internal AgentTemplate template { get; private set; }
        internal AgentSaveData.Agent owned { get; private set; }

        private UnityAction<AgentTemplate, AgentSaveData.Agent> _action;

        internal virtual void Initialize(UnityAction<AgentTemplate, AgentSaveData.Agent> action = null)
        {
            _action = action;
        }

        internal virtual void Show(AgentTemplate template, AgentSaveData.Agent owned)
        {
            this.template = template;
            this.owned = owned;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
        }

        internal virtual void SelectItem()
        {
            _action?.Invoke(template, owned);
        }

        internal virtual void DeSelectItem()
        {

        }
    }
}