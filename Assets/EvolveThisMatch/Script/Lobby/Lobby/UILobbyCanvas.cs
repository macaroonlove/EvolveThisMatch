using FrameWork.UIBinding;

namespace EvolveThisMatch.Lobby
{
    public class UILobbyCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            BattleStartButton,
            AgentInfoButton,
            ManagementButton,
            ArtifactButton,
            TomeButton,
            StoreButton,
            GachaButton,
            LootButton,
        }
        #endregion

        private UIAgentInfoCanvas _agentInfoCanvas;
        private UIDepartmentCanvas _departmentCanvas;
        private UIArtifactCanvas _artifactCanvas;
        private UITomeCanvas _tomeCanvas;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));

            _agentInfoCanvas = transform.parent.GetComponentInChildren<UIAgentInfoCanvas>();
            _departmentCanvas = transform.parent.GetComponentInChildren<UIDepartmentCanvas>();
            _artifactCanvas = transform.parent.GetComponentInChildren<UIArtifactCanvas>();
            _tomeCanvas = transform.parent.GetComponentInChildren<UITomeCanvas>();

            GetButton((int)Buttons.BattleStartButton).onClick.AddListener(BattleStart);
            GetButton((int)Buttons.AgentInfoButton).onClick.AddListener(ShowAgentInfo);
            GetButton((int)Buttons.ManagementButton).onClick.AddListener(ShowManagement);
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
            _agentInfoCanvas?.Show(true);
        }

        private void ShowManagement()
        {
            _departmentCanvas?.Show(true);
        }

        private void ShowArtifact()
        {
            _artifactCanvas?.Show(true);
        }

        private void ShowTome()
        {
            _tomeCanvas?.Show(true);
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