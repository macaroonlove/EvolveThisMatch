using FrameWork.UIBinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EvolveThisMatch.Core
{
    public class UISkillNodeController : UIBase, IPointerClickHandler
    {
        #region 바인딩
        enum Images
        {
            Icon,
        }
        enum Texts
        {
            LevelText,
        }
        enum Objects
        {
            Out,
            In,
        }
        #endregion

        private Image _icon;
        private TextMeshProUGUI _levelText;
        private Color _lockColor = new Color(1, 1, 1, 0.8f);

        private SkillNode _skillNode;
        private int _currentLevel;

        internal List<UISkillNodeController> connections = new List<UISkillNodeController>();
        internal List<UISkillPathController> connectionPaths = new List<UISkillPathController>();
        internal List<UISkillNodeController> precedingSkills = new List<UISkillNodeController>();

        internal bool isLock { get; private set; }
        internal RectTransform output { get; private set; }
        internal RectTransform input { get; private set; }

        public void Initialize(SkillNode skillNode)
        {
            _skillNode = skillNode;
            _currentLevel = skillNode.startLevel;

            connections.Clear();
            connectionPaths.Clear();
            precedingSkills.Clear();

            BindImage(typeof(Images));
            BindText(typeof(Texts));
            BindObject(typeof(Objects));

            _icon = GetImage((int)Images.Icon);
            _levelText = GetText((int)Texts.LevelText);

            _icon.sprite = skillNode.skillTemplate.sprite;
            _levelText.text = $"{_currentLevel}/{_skillNode.maxLevel}";

            output = GetObject((int)Objects.Out).transform as RectTransform;
            input = GetObject((int)Objects.In).transform as RectTransform;

            base.Show(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLock) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _currentLevel = Mathf.Clamp(_currentLevel + 1, _skillNode.minLevel, _skillNode.maxLevel);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                _currentLevel = Mathf.Clamp(_currentLevel - 1, _skillNode.minLevel, _skillNode.maxLevel);
            }

            _levelText.text = $"{_currentLevel}/{_skillNode.maxLevel}";
            UpdateNextSkillState();
        }

        #region 노드
        private void UpdateNextSkillState()
        {
            if (_currentLevel > 1) return;

            foreach (var connection in connections)
            {
                connection.UpdateSkillState();
            }

            UpdatePaths();
        }

        internal void UpdateSkillState()
        {
            if (IsUnlock())
            {
                isLock = false;
                _icon.color = Color.white;
            }
            else
            {
                _currentLevel = 0;
                isLock = true;
                _icon.color = _lockColor;
                _levelText.text = $"{_currentLevel}/{_skillNode.maxLevel}";
            }

            UpdateNextSkillState();
        }

        private bool IsUnlock()
        {
            foreach (var precedingSkill in precedingSkills)
            {
                if (precedingSkill._currentLevel <= 0)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 패스
        internal void UpdatePaths()
        {
            bool isLock = (_currentLevel < 1);
            foreach (var path in connectionPaths)
            {
                path.SetColor(isLock);
            }
        }
        #endregion
    }
}