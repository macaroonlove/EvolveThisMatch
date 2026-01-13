using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITalentFilterView : UIBase
    {
        #region 바인딩
        enum Objects
        {
            RarityConditionGroup,
            Content,
            TalentCondition,
        }
        enum Buttons
        {
            CloseButton,
            ConditionResetting,
        }
        enum Texts
        {
            ConditionResettingText,
        }
        enum CanvasGroups
        {
            Panel,
        }
        #endregion

        private Toggle[] _rarityToggles;
        private List<Toggle> _talentToggles = new List<Toggle>();
        private TextMeshProUGUI _buttonText;
        private CanvasGroupController _panel;

        private UnityAction<TalentFilterCondition> _onConfirm;
        private UnityAction _onCancel;

        #region 초기화
        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _rarityToggles = GetObject((int)Objects.RarityConditionGroup).GetComponentsInChildren<Toggle>();
            _buttonText = GetText((int)Texts.ConditionResettingText);
            _panel = GetCanvasGroupController((int)CanvasGroups.Panel);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Cancel);
            GetButton((int)Buttons.ConditionResetting).onClick.AddListener(Confirm);

            InitializeTalentToggle();
        }

        private void InitializeTalentToggle()
        {
            var agentTalentSystem = BattleManager.Instance.GetSubSystem<AgentTalentSystem>();

            var prefab = GetObject((int)Objects.TalentCondition);
            var parent = GetObject((int)Objects.Content).transform;

            for (int i = 0; i < agentTalentSystem.talentCount; i++)
            {
                var instance = Instantiate(prefab, parent);
                var toggle = instance.GetComponent<Toggle>();
                var text = instance.GetComponentInChildren<TextMeshProUGUI>();
                text.text = agentTalentSystem.GetTalentTitle((EEffectType)i);

                _talentToggles.Add(toggle);
            }

            Destroy(prefab);
        }
        #endregion

        internal void Show(string costText, UnityAction<TalentFilterCondition> onConfirm, UnityAction onCancel)
        {
            _buttonText.text = costText;
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            _panel.Show(true);
            Show(true);
        }

        #region 이벤트
        private void Confirm()
        {
            _onConfirm?.Invoke(new TalentFilterCondition
            {
                rarity = GetSelectedRarity(),
                talentIds = GetSelectedTalents()
            });

            Hide(true);
        }

        private void Cancel()
        {
            _onCancel?.Invoke();
            Hide(true);
        }
        #endregion

        #region 유틸리티
        /// <summary>
        /// 선택된 등급 찾기
        /// </summary>
        private int GetSelectedRarity()
        {
            for (int i = 0; i < _rarityToggles.Length; i++)
            {
                if (_rarityToggles[i].isOn) return i;
            }
            return -1;
        }

        /// <summary>
        /// 선택된 재능 찾기
        /// </summary>
        private List<int> GetSelectedTalents()
        {
            var result = new List<int>();
            for (int i = 0; i < _talentToggles.Count; i++)
            {
                if (_talentToggles[i].isOn)
                    result.Add(i);
            }
            return result;
        }
        #endregion
    }

    public struct TalentFilterCondition
    {
        public int rarity;
        public List<int> talentIds;
    }
}