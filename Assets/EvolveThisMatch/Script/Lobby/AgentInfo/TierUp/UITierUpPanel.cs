using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UITierUpPanel : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            CounterText,
        }
        enum Images
        {
            CounterImage,
        }
        enum Buttons
        {
            TierUpButton,
        }
        enum Objects
        {
            Arrow,
        }
        #endregion

        private TextMeshProUGUI _counterText;
        private Image _counterImage;
        private Button _tierUpButton;
        private GameObject _arrow;

        private UIAgentTier[] _tierGroups;
        private UITierAdvantageItem[] _tierAdvantageItems;

        private AgentSaveDataTemplate _agentData;
        private AgentSaveData.Agent _owned;
        private UnityAction _reShow;

        protected override void Initialize()
        {
            _tierGroups = GetComponentsInChildren<UIAgentTier>();
            _tierAdvantageItems = GetComponentsInChildren<UITierAdvantageItem>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));
            BindObject(typeof(Objects));

            _counterText = GetText((int)Texts.CounterText);
            _counterImage = GetImage((int)Images.CounterImage);
            _tierUpButton = GetButton((int)Buttons.TierUpButton);
            _arrow = GetObject((int)Objects.Arrow);

            _tierUpButton.onClick.AddListener(TierUp);
        }

        internal void Show(AgentSaveData.Agent owned, UnityAction reShow)
        {
            _owned = owned;
            _reShow = reShow;
            if (_agentData == null) _agentData = SaveManager.Instance.agentData;

            if (_owned == null)
            {
                _tierUpButton.interactable = false;
                _counterText.text = "0/1";
                _counterImage.fillAmount = 0;

                for (int i = 0; i < _tierAdvantageItems.Length; i++)
                {
                    _tierAdvantageItems[i].ShowItem(false);
                }
                return;
            }

            

            int tier = owned.tier;
            int unitCount = owned.unitCount;
            int maxUnitCount = SaveManager.Instance.agentData.GetMaxUnitCountByTier(tier);

            if (maxUnitCount == -1)
            {
                _counterText.text = $"{unitCount}";
                _counterImage.fillAmount = 1;

                _tierGroups[1].gameObject.SetActive(false);
                _arrow.SetActive(false);

                _tierGroups[0].Show(_owned.tier);
            }
            else
            {
                _counterText.text = $"{unitCount}/{maxUnitCount}";
                _counterImage.fillAmount = (float)unitCount / maxUnitCount;

                _tierGroups[1].gameObject.SetActive(true);
                _arrow.SetActive(true);

                _tierGroups[0].Show(_owned.tier);
                _tierGroups[1].Show(_owned.tier + 1);
            }
            
            for (int i = 0; i < _tierAdvantageItems.Length; i++)
            {
                _tierAdvantageItems[i].ShowItem(i < tier);
            }

            _tierUpButton.interactable = _agentData.GetTierUpAbleUnit(_owned.id);
        }

        private void TierUp()
        {
            if (_agentData.TierUpAgent(_owned.id))
            {
                _reShow?.Invoke();
            }
        }
    }
}