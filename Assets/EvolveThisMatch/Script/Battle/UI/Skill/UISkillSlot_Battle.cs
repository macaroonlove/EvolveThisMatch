using DG.Tweening;
using EvolveThisMatch.Core;
using FrameWork.UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
{
    public class UISkillSlot_Battle : UISkillSlot
    {
        #region ¹ÙÀÎµù
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

        private Image _autoSkillImage;
        private Toggle _autoSkillToggle;

        private ActiveSkillInstance _instance;
        private UISkillExecuteButton _skillExecuteButton;

        protected override void Initialize()
        {
            base.Initialize();

            _typeTag = GetComponentInChildren<UITypeTag>();
            _skillExecuteButton = GetComponentInChildren<UISkillExecuteButton>();

            BindImage(typeof(Images));
            BindToggle(typeof(Toggles));

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
                _typeTag.gameObject.SetActive(true);

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
                _typeTag.gameObject.SetActive(false);
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
                _autoSkillImage.transform.rotation = Quaternion.identity;
                _autoSkillImage.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            }
            else
            {
                _autoSkillImage.color = _inActiveColor;
                _autoSkillImage.transform.DOKill();
                _autoSkillImage.transform.rotation = Quaternion.identity;
            }
        }
    }
}