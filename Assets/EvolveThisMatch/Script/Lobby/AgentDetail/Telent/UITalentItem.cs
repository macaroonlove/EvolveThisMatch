using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITalentItem : UIBase
    {
        #region ¹ÙÀÎµù
        enum Toggles
        {
            TalentItem,
        }
        enum Texts
        {
            TelentText,
        }
        enum CanvasGroups
        {
            Empty,
        }
        #endregion

        private Toggle _toggle;
        private TextMeshProUGUI _talentText;
        private UIRarityTag _rarityTag;
        private CanvasGroupController _empty;

        private AgentTalentSystem _agentTalentSystem;

        private UnityAction<bool> _onLockChanged;

        protected override void Initialize()
        {
            _rarityTag = GetComponentInChildren<UIRarityTag>();
            _agentTalentSystem = BattleManager.Instance.GetSubSystem<AgentTalentSystem>();

            BindToggle(typeof(Toggles));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _toggle = GetToggle((int)Toggles.TalentItem);
            _talentText = GetText((int)Texts.TelentText);
            _empty = GetCanvasGroupController((int)CanvasGroups.Empty);

            _toggle.onValueChanged.AddListener(isOn => _onLockChanged?.Invoke(isOn));
        }

        internal void Show(TalentSlotState state, UnityAction<bool> onLockChanged)
        {
            _onLockChanged = onLockChanged;

            _toggle.isOn = state.isLocked;
            _empty.ShowOrHide(state.id == -1);

            if (state.id == -1) return;

            var type = (EEffectType)state.id;

            _talentText.text = _agentTalentSystem.GetTalentDescription(type, state.value);
            _rarityTag.Show(state.rarity);
        }
    }
}