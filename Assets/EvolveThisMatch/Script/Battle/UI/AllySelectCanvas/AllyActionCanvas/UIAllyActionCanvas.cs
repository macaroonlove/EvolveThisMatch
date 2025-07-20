using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UIAllyActionCanvas : UIBase
    {
        #region 바인딩
        enum Texts
        {
            SortieText,
        }
        enum Buttons
        {
            SortieButton,
            ExtractButton,
        }
        enum Objects
        {
            Pivot,
        }
        #endregion

        private Transform _pivot;
        private TextMeshProUGUI _sortieText;
        private Button _sortieButton;
        private Button _extractButton;

        private UIAllySelectCanvas _allySelectCanvas;
        private TileSystem _tileSystem;
        private AgentReturnSystem _agentReturnSystem;
        private Camera _camera;
        private AgentBattleData _selectedData;
        private DeployAbility _deployAbility;

        protected override void Initialize()
        {
            _camera = Camera.main;
            _allySelectCanvas = GetComponentInParent<UIAllySelectCanvas>();
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();
            _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();

            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindObject(typeof(Objects));

            _sortieText = GetText((int)Texts.SortieText);
            _sortieButton = GetButton((int)Buttons.SortieButton);
            _extractButton = GetButton((int)Buttons.ExtractButton);
            _pivot = GetObject((int)Objects.Pivot).transform;

            _sortieButton.onClick.AddListener(Sortie);
            _extractButton.onClick.AddListener(Extract);
        }

        internal void Show(AgentUnit unit)
        {
            _selectedData = unit.agentData;

            var tilePos = _camera.WorldToScreenPoint(_selectedData.mountTile.transform.position);
            _pivot.position = tilePos;

            if (_selectedData.agentTemplate.job.job == EJob.Melee)
            {
                _sortieButton.gameObject.SetActive(true);
                _deployAbility = _selectedData.agentUnit.GetAbility<DeployAbility>();
                if (_deployAbility.isSortie)
                {
                    _sortieText.text = "복귀하기";
                }
                else
                {
                    _sortieText.text = "출격하기";
                }
            }
            else
            {
                _sortieButton.gameObject.SetActive(false);
            }

            base.Show(true);
        }

        #region 출격
        private async void Sortie()
        {
            _allySelectCanvas.Hide();

            if (_deployAbility.isSortie)
            {
                // 복귀하기
                await _selectedData.agentUnit.GetAbility<DeployAbility>().ReturnSortie();
            }
            else
            {
                // 출격하기
                await _selectedData.agentUnit.GetAbility<DeployAbility>().StartSortie(_tileSystem.sortiePoint.position);
            }
        }
        #endregion

        #region 회수
        private void Extract()
        {
            // 표지판 반환
            if (_selectedData.signBoard != null)
            {
                _agentReturnSystem.ReturnSignBoard(_selectedData.signBoard.gameObject);
                _selectedData.DeregistSignBoard();
            }

            _selectedData.agentUnit.SetInteraction(false);

            _agentReturnSystem.ReturnUnit(_selectedData);
            _selectedData = null;
            _allySelectCanvas.Hide();
        }
        #endregion
    }
}