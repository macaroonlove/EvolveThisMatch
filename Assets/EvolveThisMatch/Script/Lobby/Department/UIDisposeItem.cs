using Cysharp.Threading.Tasks;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.NetworkTime;
using FrameWork.PlayFabExtensions;
using FrameWork.UIBinding;
using ScriptableObjectArchitecture;
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

        private int _prevCraftCount;
        private float _remainTime;
        private bool _isFirst;

        private UnityAction _showDisposeSettingPanel;
        private UnityAction<int> _completeCraft;
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

            _prevCraftCount = -1;
            _isFirst = true;
        }

        /// <summary>
        /// 해당 작업대가 해금되지 않았을 때 실행
        /// </summary>
        internal void Lock(int unLockIndex)
        {
            _showDisposeSettingPanel = null;
            _completeCraft = null;

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
        internal async void Initialize(DepartmentLocalSaveData.CraftingJob job, DepartmentCraftData craftItem, CraftResult craftResult, float craftSpeed, UnityAction showDisposeSettingPanel, UnityAction<int> completeCraft, UnityAction<int, float> gainCraftItem, UnityAction removeJob)
        {
            // 잠금 이미지 끄기
            _lock.Hide(true);

            _showDisposeSettingPanel = showDisposeSettingPanel;
            _completeCraft = completeCraft;
            _gainCraftItem = gainCraftItem;
            _removeJob = removeJob;

            var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(job.unitId);

            // 생산 개수
            int productionCount = craftResult.craftCount;

            // 대기 개수
            int waitCount = job.maxAmount - productionCount;
            
            var variable = await AddressableAssetManager.Instance.GetScriptableObject<ObscuredIntVariable>(craftItem.Variable);

            _agentBG.color = Color.white;
            _agentBG.sprite = agentTemplate.rarity.agentInfoSprite;
            _craftBG.color = Color.white;
            _craftBG.sprite = variable.IconBG;
            _fullBody.enabled = true;
            _fullBody.sprite = agentTemplate.sprite;
            _fullBody.rectTransform.anchoredPosition = agentTemplate.faceCenterPosition + new Vector2(0, -40);
            _craftIcon.enabled = true;
            _craftIcon.sprite = variable.Icon;
            _craftName.text = variable.DisplayName;
            _speedText.text = $"속도  <color=white>{craftSpeed * 100}%</color>";
            _weightText.text = $"무게  <color=white>{craftItem.Weight}kg</color>";
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

        internal async UniTask<bool> UpdateItem(DepartmentLevelData levelData, DepartmentCraftData craftItem, DepartmentLocalSaveData.CraftingJob job, float timePerItem)
        {
            // 경과 시간
            var currentTime = await NetworkTimeManager.Instance.GetUtcNow();
            TimeSpan elapsed = currentTime - job.startTime;
            float second = (float)elapsed.TotalSeconds;

            // 최대 생산량
            int maxAmount = job.maxAmount;
            
            // 생산 개수
            int craftCount = Mathf.Min(maxAmount, Mathf.FloorToInt(second / timePerItem));

            // 생산 개수 갱신
            if (craftCount != _prevCraftCount)
            {
                // 대기 개수
                int waitCount = maxAmount - craftCount;

                _productionCount.text = $"생산  <color=white>{craftCount}개</color>";
                _waitCount.text = $"대기  <color=white>{waitCount}개</color>";

                _prevCraftCount = craftCount;

                if (_isFirst == false)
                {
                    _completeCraft?.Invoke(craftItem.Weight);
                }

                _isFirst = false;
            }

            // 생산 완료
            if (craftCount >= maxAmount)
            {
                _remainTimeText.text = "생산 완료";
                _slider.fillAmount = 1f;
                _sliderText.text = "100%";
                return false;
            }
            else
            {
                // 남은 시간 계산
                _remainTime = Mathf.Clamp((craftCount + 1) * timePerItem - second, 0, timePerItem);
                int remainMinute = Mathf.FloorToInt(_remainTime / 60f);
                int remainSecond = Mathf.FloorToInt(_remainTime % 60f);

                _remainTimeText.text = $"남은 시간\n<color=white>{remainMinute}분 {remainSecond}초</color>";

                // 진행도 계산
                float currentItemElapsed = second - (craftCount * timePerItem);
                float progress = Mathf.Clamp01(currentItemElapsed / timePerItem);

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
            if (_prevCraftCount == 0) return;

            _isFirst = true;
            _gainCraftItem?.Invoke(_prevCraftCount, _remainTime);
        }

        internal (int, float) GetDataToSendServer()
        {
            return (_prevCraftCount, _remainTime);
        }

        internal void RemoveJob()
        {
            _removeJob?.Invoke();
        }
    }
}