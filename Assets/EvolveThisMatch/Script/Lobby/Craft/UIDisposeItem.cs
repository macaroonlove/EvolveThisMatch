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
        #endregion

        private Image _agentBG;
        private Image _fullBody;
        private Image _craftIcon;
        private Image _slider;
        private TextMeshProUGUI _craftName;
        private TextMeshProUGUI _speedText;
        private TextMeshProUGUI _weightText;
        private TextMeshProUGUI _productionCount;
        private TextMeshProUGUI _waitCount;
        private TextMeshProUGUI _remainTime;
        private TextMeshProUGUI _sliderText;
        private TextMeshProUGUI _lockText;
        private CanvasGroupController _lock;

        private DepartmentSaveData.CraftingJob _job;
        private CraftItemData _craftItem;
        private float _timePerItem;
        private int _prevProductionCount;

        private UnityAction _action;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindCanvasGroupController(typeof(CanvasGroups));

            _agentBG = GetImage((int)Images.AgentBG);
            _fullBody = GetImage((int)Images.FullBody);
            _craftIcon = GetImage((int)Images.CraftIcon);
            _slider = GetImage((int)Images.Slider);
            _craftName = GetText((int)Texts.CraftName);
            _speedText = GetText((int)Texts.SpeedText);
            _weightText = GetText((int)Texts.WeightText);
            _productionCount = GetText((int)Texts.ProductionCount);
            _waitCount = GetText((int)Texts.WaitCount);
            _remainTime = GetText((int)Texts.RemainTime);
            _sliderText = GetText((int)Texts.SliderText);
            _lockText = GetText((int)Texts.LockText);
            _lock = GetCanvasGroupController((int)CanvasGroups.Lock);
        }

        internal void Show(DepartmentTemplate template, DepartmentSaveData.CraftingJob job, DepartmentLevelData levelData, UnityAction action)
        {
            _lock.Hide(true);

            _job = job;
            _action = action;
            
            if (job == null)
            {
                _agentBG.color = Color.black;
                _fullBody.enabled = false;
                _craftIcon.enabled = false;
                _slider.fillAmount = 0;
                _craftName.text = "생산품을";
                _speedText.text = "설정해주세요.";
                _weightText.text = "";
                _productionCount.text = "";
                _waitCount.text = "";
                _remainTime.text = "";
                _sliderText.text = "0%";
                
                return;
            }

            var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(job.chargeUnitId);
            _craftItem = template.craftItems[job.craftItemId];

            // 속도
            float agentLevel = GameDataManager.Instance.profileSaveData.GetAgent(job.chargeUnitId).level;
            float craftSpeed = agentLevel * 0.01f + levelData.speed;

            // 아이템 1개 만드는데 걸리는 시간
            _timePerItem = _craftItem.craftTime / craftSpeed;

            // 경과 시간
            TimeSpan elapsed = DateTime.UtcNow - job.startTime;
            float second = (float)elapsed.TotalSeconds;

            // 생산 개수
            int productionCount = Mathf.Min(job.maxAmount, Mathf.FloorToInt(second / _timePerItem));
            _prevProductionCount = productionCount;

            // 대기 개수
            int waitCount = job.maxAmount - productionCount;

            _agentBG.color = Color.white;
            _agentBG.sprite = agentTemplate.rarity.agentInfoSprite;
            _fullBody.enabled = true;
            _fullBody.sprite = agentTemplate.sprite;
            _fullBody.rectTransform.anchoredPosition = agentTemplate.faceCenterPosition + new Vector2(0, -40);
            _craftIcon.enabled = true;
            _craftIcon.sprite = _craftItem.variable.Icon;
            _craftName.text = _craftItem.variable.DisplayName;
            _speedText.text = $"속도  <color=white>{craftSpeed * 100}%</color>";
            _weightText.text = $"무게  <color=white>{_craftItem.weight}kg</color>";
            _productionCount.text = $"생산  <color=white>{productionCount}개</color>";
            _waitCount.text = $"대기  <color=white>{waitCount}개</color>";
        }

        internal void Lock(int unLockIndex)
        {
            _lockText.text = $"Lv. {unLockIndex}에\n해금됩니다.";
            _lock.Show(true);

            _action = null;
        }

        private void Update()
        {
            if (_job == null) return;

            // 경과 시간
            TimeSpan elapsed = DateTime.UtcNow - _job.startTime;
            float second = (float)elapsed.TotalSeconds;

            // 생산 개수
            int productionCount = Mathf.Min(_job.maxAmount, Mathf.FloorToInt(second / _timePerItem));

            // 생산 개수 갱신
            if (productionCount  != _prevProductionCount)
            {
                int waitCount = _job.maxAmount - productionCount;

                _productionCount.text = $"생산  <color=white>{productionCount}개</color>";
                _waitCount.text = $"대기  <color=white>{waitCount}개</color>";
                _prevProductionCount = productionCount;
            }

            // 생산 완료
            if (productionCount >= _job.maxAmount)
            {
                _remainTime.text = "생산 완료";
                _slider.fillAmount = 1f;
                _sliderText.text = "100%";
                return;
            }

            // 남은 시간 계산
            float remainTime = Mathf.Clamp((productionCount + 1) * _timePerItem - second, 0, _timePerItem);
            int remainMinute = Mathf.FloorToInt(remainTime / 60f);
            int remainSecond = Mathf.FloorToInt(remainTime % 60f);

            // 진행도 계산
            float currentItemElapsed = second - (productionCount * _timePerItem);
            float progress = Mathf.Clamp01(currentItemElapsed / _timePerItem);

            _remainTime.text = $"남은 시간\n<color=white>{remainMinute}분 {remainSecond}초</color>";
            _slider.fillAmount = progress;
            _sliderText.text = $"{progress * 100f:F0}%";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _action?.Invoke();
        }
    }
}