using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 유닛의 액티브 스킬 사용을 제어합니다.
    /// </summary>
    public class ActiveSkillAbility : ConditionAbility
    {
        private UnitAnimationAbility _unitAnimationAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        private bool _isExecuteSkill;
        private Unit _targetUnit;
        private Vector3 _targetVector;

        private Dictionary<int, ActiveSkillInstance> _skills = new Dictionary<int, ActiveSkillInstance>();

        #region 스탯 계산
        private bool finalIsSkillAble
        {
            get
            {
                // 스킬 사용 불가 상태이상에 걸렸다면
                if (_abnormalStatusAbility.UnableToSkillEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _unitAnimationAbility = unit.GetAbility<UnitAnimationAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();
            
            _isExecuteSkill = false;

            InitializeActiveSkillInstance();
        }

        #region 스킬 인스턴스 관리
        private void InitializeActiveSkillInstance()
        {
            if (unit is AgentUnit agentUnit && agentUnit.template.skillTemplates.Count > 0)
            {
                InitializeActiveSkillInstance(agentUnit.template.skillTemplates);
            }
            else if (unit is SummonUnit summonUnit && summonUnit.template.skillTemplates.Count > 0)
            {
                InitializeActiveSkillInstance(summonUnit.template.skillTemplates);
            }
            else if (unit is EnemyUnit enemyUnit && enemyUnit.template.skillTemplates.Count > 0)
            {
                InitializeActiveSkillInstance(enemyUnit.template.skillTemplates);
            }
        }

        private void InitializeActiveSkillInstance(IReadOnlyList<SkillTemplate> skillTemplates)
        {
            foreach (var skillTemplate in skillTemplates)
            {
                if (skillTemplate is ActiveSkillTemplate skill)
                {
                    _skills[skill.id] = (new ActiveSkillInstance(skill, this));
                }
            }
        }

        public ActiveSkillInstance GetSkillInstance(ActiveSkillTemplate template)
        {
            return _skills[template.id];
        }
        #endregion

        internal override void Deinitialize()
        {
            foreach (var skill in _skills.Values)
            {
                skill.Deinitialize();
            }

            _skills.Clear();
        }

        internal override bool IsExecute()
        {
            ActiveSkillCooldown();

            // 스킬을 사용 중이라면 true
            return _isExecuteSkill;
        }

        internal override void UpdateAbility()
        {
            ActiveSkillCooldown();

            if (_isExecuteSkill == false)
            {
                unit.ReleaseCurrentAbility();
            }
        }

        private void ActiveSkillCooldown()
        {
            var deltaTime = Time.deltaTime;
            
            foreach (var skill in _skills.Values)
            {
                skill.Update(deltaTime);
            }
        }

        #region 스킬 발동
        public bool TryExecuteSkill(ActiveSkillTemplate template)
        {
            return TryExecuteSkill(_skills[template.id]);
        }

        internal bool TryExecuteSkill(ActiveSkillInstance skillInstance)
        {
            // 스킬 사용이 불가능한 상태라면
            if (finalIsSkillAble == false) return false;

            // 애니메이션이 있는 스킬인데, 이미 스킬을 사용 중이라면
            if (skillInstance.template.parameterHash != 0 && _isExecuteSkill) return false;

            // 스킬 사용 가능 여부
            if (skillInstance.CanExecute() == false) return false;

            var template = skillInstance.template;
            switch (template.skillTargetingType)
            {
                case EActiveSkillTargetingType.InstantTargeting:
                    return TryExecuteInstantTargetingSkill(template);
                case EActiveSkillTargetingType.NonTargeting:
                    return TryExecuteNonTargetingSkill(template);
            }

            return false;
        }

        #region 스킬 발동 방식별 시도 로직
        private bool TryExecuteInstantTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is GetTargetUnitEffect unitEffect)
                {
                    var targets = unitEffect.GetTarget(unit);

                    if (targets.Count > 0 && targets[0] != null)
                    {
                        return SkillAnimation(template);
                    }
                }
            }

            return false;
        }

        private bool TryExecuteNonTargetingSkill(ActiveSkillTemplate template)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                _targetVector = hit.point.normalized;

                return SkillAnimation(template);
            }

            return false;
        }
        #endregion

        private bool SkillAnimation(ActiveSkillTemplate template)
        {
            // 스킬 소모 자원 지불
            if (_skills[template.id].TryExecute() == false) return false;

            bool isSuccess = false;
            if (template.parameterHash != 0)
            {
                isSuccess = _unitAnimationAbility.TrySetTrigger(template.parameterHash);
            }

            if (isSuccess == true)
            {
                _unitAnimationAbility.SetSkillID(template.id);
                _isExecuteSkill = true;
            }
            else
            {
                ExecuteSkill(template);
            }

            return true;
        }

        internal void ExecuteSkill(int skillTemplateID)
        {
            ExecuteSkill(_skills[skillTemplateID].template);
        }

        private void ExecuteSkill(ActiveSkillTemplate template)
        {
            ExecuteCasterFX(template);

            switch (template.skillTargetingType)
            {
                case EActiveSkillTargetingType.InstantTargeting:
                    ExecuteInstantTargetingSkill(template);
                    break;
                case EActiveSkillTargetingType.NonTargeting:
                    ExecuteNonTargetingSkill(template);
                    break;
            }
        }

        #region 스킬 발동 방식 별 실행 로직
        private void ExecuteInstantTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is GetTargetUnitEffect unitEffect)
                {
                    var targets = unitEffect.GetTarget(unit);

                    foreach (var target in targets)
                    {
                        unitEffect.Execute(unit, target);
                    }
                }
            }
        }

        private void ExecuteNonTargetingSkill(ActiveSkillTemplate template)
        {
            foreach (var effect in template.effects)
            {
                if (effect is PointEffect pointEffect)
                {
                    pointEffect.Execute(unit, _targetVector);
                }
            }
        }
        #endregion

        internal void EndSkill()
        {
            _isExecuteSkill = false;
        }
        #endregion

        #region FX
        private void ExecuteCasterFX(ActiveSkillTemplate template)
        {
            if (template.casterFX != null)
            {
                template.casterFX.Play(unit);
            }
        }
        #endregion
    }
}