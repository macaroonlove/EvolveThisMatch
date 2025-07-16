using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class ActiveSkillInstance
    {
        private ActiveSkillAbility _activeSkillAbility;
        private HealthAbility _healthAbility;
        private BuffAbility _buffAbility;
        private bool _isEnoughPayAmount;

        public ActiveSkillTemplate template { get; private set; }
        public float coolDownTime { get; private set; }
        internal bool isAutoSkill { get; set; }

        internal float finalCoolDownTime
        {
            get
            {
                float result = template.cooldownTime;

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.SkillCooldownIncreaseDataEffects.Keys)
                {
                    increase -= effect.value;
                }

                result *= increase;
                #endregion

                return Mathf.Max(0.01f, result);
            }
        }

        public event UnityAction<bool> onChangedIsEnoughPayAmount;

        public ActiveSkillInstance(ActiveSkillTemplate template, ActiveSkillAbility activeSkillAbility)
        {
            this.template = template;
            _activeSkillAbility = activeSkillAbility;
            _healthAbility = _activeSkillAbility.unit.healthAbility;
            _buffAbility = _activeSkillAbility.unit.GetAbility<BuffAbility>();

            _isEnoughPayAmount = true;
            isAutoSkill = true;

            if (template.skillTriggerType == EActiveSkillTriggerType.Spawn)
            {
                _activeSkillAbility.TryExecuteSkill(this);
            }
            else if (template.skillTriggerType == EActiveSkillTriggerType.Death)
            {
                _healthAbility.onDeath += OnDeath;
            }

            if (template.skillPayType == EActiveSkillPayType.Health)
            {
                _healthAbility.onChangedHealth += OnChangePayAmount;
            }
        }

        #region 쿨타임 관리
        public float Update(float deltaTime)
        {
            if (coolDownTime > 0)
            {
                coolDownTime -= deltaTime;
                if (coolDownTime < 0) coolDownTime = 0;
            }

            if (CanExecute() && isAutoSkill)
            {
                _activeSkillAbility.TryExecuteSkill(this);
            }

            return coolDownTime;
        }

        public bool CanExecute()
        {
            if (coolDownTime > 0) return false;

            if (_isEnoughPayAmount == false) return false;

            return true;
        }

        public void ResetCoolDown()
        {
            coolDownTime = 0;
        }
        #endregion

        #region 소모 자원 관리
        private void OnChangePayAmount(int amount)
        {
            bool newIsEnoughPayAmount = IsEnoughPayAmount();
            if (newIsEnoughPayAmount != _isEnoughPayAmount)
            {
                _isEnoughPayAmount = newIsEnoughPayAmount;

                onChangedIsEnoughPayAmount?.Invoke(newIsEnoughPayAmount);
            }
        }

        private bool IsEnoughPayAmount()
        {
            if (template.skillPayType == EActiveSkillPayType.Health)
            {
                if (_healthAbility.CheckHP(template.payAmount) == false) return false;
            }

            return true;
        }
        #endregion

        #region 스킬 실행 시도
        public bool TryExecute()
        {
            if (template.skillPayType == EActiveSkillPayType.Health)
            {
                if (_healthAbility.TryExecuteSkill(template.payAmount) == false) return false;
            }
            
            coolDownTime = finalCoolDownTime;

            return true;
        }
        #endregion

        #region 사망 및 객체 해제
        private void OnDeath()
        {
            _activeSkillAbility.TryExecuteSkill(this);

            _healthAbility.onDeath -= OnDeath;
        }

        public void Deinitialize()
        {
            if (template.skillTriggerType == EActiveSkillTriggerType.Death)
            {
                _healthAbility.onDeath -= OnDeath;
            }

            if (template.skillPayType == EActiveSkillPayType.Health)
            {
                _healthAbility.onChangedHealth -= OnChangePayAmount;
            }
        }
        #endregion
    }
}