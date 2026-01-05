namespace EvolveThisMatch.Lobby
{
    public class UIAgentListCanvas_Department : UIAgentListCanvas
    {
        private void Start()
        {
            ChangeFilterOrder(2);

            _agentListItems[0].SelectItem();
        }
    }
}