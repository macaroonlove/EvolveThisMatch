using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposeItem : UIBase, IPointerClickHandler
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

        private int _prevProductionCount;
        private int _checkProductionCount;
        private float _remainTime;
        private bool _isFirst;

        private UnityAction _showDisposeSettingPanel;
        private UnityAction<int> _updateInfoPanel;
        private UnityAction<int, float> _gainCraftItem;
        private UnityAction _removeJob;

        internal bool isInitialize { get; private set; }

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

            GetButton((int)Buttons.GainButton).onClick.AddListener(GainCraftItem);
            GetButton((int)Buttons.RemoveJobButton).onClick.AddListener(RemoveJob);

            _prevProductionCount = -1;
            _isFirst = true;
        }

        /// <summary>
        /// 해당 작업대가 해금되지 않았을 때 실행
        /// </summary>
        internal void Lock(int unLockIndex)
        {
            _showDisposeSettingPanel = null;
            _updateInfoPanel = null;

            // 잠금 이미지 켜기
            _lock.Show(true);

            _lockText.text = $"Lv. {unLockIndex}에\n해금됩니다.";

            isInitialize = false;
        }

        /// <summary>
        /// 해당 작업대가 비어있을 때 초기화
        /// </summary>
        internal void Initialize(UnityAction showDisposeSettingPanel)
        {
            _showDisposeSettingPanel = showDisposeSettingPanel;

            // 잠금 이미지 끄기
            _lock.Hide(true);

            _agentBG.color = Color.black;
            _craftBG.color = Color.black;
            _fullBody.enabled = false;
            _craftIcon.enabled = false;
            _slider.fillAmount = 0;
            _craftName.text = "생산품을";
            _speedText.text = "설정해주세요.";
            _weightText.text = "";
            _productionCount.text = "";
            _waitCount.text = "";
            _remainTimeText.text = "";
            _sliderText.text = "0%";

            isInitialize = false;
        }

        /// <summary>
        /// 해당 작업대에 작업이 있었을 때 초기화
        /// </summary>
        /// <param name="levelData">부서 정보</param>
        /// <param name="job">작업대 사용정보</param>
        /// <param name="craftItem">생산중이던 아이템</param>
        /// <param name="craftResult">현재까지 생산 정보</param>
        internal void Initialize(DepartmentSaveData.CraftingJob job, CraftItemData craftItem, CraftResult craftResult, float craftSpeed, UnityAction showDisposeSettingPanel, UnityAction<int> updateInfoPanel, UnityAction<int, float> gainCraftItem, UnityAction removeJob)
        {
            _showDisposeSettingPanel = showDisposeSettingPanel;

            // 잠금 이미지 끄기
            _lock.Hide(true);

            _showDisposeSettingPanel = showDisposeSettingPanel;
            _updateInfoPanel = updateInfoPanel;
            _gainCraftItem = gainCraftItem;
            _removeJob = removeJob;

            var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(job.chargeUnitId);

            // 생산 개수
            int productionCount = craftResult.productionCount;

            // 대기 개수
            int waitCount = job.maxAmount - productionCount;

            _checkProductionCount = -1;

            _agentBG.color = Color.white;
            _agentBG.sprite = agentTemplate.rarity.agentInfoSprite;
            _craftBG.color = Color.white;
            _craftBG.sprite = craftItem.background;
            _fullBody.enabled = true;
            _fullBody.sprite = agentTemplate.sprite;
            _fullBody.rectTransform.anchoredPosition = agentTemplate.faceCenterPosition + new Vector2(0, -40);
            _craftIcon.enabled = true;
            _craftIcon.sprite = craftItem.variable.Icon;
            _craftName.text = craftItem.variable.DisplayName;
            _speedText.text = $"속도  <color=white>{craftSpeed * 100}%</color>";
            _weightText.text = $"무게  <color=white>{craftItem.weight}kg</color>";
            _productionCount.text = $"생산  <color=white>{productionCount}개</color>";
            _waitCount.text = $"대기  <color=white>{waitCount}개</color>";

            isInitialize = true;
        }

        internal void FullStorageWeight()
        {
            _remainTimeText.text = "보관 창고가\n가득찼습니다.";
            _slider.fillAmount = 0f;
            _sliderText.text = "0%";
        }

        internal void LackRequiredItem()
        {
            _remainTimeText.text = "재료가\n부족합니다.";
            _slider.fillAmount = 0f;
            _sliderText.text = "0%";
        }

        internal bool UpdateItem(DepartmentLevelData levelData, CraftItemData craftItem, DepartmentSaveData.CraftingJob job, float timePerItem)
        {
            // 경과 시간
            TimeSpan elapsed = DateTime.UtcNow - job.startTime;
            float second = (float)elapsed.TotalSeconds;

            // 최대 생산량
            int maxAmount = job.maxAmount;

            // 생산 개수
            int productionCount = Mathf.Min(maxAmount, Mathf.FloorToInt(second / timePerItem));

            // 생산 개수 갱신
            if (productionCount != _prevProductionCount)
            {
                // 대기 개수
                int waitCount = maxAmount - productionCount;

                _productionCount.text = $"생산  <color=white>{productionCount}개</color>";
                _waitCount.text = $"대기  <color=white>{waitCount}개</color>";

                _prevProductionCount = productionCount;

                if (_isFirst == false)
                {
                    _updateInfoPanel?.Invoke(craftItem.weight);
                }

                _isFirst = false;
            }

            if (_checkProductionCount != productionCount)
            {
                // 무게 초과로 인한 생산 중단
                int totalWeight = productionCount * craftItem.weight;
                if (totalWeight >= levelData.storageWeight)
                {
                    FullStorageWeight();

                    _updateInfoPanel?.Invoke(craftItem.weight);
                    return false;
                }

                // 필요한 재료가 충분한지 검사
                foreach (var required in craftItem.requiredItems)
                {
                    // 충분하지 않다면
                    if (required.item.Value < required.amount)
                    {
                        LackRequiredItem();

                        return false;
                    }
                }

                _checkProductionCount = productionCount;
            }

            // 생산 완료
            if (productionCount >= maxAmount)
            {
                _remainTimeText.text = "생산 완료";
                _slider.fillAmount = 1f;
                _sliderText.text = "100%";
                return false;
            }
            else
            {
                // 남은 시간 계산
                _remainTime = Mathf.Clamp((productionCount + 1) * timePerItem - second, 0, timePerItem);
                int remainMinute = Mathf.FloorToInt(_remainTime / 60f);
                int remainSecond = Mathf.FloorToInt(_remainTime % 60f);

                // 진행도 계산
                float currentItemElapsed = second - (productionCount * timePerItem);
                float progress = Mathf.Clamp01(currentItemElapsed / timePerItem);

                _remainTimeText.text = $"남은 시간\n<color=white>{remainMinute}분 {remainSecond}초</color>";
                _slider.fillAmount = progress;
                _sliderText.text = $"{progress * 100f:F0}%";
            }

            return true;
        }

        /// <summary>
        /// 배치 설정창 열기
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            _showDisposeSettingPanel?.Invoke();
        }

        /// <summary>
        /// 아이템 획득하기
        /// </summary>
        internal void GainCraftItem()
        {
            // 생산한게 없다면 반환
            if (_prevProductionCount == 0) return;

            _isFirst = true;
            _gainCraftItem?.Invoke(_prevProductionCount, _remainTime);
        }

        internal void RemoveJob()
        {
            // 생산한 아이템이 있다면 먼저 획득
            GainCraftItem();

            _removeJob?.Invoke();
        }
    }
}