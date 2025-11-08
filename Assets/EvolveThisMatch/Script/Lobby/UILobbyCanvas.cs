using Cysharp.Threading.Tasks;
using DG.Tweening;
using FrameWork;
using FrameWork.UI;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
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
            DepartmentButton,
            ArtifactButton,
            TomeButton,
            StoreButton,
            GachaButton,
            LootButton,
            FormationButton,
            StageDisplay,
        }
        enum Toggles
        {
            MenuToggle,
        }
        enum CanvasGroups
        {
            MenuPanel,
        }
        #endregion

        private UIAgentInfoCanvas _agentInfoCanvas;
        private UIDepartmentCanvas _departmentCanvas;
        private UIArtifactCanvas _artifactCanvas;
        private UITomeCanvas _tomeCanvas;
        private UIShopCanvas _shopCanvas;
        private UIGachaCanvas _gachaCanvas;
        private UIFormationCanvas _formationCanvas;
        private UIStagePanel _stagePanel;
        private UIBattleStartCanvas _battleStartCanvas;

        private CanvasGroupController _menuPanel;

        protected override void Initialize()
        {
            ShowVariable();

            BindButton(typeof(Buttons));
            BindToggle(typeof(Toggles));
            BindCanvasGroupController(typeof(CanvasGroups));

            _agentInfoCanvas = transform.parent.GetComponentInChildren<UIAgentInfoCanvas>();
            _departmentCanvas = transform.parent.GetComponentInChildren<UIDepartmentCanvas>();
            _artifactCanvas = transform.parent.GetComponentInChildren<UIArtifactCanvas>();
            _tomeCanvas = transform.parent.GetComponentInChildren<UITomeCanvas>();
            _shopCanvas = transform.parent.GetComponentInChildren<UIShopCanvas>();
            _gachaCanvas = transform.parent.GetComponentInChildren<UIGachaCanvas>();
            _formationCanvas = transform.parent.GetComponentInChildren<UIFormationCanvas>();
            _stagePanel = transform.parent.GetComponentInChildren<UIStagePanel>();
            _battleStartCanvas = transform.parent.GetComponentInChildren<UIBattleStartCanvas>();

            _menuPanel = GetCanvasGroupController((int)CanvasGroups.MenuPanel);
            _menuPanel.Hide(true);

            GetButton((int)Buttons.BattleStartButton).onClick.AddListener(BattleStart);
            GetButton((int)Buttons.AgentInfoButton).onClick.AddListener(ShowAgentInfo);
            GetButton((int)Buttons.DepartmentButton).onClick.AddListener(ShowDepartment);
            GetButton((int)Buttons.ArtifactButton).onClick.AddListener(ShowArtifact);
            GetButton((int)Buttons.TomeButton).onClick.AddListener(ShowTome);
            GetButton((int)Buttons.StoreButton).onClick.AddListener(ShowShop);
            GetButton((int)Buttons.GachaButton).onClick.AddListener(ShowGacha);
            GetButton((int)Buttons.LootButton).onClick.AddListener(ShowLoot);
            GetButton((int)Buttons.FormationButton).onClick.AddListener(ShowFormation);
            GetButton((int)Buttons.StageDisplay).onClick.AddListener(ShowStagePanel);
            GetToggle((int)Toggles.MenuToggle).onValueChanged.AddListener(OnChangedMenu);
        }

        private async void ShowVariable()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            VariableDisplayManager.Instance.HideAll();

            var gold = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("Gold");
            var essence = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("Essence");
            var loot = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("Loot");
            var action = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>("Action");

            if (gold != null) VariableDisplayManager.Instance.Show(gold);
            if (essence != null) VariableDisplayManager.Instance.Show(essence);
            if (loot != null) VariableDisplayManager.Instance.Show(loot);
            if (action != null) VariableDisplayManager.Instance.Show(action);
        }

        private void BattleStart()
        {
            _battleStartCanvas?.Show();
        }

        #region Bottom Left
        private void ShowAgentInfo()
        {
            VariableDisplayManager.Instance.HideAll();

            _agentInfoCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }

        private void ShowDepartment()
        {
            VariableDisplayManager.Instance.HideAll();

            _departmentCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }

        private void ShowArtifact()
        {
            VariableDisplayManager.Instance.HideAll();

            _artifactCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }

        private void ShowTome()
        {
            VariableDisplayManager.Instance.HideAll();

            _tomeCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }

        private void ShowShop()
        {
            VariableDisplayManager.Instance.HideAll();

            _shopCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }

        private void ShowGacha()
        {
            VariableDisplayManager.Instance.HideAll();

            _gachaCanvas?.Show(() =>
            {
                ShowVariable();
            });
        }
        #endregion

        #region Bottom Right
        private void ShowLoot()
        {

        }

        private void ShowFormation()
        {
            VariableDisplayManager.Instance.HideAll();
            Hide();

            _formationCanvas?.Show(() =>
            {
                ShowVariable();
                Show(true);
            });
        }
        #endregion

        #region Top Right
        private void OnChangedMenu(bool isOn)
        {
            if (isOn)
            {
                _menuPanel.transform.localScale = new Vector3(1, 0, 1);
                _menuPanel.Show(true);

                _menuPanel.transform.DOScaleY(1, 0.1f);
            }
            else
            {
                _menuPanel.transform.DOScaleY(0, 0.1f).OnComplete(() => { _menuPanel.Hide(true); });
            }
        }

        private void ShowStagePanel()
        {
            VariableDisplayManager.Instance.HideAll();

            _stagePanel?.Show(() =>
            {
                ShowVariable();
            });
        }
        #endregion
    }
}