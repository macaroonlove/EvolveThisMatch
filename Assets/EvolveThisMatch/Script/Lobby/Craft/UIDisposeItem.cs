using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using FrameWork;
using FrameWork.UIBinding;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Lobby
{
    public class UIDisposeItem : UIBase
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

        internal void Show(DepartmentTemplate template, DepartmentSaveData.CraftingJob job, DepartmentLevelData levelData)
        {
            _lock.Hide(true);

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

            //TimeSpan elapsed = DateTime.UtcNow - job.startTime;
            //float second = (float)elapsed.TotalSeconds;
            
            //float timePerItem = craftItem.craftTime / craftSpeed;
            //int count = Mathf.Min(job.maxAmount, Mathf.FloorToInt(second / timePerItem));

            var agentTemplate = GameDataManager.Instance.GetAgentTemplateById(job.chargeUnitId);
            var craftItem = template.craftItems[job.craftItemId];
            float craftSpeed = GameDataManager.Instance.profileSaveData.GetAgent(job.chargeUnitId).level + levelData.speed;

            _agentBG.color = Color.white;
            _agentBG.sprite = agentTemplate.rarity.agentInfoSprite;
            _fullBody.enabled = true;
            _fullBody.sprite = agentTemplate.sprite;
            _fullBody.rectTransform.anchoredPosition = agentTemplate.faceCenterPosition;
            _craftIcon.enabled = true;
            _craftIcon.sprite = craftItem.variable.Icon;
            _craftName.text = craftItem.variable.DisplayName;
            _speedText.text = $"속도  <color=white>{craftSpeed}%</color>";
            _weightText.text = $"무게  <color=white>{craftItem.weight}kg</color>";
            _productionCount.text = $"생산  <color=white>{0}개</color>";
            _waitCount.text = $"대기  <color=white>{0}개</color>";
            _remainTime.text = $"남은 시간\n<color=white>{0}분 {0}초</color>";
        }

        internal void Lock(int unLockIndex)
        {
            _lockText.text = $"Lv. {unLockIndex}에\n해금됩니다.";
            _lock.Show(true);
        }
    }
}