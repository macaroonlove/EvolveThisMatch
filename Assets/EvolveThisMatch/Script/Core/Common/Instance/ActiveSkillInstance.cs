using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class ActiveSkillInstance
    {
        private ActiveSkillAbility _activeSkillAbility;
        private HealthAbility _healthAbility;
        private ManaAbility _manaAbility;
        private bool _isEnoughPayAmount;

        public ActiveSkillTemplate template { get; private set; }
        public float coolDownTime { get; private set; }

        internal float finalCoolDownTime
        {
            get
            {
                float result = template.cooldownTime;

                return result;
            }
        }

        public event UnityAction<bool> onChangedIsEnoughPayAmount;

        public ActiveSkillInstance(ActiveSkillTemplate template, ActiveSkillAbility activeSkillAbility, ManaAbility manaAbility)
        {
            this.template = template;
            _activeSkillAbility = activeSkillAbility;
            _healthAbility = _activeSkillAbility.unit.healthAbility;
            _manaAbility = manaAbility;

            _isEnoughPayAmount = true;

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
            else if (template.skillPayType == EActiveSkillPayType.Mana)
            {
                _manaAbility.onChangedMana += OnChangePayAmount;
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

            if (CanExecute() && template.skillTriggerType == EActiveSkillTriggerType.Automatic)
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
            else if (template.skillPayType == EActiveSkillPayType.Mana)
            {
                if (_manaAbility.CheckMana(template.payAmount) == false) return false;
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
            else if (template.skillPayType == EActiveSkillPayType.Mana)
            {
                if (_manaAbility.TryExecuteSkill(template.payAmount) == false) return false;
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
            else if (template.skillPayType == EActiveSkillPayType.Mana)
            {
                _manaAbility.onChangedMana -= OnChangePayAmount;
            }
        }
        #endregion
    }
}