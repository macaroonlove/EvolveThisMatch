using Cysharp.Threading.Tasks;
using FrameWork.UIBinding;
using System.Collections;
using System.Collections.Generic;
using EvolveThisMatch.Save;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UIFormationCanvas : UIBase
    {
        #region 바인딩
        enum Buttons
        {
            CloseButton,
            StartButton,
            BundleFormationButton,
        }
        #endregion

        private Button _startButton;

        private UIFormationAgentSelectCanvas _agentSelectCanvas;
        private UIFormationSlot[] _uiFormationSlots;

        private int _formationMaxCount;

        protected override void Initialize()
        {
            _agentSelectCanvas = GetComponentInChildren<UIFormationAgentSelectCanvas>();

            BindButton(typeof(Buttons));

            _startButton = GetButton((int)Buttons.StartButton);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            _startButton.onClick.AddListener(GameStart);
            GetButton((int)Buttons.BundleFormationButton).onClick.AddListener(BundleFormation);

            _uiFormationSlots = GetComponentsInChildren<UIFormationSlot>();
        }

        [ContextMenu("초기화")]
        public async void Show()
        {
            var formationSaveData = await GameDataManager.Instance.GetFormationSaveData();

            var formationSlots = formationSaveData.formation;
            int formationCount = formationSlots.Count;

            // TODO: 맵 데이터와 같은 곳에서 최대 배치 수를 받아오기
            _formationMaxCount = 10;

            for (int i = 0; i < _uiFormationSlots.Length; i++)
            {
                int index = i;
                // 최대 배치 수를 넘어간다면
                if (index >= _formationMaxCount)
                {
                    _uiFormationSlots[index].Lock();
                }
                // 해당 자리에 유닛이 존재한다면
                else if (index < formationCount)
                {
                    var slot = formationSlots[index];
                    var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(slot.id);

                    _uiFormationSlots[index].Show(agentTemplate, () => { ShowSelectCanvas(index); });
                }
                // 해당 자리가 비어있다면
                else
                {
                    _uiFormationSlots[index].Show(null, () => { ShowSelectCanvas(index); });
                }
            }

            _startButton.interactable = true;

            base.Show();
        }

        private void ShowSelectCanvas(int index)
        {
            index = GetFinalIndex(index);

            _agentSelectCanvas.Show(index, _uiFormationSlots, (template) => { ChangeSlot(index, template); });
        }

        private void ChangeSlot(int index, AgentTemplate template)
        {
            _uiFormationSlots[index].Change(template);

            if (template == null)
            {
                // 슬롯 당기기
                for (int i = index + 1; i < _uiFormationSlots.Length; i++)
                {
                    var nextTemplate = _uiFormationSlots[i].template;
                    if (nextTemplate != null)
                    {
                        _uiFormationSlots[i - 1].Change(nextTemplate);
                        _uiFormationSlots[i].Change(null);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void BundleFormation()
        {
            _agentSelectCanvas.Show(_formationMaxCount, _uiFormationSlots, (templates) => { ChangeBunddle(templates); });
        }

        private void ChangeBunddle(List<AgentTemplate> templates)
        {
            for (int i = 0; i < _uiFormationSlots.Length; i++)
            {
                if (i < templates.Count)
                {
                    _uiFormationSlots[i].Change(templates[i]);
                }
                else
                {
                    _uiFormationSlots[i].Clear();
                }
            }
        }

        private async void GameStart()
        {
            _startButton.interactable = false;

            // 전투에 사용될 유닛 어드레서블 로드
            List<UniTask> tasks = new List<UniTask>();

            foreach (var slot in _uiFormationSlots) 
            {
                if (slot.template != null)
                {
                    var task = slot.template.LoadSkinBattleTemplate();
                    tasks.Add(task);
                }
            }

            await UniTask.WhenAll(tasks);

            // 전투 시작
            BattleManager.Instance.InitializeBattle();
            BattleManager.Instance.onBattleDeinitialize += OnBattleDeinitialize;

            // 배치 저장
            List<FormationSlot> formation = new List<FormationSlot>();

            foreach (var slot in _uiFormationSlots)
            {
                if (slot.template != null)
                {
                    var newFormationSlot = new FormationSlot();
                    newFormationSlot.id = slot.template.id;
                    
                    formation.Add(newFormationSlot);
                }
            }

            GameDataManager.Instance.SetFormationSaveData(formation);

            Hide();
        }

        private void OnBattleDeinitialize()
        {
            // 전투 종료시, 어드레서블 해제
            foreach (var slot in _uiFormationSlots)
            {
                if (slot.template != null)
                {
                    slot.template.ReleaseSkinBattleTemplate();
                }
            }
        }

        private void Hide()
        {
            base.Hide();
        }

        private int GetFinalIndex(int index)
        {
            // 현재 인덱스가 비어있다면 가장 앞쪽의 인덱스를 선택
            if (_uiFormationSlots[index].isEmpty)
            {
                for (int i = 0; i < _uiFormationSlots.Length; i++)
                {
                    if (_uiFormationSlots[i].isEmpty)
                    {
                        return i;
                    }
                }
            }

            // 현재 인덱스 반환
            return index;
        }
    }
}