using FrameWork.UIBinding;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SkillTreeTestCanvas : UIBase
    {
        #region ¹ÙÀÎµù
        enum Buttons
        {
            ShowButton,
        }
        #endregion

        private Unit _unit;
        private SkillTreeGraph _skillTreeGraph;
        private SkillTreeCanvas _skillTreeCanvas;

        protected override void Awake()
        {
            base.Awake();

            _skillTreeCanvas = GetComponentInChildren<SkillTreeCanvas>();

            BindButton(typeof(Buttons));
            GetButton((int)Buttons.ShowButton).onClick.AddListener(ShowSkillTree);
        }

        internal void Initialize(Unit unit)
        {
            _unit = unit;
            _skillTreeGraph = null;
            if (_unit is AgentUnit agent)
            {
                _skillTreeGraph = agent.template.skillTreeGraph;
            }
            else if (_unit is EnemyUnit enemy)
            {
                _skillTreeGraph = enemy.template.skillTreeGraph;
            }
        }

        private void ShowSkillTree()
        {
            if (_skillTreeGraph == null) return;

            _skillTreeCanvas.Show(_skillTreeGraph);
        }
    }
}