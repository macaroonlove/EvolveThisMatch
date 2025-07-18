using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public class UILobbyCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            BattleStartButton,
            AgentInfoButton,
            DinerButton,
            ArtifactButton,
            TomeButton,
            StoreButton,
            GachaButton,
            LootButton,
        }
        #endregion

        private UIAgentInfoCanvas _agentInfoCanvas;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));

            _agentInfoCanvas = transform.parent.GetComponentInChildren<UIAgentInfoCanvas>();

            GetButton((int)Buttons.BattleStartButton).onClick.AddListener(BattleStart);
            GetButton((int)Buttons.AgentInfoButton).onClick.AddListener(ShowAgentInfo);
            GetButton((int)Buttons.DinerButton).onClick.AddListener(ShowDiner);
            GetButton((int)Buttons.ArtifactButton).onClick.AddListener(ShowArtifact);
            GetButton((int)Buttons.TomeButton).onClick.AddListener(ShowTome);
            GetButton((int)Buttons.StoreButton).onClick.AddListener(ShowStore);
            GetButton((int)Buttons.GachaButton).onClick.AddListener(ShowGacha);
            GetButton((int)Buttons.LootButton).onClick.AddListener(ShowLoot);
        }

        private void BattleStart()
        {

        }

        private void ShowAgentInfo()
        {
            _agentInfoCanvas.Show();
        }

        private void ShowDiner()
        {

        }

        private void ShowArtifact()
        {

        }

        private void ShowTome()
        {

        }

        private void ShowStore()
        {

        }

        private void ShowGacha()
        {

        }

        private void ShowLoot()
        {

        }
    }
}