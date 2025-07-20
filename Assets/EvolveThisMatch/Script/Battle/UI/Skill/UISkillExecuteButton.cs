using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    /// <summary>
    /// 배치해두고 사용하는 스킬 버튼
    /// </summary>
    public class UISkillExecuteButton : UIBase
    {
        #region 바인딩
        enum Images
        {
            Icon,
            CooldownTimeImage,
            LackPayAmountImage,
        }
        enum Texts
        {
            CooldownTimeText,
        }
        enum Buttons
        {
            ActiveSkillButton,
        }
        #endregion

        private enum EActionType
        {
            Skill_1,
            Skill_2,
            Skill_3,
            Skill_4,
        }

        [SerializeField] private EActionType _actionType;

        private Image _lackPayAmountImage;
        private Image _coolDownTimeImage;
        private TextMeshProUGUI _coolDownTimeText;

        private InputSystem _inputSystem;

        private Unit _unit;
        private ActiveSkillTemplate _template;
        private ActiveSkillInstance _skillInstance;

        private bool _isCoolDownVisible;
        private float _coolDownRatio;

        protected override void Initialize()
        {
            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            _lackPayAmountImage = GetImage((int)Images.LackPayAmountImage);
            _coolDownTimeImage = GetImage((int)Images.CooldownTimeImage);
            _coolDownTimeText = GetText((int)Texts.CooldownTimeText);

            _lackPayAmountImage.gameObject.SetActive(false);

            GetButton((int)Buttons.ActiveSkillButton).onClick.AddListener(ExecuteSkill);
        }

        internal void Show(AgentUnit unit, SkillTemplate template)
        {
            GetImage((int)Images.Icon).sprite = template.sprite;

            if (template is ActiveSkillTemplate activeSkillTemplate)
            {
                _unit = unit;
                _template = activeSkillTemplate;

                _inputSystem = BattleManager.Instance.GetSubSystem<InputSystem>();
                if (_inputSystem != null)
                {
                    InputBinding();
                }

                _skillInstance = unit.GetAbility<ActiveSkillAbility>().GetSkillInstance(activeSkillTemplate);
                if (_skillInstance != null)
                {
                    _skillInstance.onChangedIsEnoughPayAmount += OnChangedIsEnoughPayAmount;
                }

                _isCoolDownVisible = true;
                CalcCoolDownTimeRatio();
            }
            else
            {
                _coolDownTimeImage.gameObject.SetActive(false);
            }
        }

        internal void Hide()
        {
            _unit = null;
            _template = null;

            if (_inputSystem != null)
            {
                InputCancelBinding();
                _inputSystem = null;
            }

            if (_skillInstance != null)
            {
                _skillInstance.onChangedIsEnoughPayAmount -= OnChangedIsEnoughPayAmount;
            }
        }

        #region Input Binding
        private void InputBinding()
        {
            switch (_actionType)
            {
                case EActionType.Skill_1:
                    _inputSystem.onSkill_1 += ExecuteSkill;
                    break;
                case EActionType.Skill_2:
                    _inputSystem.onSkill_2 += ExecuteSkill;
                    break;
                case EActionType.Skill_3:
                    _inputSystem.onSkill_3 += ExecuteSkill;
                    break;
                case EActionType.Skill_4:
                    _inputSystem.onSkill_4 += ExecuteSkill;
                    break;
            }
        }

        private void InputCancelBinding()
        {
            switch (_actionType)
            {
                case EActionType.Skill_1:
                    _inputSystem.onSkill_1 -= ExecuteSkill;
                    break;
                case EActionType.Skill_2:
                    _inputSystem.onSkill_2 -= ExecuteSkill;
                    break;
                case EActionType.Skill_3:
                    _inputSystem.onSkill_3 -= ExecuteSkill;
                    break;
                case EActionType.Skill_4:
                    _inputSystem.onSkill_4 -= ExecuteSkill;
                    break;
            }
        }
        #endregion

        #region 쿨타임
        private void CalcCoolDownTimeRatio()
        {
            var finalCoolDownTime = _skillInstance.finalCoolDownTime;

            if (finalCoolDownTime == 0)
            {
                _coolDownRatio = 0;
            }
            else
            {
                _coolDownRatio = 1 / finalCoolDownTime;
            }
        }

        private void UpdateCoolDownTime()
        {
            float currentCooldownTime = _skillInstance.coolDownTime;

            if (currentCooldownTime <= 0)
            {
                SetCoolDownVisibility(false);
                return;
            }

            SetCoolDownVisibility(true);
            _coolDownTimeText.text = currentCooldownTime.ToString("F1");
            _coolDownTimeImage.fillAmount = currentCooldownTime * _coolDownRatio;
        }

        private void SetCoolDownVisibility(bool isVisible)
        {
            if (isVisible != _isCoolDownVisible)
            {
                _coolDownTimeImage.gameObject.SetActive(isVisible);
                _isCoolDownVisible = isVisible;
            }
        }
        #endregion

        #region 소모 자원
        private void OnChangedIsEnoughPayAmount(bool isOn)
        {
            _lackPayAmountImage.gameObject.SetActive(isOn);
        }
        #endregion

        private void Update()
        {
            if (_skillInstance == null) return;

            UpdateCoolDownTime();
        }

        private void ExecuteSkill()
        {
            if (_skillInstance == null ||  _skillInstance.CanExecute() == false) return;

            if (_unit.GetAbility<ActiveSkillAbility>().TryExecuteSkill(_template))
            {
                // 쿨타임 적용
                CalcCoolDownTimeRatio();
            }
        }
    }
}