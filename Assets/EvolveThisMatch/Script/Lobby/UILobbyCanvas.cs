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
            DepartmentButton,
            ArtifactButton,
            TomeButton,
            StoreButton,
            GachaButton,
            LootButton,
            FormationButton,
        }
        #endregion

        private UIAgentInfoCanvas _agentInfoCanvas;
        private UIDepartmentCanvas _departmentCanvas;
        private UIArtifactCanvas _artifactCanvas;
        private UITomeCanvas _tomeCanvas;
        private UIShopCanvas _shopCanvas;
        private UIGachaCanvas _gachaCanvas;
        private UIFormationCanvas _formationCanvas;

        protected override void Initialize()
        {
            BindButton(typeof(Buttons));

            _agentInfoCanvas = transform.parent.GetComponentInChildren<UIAgentInfoCanvas>();
            _departmentCanvas = transform.parent.GetComponentInChildren<UIDepartmentCanvas>();
            _artifactCanvas = transform.parent.GetComponentInChildren<UIArtifactCanvas>();
            _tomeCanvas = transform.parent.GetComponentInChildren<UITomeCanvas>();
            _shopCanvas = transform.parent.GetComponentInChildren<UIShopCanvas>();
            _gachaCanvas = transform.parent.GetComponentInChildren<UIGachaCanvas>();
            _formationCanvas = transform.parent.GetComponentInChildren<UIFormationCanvas>();

            GetButton((int)Buttons.BattleStartButton).onClick.AddListener(BattleStart);
            GetButton((int)Buttons.AgentInfoButton).onClick.AddListener(ShowAgentInfo);
            GetButton((int)Buttons.DepartmentButton).onClick.AddListener(ShowDepartment);
            GetButton((int)Buttons.ArtifactButton).onClick.AddListener(ShowArtifact);
            GetButton((int)Buttons.TomeButton).onClick.AddListener(ShowTome);
            GetButton((int)Buttons.StoreButton).onClick.AddListener(ShowShop);
            GetButton((int)Buttons.GachaButton).onClick.AddListener(ShowGacha);
            GetButton((int)Buttons.LootButton).onClick.AddListener(ShowLoot);
            GetButton((int)Buttons.FormationButton).onClick.AddListener(ShowFormation);
        }

        private void BattleStart()
        {

        }

        private void ShowAgentInfo()
        {
            _agentInfoCanvas?.Show(true);
        }

        private void ShowDepartment()
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

        private void ShowShop()
        {
            _shopCanvas?.Show(true);
        }

        private void ShowGacha()
        {
            _gachaCanvas?.Show(true);
        }

        private void ShowLoot()
        {

        }

        private void ShowFormation()
        {
            Hide();
            _formationCanvas?.Show(() => Show(true));
        }
    }
}