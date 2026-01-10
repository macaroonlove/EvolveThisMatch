using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDepartmentDisposeItem : UIBase, IPointerClickHandler
    {
        #region 바인딩
        enum Images
        {
            AgentBG,
            CraftBG,
            FullBody,
            CraftIcon,
            Slider,
        }
        enum Texts
        {
            CraftName,
            SpeedText,
            WeightText,
            ProductionCount,
            WaitCount,
            RemainTime,
            SliderText,
            LockText,
        }
        enum CanvasGroups
        {
            Lock,
        }
        enum Buttons
        {
            GainButton,
            RemoveJobButton,
        }
        #endregion

        private Image _agentBG;
        private Image _craftBG;
        private Image _fullBody;
        private Image _craftIcon;
        private Image _slider;
        private TextMeshProUGUI _craftName;
        private TextMeshProUGUI _speedText;
        private TextMeshProUGUI _weightText;
        private TextMeshProUGUI _productionCount;
        private TextMeshProUGUI _waitCount;
        private TextMeshProUGUI _remainTimeText;
        private TextMeshProUGUI _sliderText;
        private TextMeshProUGUI _lockText;
        private CanvasGroupController _lock;

        private int _slotIndex;
        private bool _isLock;

        private UnityAction<int> _onOpenDepartmentDisposeRegistView;
        private UnityAction<int> _onGainItem;
        private UnityAction<int> _onRemoveJob;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));
            BindButton(typeof(Buttons));

            _agentBG = GetImage((int)Images.AgentBG);
            _craftBG = GetImage((int)Images.CraftBG);
            _fullBody = GetImage((int)Images.FullBody);
            _craftIcon = GetImage((int)Images.CraftIcon);
            _slider = GetImage((int)Images.Slider);
            _craftName = GetText((int)Texts.CraftName);
            _speedText = GetText((int)Texts.SpeedText);
            _weightText = GetText((int)Texts.WeightText);
            _productionCount = GetText((int)Texts.ProductionCount);
            _waitCount = GetText((int)Texts.WaitCount);
            _remainTimeText = GetText((int)Texts.RemainTime);
            _sliderText = GetText((int)Texts.SliderText);
            _lockText = GetText((int)Texts.LockText);
            _lock = GetCanvasGroupController((int)CanvasGroups.Lock);

            // 아이템 획득
            GetButton((int)Buttons.GainButton).onClick.AddListener(() => _onGainItem?.Invoke(_slotIndex));

            // 작업대 비우기
            GetButton((int)Buttons.RemoveJobButton).onClick.AddListener(() => _onRemoveJob?.Invoke(_slotIndex));
        }

        public void Initailize(int slotIndex, UnityAction<int> onOpenDepartmentDisposeRegistView, UnityAction<int> onGainItem, UnityAction<int> onRemoveJob)
        {
            _slotIndex = slotIndex;
            _onOpenDepartmentDisposeRegistView = onOpenDepartmentDisposeRegistView;
            _onGainItem = onGainItem;
            _onRemoveJob = onRemoveJob;
        }

        public void Lock(int unlockLevel)
        {
            _isLock = true;
            _lock.Show(true);
            _lockText.text = $"Lv. {unlockLevel}에\n해금됩니다.";
        }

        public void Render(DepartmentDisposeItemViewState state)
        {
            if (state.IsEmpty)
            {
                RenderEmpty();
                return;
            }

            RenderWorking(state);
        }

        private void RenderEmpty()
        {
            _agentBG.color = Color.black;
            _craftBG.color = Color.black;

            _fullBody.enabled = false;
            _craftIcon.enabled = false;

            _slider.fillAmount = 0f;

            _craftName.text = "생산품을";
            _speedText.text = "설정해주세요.";
            _weightText.text = "";
            _productionCount.text = "";
            _waitCount.text = "";
            _remainTimeText.text = "";
            _sliderText.text = "0%";

            _isLock = false;
            _lock.Hide(true);
        }

        private void RenderWorking(DepartmentDisposeItemViewState state)
        {
            var craftItem = state.craftItem;
            var variable = SaveManager.Instance.profileData.GetVariable(craftItem.Variable);
            var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(state.agentId);

            _agentBG.color = Color.white;
            _agentBG.sprite = agentTemplate.rarity.agentInfoSprite;
            _craftBG.color = Color.white;
            _craftBG.sprite = variable.IconBG;

            _fullBody.enabled = true;
            _fullBody.sprite = agentTemplate.sprite;
            _craftIcon.enabled = true;
            _craftIcon.sprite = variable.Icon;

            _craftName.text = variable.DisplayName;
            _speedText.text = $"속도  <color=white>{state.craftSpeed * 100}%</color>";
            _weightText.text = $"무게  <color=white>{craftItem.Weight}kg</color>";

            _productionCount.text = $"생산  <color=white>{state.craftCount}개</color>";
            _waitCount.text = $"대기  <color=white>{state.remainingCount}개</color>";

            _slider.fillAmount = state.progress;
            _sliderText.text = $"{state.progress * 100f:F0}%";

            if (state.remainingCount <= 0)
                _remainTimeText.text = "생산 완료";
            else
                _remainTimeText.text = $"남은 시간\n<color=white>{state.remainTime.Minutes}분 {state.remainTime.Seconds}초</color>";

            _isLock = false;
            _lock.Hide(true);
        }

        public void RenderFullStorage()
        {
            _remainTimeText.text = "보관 창고가\n가득찼습니다.";
            _slider.fillAmount = 0f;
            _sliderText.text = "0%";
        }

        public void RenderLackRequiredItem()
        {
            _remainTimeText.text = "재료가\n부족합니다.";
            _slider.fillAmount = 0f;
            _sliderText.text = "0%";
        }

        /// <summary>
        /// 배치 설정 창 열기
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isLock)
            {
                _onOpenDepartmentDisposeRegistView?.Invoke(_slotIndex);
            }
        }
    }

    public readonly struct DepartmentDisposeItemViewState
    {
        public readonly int slotIndex;
        public readonly bool isUnlocked;
        public readonly int unlockLevel;

        public readonly DepartmentCraftData craftItem;
        public readonly int agentId;
        public readonly float craftSpeed;
        public readonly int itemWeight;
        public readonly int craftCount;
        public readonly int remainingCount;

        public readonly float progress;
        public readonly TimeSpan remainTime;

        public bool IsEmpty => craftItem == null;

        public DepartmentDisposeItemViewState(int slotIndex, bool isUnlocked, int unlockLevel, DepartmentCraftData craftItem, int agentId, float craftSpeed, int itemWeight, int craftCount, int remainingCount, float progress, TimeSpan remainTime)
        {
            this.slotIndex = slotIndex;
            this.isUnlocked = isUnlocked;
            this.unlockLevel = unlockLevel;
            this.craftItem = craftItem;
            this.agentId = agentId;
            this.craftSpeed = craftSpeed;
            this.itemWeight = itemWeight;
            this.craftCount = craftCount;
            this.remainingCount = remainingCount;
            this.progress = progress;
            this.remainTime = remainTime;
        }
    }
}