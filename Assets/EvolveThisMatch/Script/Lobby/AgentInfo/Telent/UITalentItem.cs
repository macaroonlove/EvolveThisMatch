using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITalentItem : UIBase
    {
        #region 바인딩
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
        private UnityAction _action;

        internal AgentSaveData.Talent talent { get; private set; }
        internal bool isLock => _toggle.isOn;

        protected override void Initialize()
        {
            _rarityTag = GetComponentInChildren<UIRarityTag>();

            BindToggle(typeof(Toggles));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _toggle = GetToggle((int)Toggles.TalentItem);
            _talentText = GetText((int)Texts.TelentText);
            _empty = GetCanvasGroupController((int)CanvasGroups.Empty);

            _toggle.onValueChanged.AddListener(ChangeLockState);
        }

        private void ChangeLockState(bool isOn)
        {
            if (talent == null) return;

            talent.isLock = isOn;
            _action?.Invoke();

            _ = SaveManager.Instance.SaveData(SaveKey.Agent);
        }

        internal void Show(AgentSaveData.Talent talent, UnityAction action)
        {
            this.talent = talent;
            _action = action;

            _toggle.isOn = talent.isLock;

            bool isEmpty = talent.id == -1;
            _empty.ShowOrHide(isEmpty);

            if (isEmpty) return;

            var data = GameDataManager.Instance.GetTalentEffect(talent.id);

            if (data.effect is IRuntimeDataEffect runtimeDataEffect)
            {
                runtimeDataEffect.SetValue(talent.value);
                _talentText.text = data.effect.GetDescription();
                _rarityTag.Show(data.GetRarity(talent.value));
            }
        }

        internal AgentTalentData Resetting()
        {
            // 재능 종류 받아오기
            var data = GameDataManager.Instance.GetRandomTalentEffect();

            if (data.effect is IRuntimeDataEffect runtimeDataEffect)
            {
                // 재능의 등급 받아오기
                var rarity = GameDataManager.Instance.GetRandomRarity();
                _rarityTag.Show(rarity);

                // 재능에 맞는 값 적용하기
                int newValue = rarity.rarity switch
                {
                    EAgentRarity.Myth => Random.Range(data.mythLimit, data.maxValue),
                    EAgentRarity.Legend => Random.Range(data.legendLimit, data.mythLimit),
                    EAgentRarity.Epic => Random.Range(data.epicLimit, data.legendLimit),
                    EAgentRarity.Rare => Random.Range(data.rareLimit, data.epicLimit),
                    _ => Random.Range(data.minValue, data.rareLimit)
                };

                runtimeDataEffect.SetValue(newValue);
                _talentText.text = data.effect.GetDescription();

                // SaveData에 적용
                talent.id = data.id;
                talent.value = newValue;

                // 값이 설정되었으므로 재능이 없을 경우 보이는 UI 비가시화
                _empty.Hide();
            }

            return data;
        }
    }
}