using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork.UIBinding;
using TMPro;
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
        #endregion

        private TextMeshProUGUI _counterText;
        private Image _counterImage;
        private Button _tierUpButton;

        private UIAgentTier[] _tierGroups;
        private UITierAdvantageItem[] _tierAdvantageItems;

        private ProfileSaveDataTemplate _profileData;
        private ProfileSaveData.Agent _owned;
        private UnityAction _reShow;

        protected override void Initialize()
        {
            _tierGroups = GetComponentsInChildren<UIAgentTier>();
            _tierAdvantageItems = GetComponentsInChildren<UITierAdvantageItem>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindButton(typeof(Buttons));

            _counterText = GetText((int)Texts.CounterText);
            _counterImage = GetImage((int)Images.CounterImage);
            _tierUpButton = GetButton((int)Buttons.TierUpButton);

            _tierUpButton.onClick.AddListener(TierUp);
        }

        internal void Show(ProfileSaveData.Agent owned, UnityAction reShow)
        {
            _owned = owned;
            _reShow = reShow;
            if (_profileData == null) _profileData = GameDataManager.Instance.profileSaveData;

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

            _tierGroups[0].Show(_owned.tier);
            _tierGroups[1].Show(_owned.tier + 1);

            int tier = owned.tier;
            int unitCount = owned.unitCount;
            int maxUnitCount = GameDataManager.Instance.profileSaveData.GetMaxUnitCountByTier(tier);

            _counterText.text = $"{unitCount}/{maxUnitCount}";
            _counterImage.fillAmount = (float)unitCount / maxUnitCount;

            for (int i = 0; i < _tierAdvantageItems.Length; i++)
            {
                _tierAdvantageItems[i].ShowItem(i < tier);
            }

            _tierUpButton.interactable = _profileData.GetTierUpAbleUnit(_owned.id);
        }

        private void TierUp()
        {
            if (_profileData.TierUpAgent(_owned.id))
            {
                _reShow?.Invoke();
            }
        }
    }
}