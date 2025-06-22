using DG.Tweening;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UISkillSlot : UIBase
    {
        #region ¹ÙÀÎµù
        enum Texts
        {
            SkillName,
            SkillDescription,
        }
        enum Images
        {
            AutoSkill,
        }
        enum Toggles
        {
            AutoSkillToggle,
        }
        #endregion

        [SerializeField] private Color _inActiveColor;
        [SerializeField] private Color _activeColor;

        private TextMeshProUGUI _skillName;
        private TextMeshProUGUI _skillDescription;
        private Image _autoSkillImage;
        private Toggle _autoSkillToggle;

        private ActiveSkillInstance _instance;
        private UITypeTag _typeTag;
        private UISkillExecuteButton _skillExecuteButton;

        protected override void Initialize()
        {
            _typeTag = GetComponentInChildren<UITypeTag>();
            _skillExecuteButton = GetComponentInChildren<UISkillExecuteButton>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindToggle(typeof(Toggles));

            _skillName = GetText((int)Texts.SkillName);
            _skillDescription = GetText((int)Texts.SkillDescription);
            _autoSkillImage = GetImage((int)Images.AutoSkill);
            _autoSkillToggle = GetToggle((int)Toggles.AutoSkillToggle);

            _autoSkillToggle.onValueChanged.AddListener(AutoSkill);
        }

        internal void ShowSkillSlot(AgentUnit unit, SkillTemplate template)
        {
            _skillName.text = template.displayName;
            _skillDescription.text = template.description;

            _skillExecuteButton.Show(unit, template);

            if (template is ActiveSkillTemplate activeSkillTemplate)
            {
                _typeTag.Show(activeSkillTemplate.skillType);
                _instance = unit.GetAbility<ActiveSkillAbility>().GetSkillInstance(activeSkillTemplate);

                _autoSkillToggle.gameObject.SetActive(true);
                if (_instance.isAutoSkill)
                {
                    _autoSkillToggle.isOn = true;
                }
                else
                {
                    _autoSkillToggle.isOn = false;
                }
            }
            else
            {
                _typeTag.Hide(true);
                _autoSkillToggle.gameObject.SetActive(false);
            }

            base.Show(true);
        }

        private void AutoSkill(bool isOn)
        {
            _instance.isAutoSkill = isOn;

            if (isOn)
            {
                _autoSkillImage.color = _activeColor;
                _autoSkillImage.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            }
            else
            {
                _autoSkillImage.color = _inActiveColor;
                _autoSkillImage.transform.DOKill();
            }
        }
    }
}