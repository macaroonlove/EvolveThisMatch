using Cysharp.Threading.Tasks;
using DG.Tweening;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using FrameWork.UIPopup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIFormationCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            ClearButton,
        }
        enum Toggles
        {
            BundleBatchToggle,
            ControlToggle,
        }
        enum Objects
        {
            Background,
        }
        #endregion

        private TileSystem _tileSystem;
        private TileRayCastSystem _tileRayCastSystem;
        private AgentCreateSystem _agentCreateSystem;
        private AgentReturnSystem _agentReturnSystem;
        private UIAgentListCanvas_Formation _agentListCanvas;

        private RectTransform _background;
        private Toggle _bundleBatchToggle;
        private Toggle _controlToggle;

        private AgentTemplate _selectedAgent;
        private Vector2 _initialPos;
        private UnityAction _onClose;

        protected override void Initialize()
        {
            _tileSystem = BattleManager.Instance.GetSubSystem<TileSystem>();
            _tileRayCastSystem = BattleManager.Instance.GetSubSystem<TileRayCastSystem>();
            _agentCreateSystem = BattleManager.Instance.GetSubSystem<AgentCreateSystem>();
            _agentReturnSystem = BattleManager.Instance.GetSubSystem<AgentReturnSystem>();

            _agentListCanvas = GetComponentInChildren<UIAgentListCanvas_Formation>();
            _agentListCanvas.Initialize(SelectAgent);

            BindButton(typeof(Buttons));
            BindToggle(typeof(Toggles));
            BindObject(typeof(Objects));

            _background = GetObject((int)Objects.Background).transform as RectTransform;
            _bundleBatchToggle = GetToggle((int)Toggles.BundleBatchToggle);
            _controlToggle = GetToggle((int)Toggles.ControlToggle);

            _initialPos = _background.anchoredPosition;

            _bundleBatchToggle.onValueChanged.AddListener(ResetSelectedAgent);
            _controlToggle.onValueChanged.AddListener(ResetSelectedAgent);
            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.ClearButton).onClick.AddListener(ClearButton);

            BattleManager.Instance.onBattleInitialize += InitializeFormation;
        }

        private async void InitializeFormation()
        {
            Clear();

            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            var formationSaveData = SaveManager.Instance.formationData;
            var formationSlots = formationSaveData.formation;
            
            if (formationSlots == null) return;

            int formationCount = formationSlots.Count;

            for (int i = 0; i < formationCount; i++)
            {
                int index = i;

                // 해당 자리에 유닛이 존재한다면
                if (index < formationCount)
                {
                    var slot = formationSlots[index];
                    
                    // 빈 공간이라면 넘어가기
                    if (slot.id == -1) continue;

                    var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(slot.id);
                    var tileController = _tileSystem.GetTile(index);

                    // 유닛 배치
                    _agentCreateSystem.CreateFixedUnit(agentTemplate, tileController);

                    // 리스트에서 제외
                    _agentListCanvas.HideItem(agentTemplate);
                }
            }
        }

        public void Show(UnityAction onClose)
        {
            _onClose = onClose;

            // 시작 위치를 화면 오른쪽 바깥으로 이동
            _background.anchoredPosition = new Vector2(_initialPos.x + _background.rect.width, _initialPos.y);

            // 보이기
            _background.DOAnchorPos(_initialPos, 0.5f).SetEase(Ease.OutCubic);

            _tileSystem.VisibleRenderer(true);

            _tileRayCastSystem.onCast += OnCastTile;

            base.Show(true);
        }

        public void Hide()
        {
            _tileSystem.VisibleRenderer(false);

            _tileRayCastSystem.onCast -= OnCastTile;

            // 최종 위치 저장
            Vector2 targetPos = new Vector2(_initialPos.x + _background.rect.width, _initialPos.y);

            // 숨기기
            _background.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                base.Hide(false);

                _onClose?.Invoke();
            });
        }

        private void SelectAgent(AgentTemplate template, AgentSaveData.Agent owned)
        {
            // 일괄 배치
            if (_bundleBatchToggle.isOn)
            {
                BundleTile(template);
            }
            // 선택 배치
            else
            {
                _selectedAgent = template;
            }
        }

        private void ResetSelectedAgent(bool isOn)
        {
            _selectedAgent = null;
            _agentListCanvas.SelectedClear();
        }

        private void OnCastTile(TileController tileController)
        {
            // 유닛 반환
            if (_controlToggle.isOn)
            {
                ReturnUnit(tileController);

                return;
            }

            if (_selectedAgent != null)
            {
                // 유닛이 배치되어 있다면 리턴
                ReturnUnit(tileController);

                // 유닛 배치
                _agentCreateSystem.CreateFixedUnit(_selectedAgent, tileController);

                RegistFormation();

                // 리스트에서 제외
                _agentListCanvas.HideItem(_selectedAgent);

                _selectedAgent = null;
            }
        }

        private void BundleTile(AgentTemplate template)
        {
            _agentCreateSystem.CreateFixedUnit(template);

            RegistFormation();

            // 리스트에서 제외
            _agentListCanvas.HideItem(template);
        }

        private void ReturnUnit(TileController tileController)
        {
            if (tileController.isPlaceUnit && tileController.placedAgentData.agentTemplate != null)
            {
                // 리스트에 추가
                _agentListCanvas.ShowItem(tileController.placedAgentData.agentTemplate);

                _agentReturnSystem.ReturnUnit(tileController.placedAgentData, false);

                RegistFormation();
            }
        }

        private void RegistFormation()
        {
            // 배치 저장
            List<FormationSlot> formation = new List<FormationSlot>();

            var datas = _tileSystem.GetPlacedAgentDatas();

            foreach (var data in datas)
            {
                var newFormationSlot = new FormationSlot();

                if (data != null)
                {
                    newFormationSlot.id = data.agentTemplate.id;
                }
                else
                {
                    newFormationSlot.id = -1;
                }

                formation.Add(newFormationSlot);
            }

            SaveManager.Instance.formationData.UpdateFormation(formation);
            SaveManager.Instance.Save_FormationData();
        }

        private void ClearButton()
        {
            UIPopupManager.Instance.ShowConfirmCancelPopup("정말 초기화 하시겠습니까?", (isConfirm) =>
            {
                if (isConfirm)
                {
                    Clear();

                    SaveManager.Instance.formationData.UpdateFormation(null);
                    SaveManager.Instance.Save_FormationData();
                }
            });
        }

        private void Clear()
        {
            var datas = _tileSystem.GetPlacedAgentDatas();

            foreach (var data in datas)
            {
                if (data != null)
                {
                    // 리스트에 추가
                    _agentListCanvas.ShowItem(data.agentTemplate);
                    
                    _agentReturnSystem.ReturnUnit(data, false);
                }
            }

            
        }
    }
}