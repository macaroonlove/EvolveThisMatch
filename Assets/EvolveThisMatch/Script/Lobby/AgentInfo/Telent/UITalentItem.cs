using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = FrameWork.PlayFabExtensions.Random;

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

        private TalentSaveData.Talent _talent;
        private UnityAction<bool> _action;

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
            if (_talent == null) return;

            _talent.isLock = isOn;
            _action?.Invoke(isOn);
        }

        #region Show
        internal void Show(TalentSaveData.Talent talent, UnityAction<bool> action)
        {
            _talent = talent;
            _action = action;

            _toggle.isOn = talent.isLock;

            bool isEmpty = talent.id == -1;
            _empty.ShowOrHide(isEmpty);

            if (isEmpty) return;

            var data = AgentSaveDataTemplate.talentTitleData.talentData[talent.id];
            var effect = GameDataManager.Instance.talentEffects[talent.id];

            if (effect is IRuntimeDataEffect runtimeDataEffect)
            {
                runtimeDataEffect.SetValue(talent.value);
                _talentText.text = effect.GetDescription();
                _rarityTag.Show(GetRarity(data, talent.value));
            }
        }

        private AgentRarityTemplate GetRarity(TalentConfig data, int value)
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            if (value > data.mythLimit) return raritys[0];
            else if (value > data.legendLimit) return raritys[1];
            else if (value > data.epicLimit) return raritys[2];
            else if (value > data.rareLimit) return raritys[3];

            return raritys[4];
        }
        #endregion

        #region 재설정
        internal bool Resetting(Random rng, float[] rarityProbabilities, int rarity = -1, List<int> talents = null)
        {
            // 재능 종류 받아오기
            var talentData = AgentSaveDataTemplate.talentTitleData.talentData;
            int id = rng.Next(0, talentData.Count - 1);
            var data = talentData[id];
            var effect = GameDataManager.Instance.talentEffects[id];

            if (effect is IRuntimeDataEffect runtimeDataEffect)
            {
                // 재능의 등급 받아오기
                var newRarity = GetRandomRarity(rng, rarityProbabilities);
                _rarityTag.Show(newRarity);

                // 재능에 맞는 값 적용하기
                int newValue = newRarity.rarity switch
                {
                    EAgentRarity.Myth => rng.Next(data.mythLimit, data.maxValue),
                    EAgentRarity.Legend => rng.Next(data.legendLimit, data.mythLimit),
                    EAgentRarity.Epic => rng.Next(data.epicLimit, data.legendLimit),
                    EAgentRarity.Rare => rng.Next(data.rareLimit, data.epicLimit),
                    _ => rng.Next(data.minValue, data.rareLimit)
                };

                runtimeDataEffect.SetValue(newValue);
                _talentText.text = effect.GetDescription();

                // SaveData에 적용
                _talent.id = id;
                _talent.value = newValue;

                // 값이 설정되었으므로 재능이 없을 경우 보이는 UI 비가시화
                _empty.Hide();
            }

            bool isSuccess = rarity >= (int)GetRarity(data, _talent.value).rarity && talents.Contains(_talent.id);

            return isSuccess;
        }

        private AgentRarityTemplate GetRandomRarity(Random rng, float[] rarityProbabilities)
        {
            var raritys = GameDataManager.Instance.agentRarityTemplates;

            int rand = rng.Next(0, 100);
            float cumulativeProbability = 0;

            for (int i = 0; i < rarityProbabilities.Length; i++)
            {
                cumulativeProbability += rarityProbabilities[i];
                if (rand <= cumulativeProbability)
                {
                    return raritys[i];
                }
            }

            return raritys[rarityProbabilities.Length - 1];
        }
        #endregion
    }
}