using DG.Tweening;
using EvolveThisMatch.Core;
using FrameWork;
using FrameWork.UIBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EvolveThisMatch.Battle
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
        enum CanvasGroups
        {
            LockSkill,
        }
        #endregion

        [SerializeField] private Color _inActiveColor;
        [SerializeField] private Color _activeColor;

        private TextMeshProUGUI _skillName;
        private TextMeshProUGUI _skillDescription;
        private Image _autoSkillImage;
        private Toggle _autoSkillToggle;
        private CanvasGroupController _lockSkillCanvasGroupController;

        private UITypeTag _typeTag;
        private ActiveSkillInstance _instance;
        private UISkillExecuteButton _skillExecuteButton;

        protected override void Initialize()
        {
            _typeTag = GetComponentInChildren<UITypeTag>();
            _skillExecuteButton = GetComponentInChildren<UISkillExecuteButton>();

            BindText(typeof(Texts));
            BindImage(typeof(Images));
            BindToggle(typeof(Toggles));
            BindCanvasGroupController(typeof(CanvasGroups));

            _skillName = GetText((int)Texts.SkillName);
            _skillDescription = GetText((int)Texts.SkillDescription);
            _autoSkillImage = GetImage((int)Images.AutoSkill);
            _autoSkillToggle = GetToggle((int)Toggles.AutoSkillToggle);
            _lockSkillCanvasGroupController = GetCanvasGroupController((int)CanvasGroups.LockSkill);

            _autoSkillToggle.onValueChanged.AddListener(AutoSkill);
        }

        internal void ShowSkillSlot(AgentUnit unit, SkillTemplate template, bool isUnlock)
        {
            _skillName.text = template.displayName;
            _skillDescription.text = template.description;

            _skillExecuteButton.Show(unit, template);
            _lockSkillCanvasGroupController.ShowOrHide(isUnlock);

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

                _skillExecuteButton.gameObject.SetActive(true);
            }
            else
            {
                _typeTag.gameObject.SetActive(false);
                _autoSkillToggle.gameObject.SetActive(false);
                _skillExecuteButton.gameObject.SetActive(false);
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