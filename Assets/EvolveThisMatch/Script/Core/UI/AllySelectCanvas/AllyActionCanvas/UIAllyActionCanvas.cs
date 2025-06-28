using Cysharp.Threading.Tasks;
using FrameWork.UIBinding;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UIAllyActionCanvas : UIBase
    {
        #region ¹ÙÀÎµù
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
        private Button _sortieButton;
        private Button _extractButton;

        private UIAllySelectCanvas _allySelectCanvas;
        private TileSystem _tileSystem;
        private Camera _camera;
        private AgentBattleData _selectedData;

        protected override void Initialize()
        {
            _camera = Camera.main;
            _allySelectCanvas = GetComponentInParent<UIAllySelectCanvas>();
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();

            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

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
            }
            else
            {
                _sortieButton.gameObject.SetActive(false);
            }

            base.Show(true);
        }

        private async void Sortie()
        {
            _allySelectCanvas.Hide();
            _sortieStart.Play(_selectedData.agentUnit);
            await UniTask.Delay(500);
            _selectedData.agentUnit.transform.position = _tileSystem.sortiePoint.position;
            _sortieEnd.Play(_selectedData.agentUnit);
            await UniTask.Delay(1500);
            _selectedData.agentUnit.GetAbility<DeployAbility>().Sortie();
        }

        private void Extract()
        {

        }
    }
}
