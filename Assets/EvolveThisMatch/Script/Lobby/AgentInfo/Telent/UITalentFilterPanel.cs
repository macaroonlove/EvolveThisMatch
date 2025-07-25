using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITalentFilterPanel : UIBase
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
        private UITalentPanel _talentPanel;
        private TextMeshProUGUI _buttonText;
        private CanvasGroupController _panel;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _rarityToggles = GetObject((int)Objects.RarityConditionGroup).GetComponentsInChildren<Toggle>();
            _buttonText = GetText((int)Texts.ConditionResettingText);
            _panel = GetCanvasGroupController((int)CanvasGroups.Panel);

            GetButton((int)Buttons.CloseButton).onClick.AddListener(Hide);
            GetButton((int)Buttons.ConditionResetting).onClick.AddListener(ConditionResetting);
            InitializeTalentToggle();
        }

        private async void InitializeTalentToggle()
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            var talentDatas = GameDataManager.Instance.GetAllTalentEffect();

            var prefab = GetObject((int)Objects.TalentCondition);
            var parent = GetObject((int)Objects.Content).transform;

            foreach (var data in talentDatas)
            {
                if (data.effect is not IRuntimeDataEffect runtimeDataEffect) continue;

                var instance = Instantiate(prefab, parent);
                var toggle = instance.GetComponent<Toggle>();
                var text = instance.GetComponentInChildren<TextMeshProUGUI>();
                text.text = runtimeDataEffect.GetTitle();

                _talentToggles.Add(toggle);
            }

            Destroy(prefab);
        }

        internal void Initialize(UITalentPanel talentPanel)
        {
            _talentPanel = talentPanel;
        }

        internal void Show()
        {
            _buttonText.text = _talentPanel.GetChangeTalentText();

            _panel.Show(true);
            Show(true);
        }

        private void ConditionResetting()
        {
            _talentPanel?.ChangeTalent(GetActiveRarityToggle(), GetActiveTalentToggle());

            _panel.Hide(true);
        }

        private void Hide()
        {
            _talentPanel?.StopFilter();

            Hide(true);
        }

        /// <summary>
        /// 활성화된 등급 토글 찾기
        /// </summary>
        private int GetActiveRarityToggle()
        {
            for (int i = 0; i < _rarityToggles.Length; i++)
            {
                if (_rarityToggles[i].isOn) return i;
            }

            return -1;
        }

        public List<int> GetActiveTalentToggle()
        {
            var selectedList = new List<int>();

            for (int i = 0; i < _talentToggles.Count; i++)
            {
                if (_talentToggles[i].isOn) selectedList.Add(i);
            }

            return selectedList;
        }
    }
}