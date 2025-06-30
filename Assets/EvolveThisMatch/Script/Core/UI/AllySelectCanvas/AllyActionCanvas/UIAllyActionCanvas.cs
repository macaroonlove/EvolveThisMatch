using Cysharp.Threading.Tasks;
using FrameWork.UIBinding;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
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

        [SerializeField] private FX _sortieStart;
        [SerializeField] private FX _sortieEnd;

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
                await ReturnSortie();
            }
            else
            {
                // 출격하기
                await StartSortie();
            }
        }

        private async UniTask StartSortie()
        {
            _sortieStart.Play(_selectedData.agentUnit);

            await UniTask.Delay(500);

            SetAgentPosition(_tileSystem.sortiePoint.position);

            _sortieEnd.Play(_selectedData.agentUnit);

            await UniTask.Delay(1500);

            _deployAbility.Sortie(true);
        }

        private async UniTask ReturnSortie()
        {
            _deployAbility.Sortie(false);

            _sortieEnd.Play(_selectedData.agentUnit);

            await UniTask.Delay(500);

            SetAgentPosition(_selectedData.mountTile.transform.position);

            _sortieStart.Play(_selectedData.agentUnit);
        }

        private void SetAgentPosition(Vector3 targetPosition)
        {
            var agentTransform = _selectedData.agentUnit.transform;
            agentTransform.position = targetPosition;

            var pos = agentTransform.position;
            pos.z = pos.y;
            agentTransform.position = pos;

            agentTransform.GetChild(3).localScale = Vector3.one * 0.15f;
        }
        #endregion

        #region 회수
        private void Extract()
        {
            _agentReturnSystem.ReturnUnit(_selectedData);
            _selectedData = null;
            _allySelectCanvas.Hide();
        }
        #endregion
    }
}