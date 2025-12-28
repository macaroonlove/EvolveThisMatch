using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class DamageCalculateAbility : AlwaysAbility
    {
        private EDamageType _baseDamageType;
        private int _basePhysicalResistance;
        private int _baseMagicResistance;
        private int _basePhysicalPenetration;
        private int _baseMagicPenetration;
        private float _baseCriticalHitChance;
        private float _baseCriticalHitDamage;

        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        #region 프로퍼티
        public int basePhysicalResistance => _basePhysicalResistance;
        public int baseMagicResistance => _baseMagicResistance;
        #endregion

        #region 계산 스탯
        #region 데미지 타입
        internal EDamageType finalDamageType
        {
            get
            {
                EDamageType result = _baseDamageType;

                foreach (var effect in _buffAbility.SetDamageTypeEffects)
                {
                    result = effect.value;
                }

                return result;
            }
        }
        #endregion

        #region 입히는 데미지 추가·차감 & 증가·감소 & 상승·하락
        private int finalDamageAdditional
        {
            get
            {
                int result = 1;

                foreach (var instance in _buffAbility.DamageAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                return result;
            }
        }

        private float finalDamageIncrease
        {
            get
            {
                float result = 1;

                foreach (var instance in _buffAbility.DamageIncreaseDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                return result;
            }
        }

        private float finalDamageMultiplier
        {
            get
            {
                float result = 1;

                foreach (var instance in _buffAbility.DamageMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }

                return result;
            }
        }
        #endregion

        #region 받는 데미지 추가·차감 & 증가·감소 & 상승·하락
        private int finalReceiveDamageAdditional
        {
            get
            {
                int result = 1;

                foreach (var instance in _buffAbility.ReceiveDamageAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                return result;
            }
        }

        private float finalReceiveDamageIncrease
        {
            get
            {
                float result = 1;

                foreach (var instance in _buffAbility.ReceiveDamageIncreaseDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                foreach (var instance in _abnormalStatusAbility.ReceiveDamageIncreaseDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                return result;
            }
        }

        private float finalReceiveDamageMultiplier
        {
            get
            {
                float result = 1;

                foreach (var instance in _buffAbility.ReceiveDamageMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }

                return result;
            }
        }
        #endregion

        #region 저항력
        public int finalPhysicalResistance
        {
            get
            {
                float result = _basePhysicalResistance;

                #region 추가·차감
                foreach (var instance in _buffAbility.PhysicalResistanceAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.PhysicalResistanceIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                foreach (var instance in _abnormalStatusAbility.PhysicalResistanceIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.PhysicalResistanceMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return (int)result;
            }
        }

        public int finalMagicResistance
        {
            get
            {
                float result = _baseMagicResistance;

                #region 추가·차감
                foreach (var instance in _buffAbility.MagicResistanceAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.MagicResistanceIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                foreach (var instance in _abnormalStatusAbility.MagicResistanceIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.MagicResistanceMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return (int)result;
            }
        }
        #endregion

        #region 관통력
        private int finalPhysicalPenetration
        {
            get
            {
                float result = _basePhysicalPenetration;

                #region 추가·차감
                foreach (var instance in _buffAbility.PhysicalPenetrationAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.PhysicalPenetrationIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.PhysicalPenetrationMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return (int)result;
            }
        }

        private int finalMagicPenetration
        {
            get
            {
                float result = _baseMagicPenetration;

                #region 추가·차감
                foreach (var instance in _buffAbility.MagicPenetrationAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.MagicPenetrationIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.MagicPenetrationMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return (int)result;
            }
        }
        #endregion

        #region 치명타
        private bool finalIsCriticalHit
        {
            get
            {
                float chance = _baseCriticalHitChance;

                foreach (var instance in _buffAbility.CriticalHitChanceAdditionalDataEffects)
                {
                    chance += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                if (chance > 0)
                {
                    return Random.value * 100 < chance;
                }
                else
                {
                    return false;
                }
            }
        }

        private float finalCriticalHitDamage
        {
            get
            {
                float result = _baseCriticalHitDamage;

                #region 추가·차감
                foreach (var instance in _buffAbility.CriticalHitDamageAdditionalDataEffects)
                {
                    result += instance.effect.GetValue(unit.effectContext, instance.context);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var instance in _buffAbility.CriticalHitDamageIncreaseDataEffects)
                {
                    increase += instance.effect.GetValue(unit.effectContext, instance.context);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var instance in _buffAbility.CriticalHitDamageMultiplierDataEffects)
                {
                    result *= (1 + instance.effect.GetValue(unit.effectContext, instance.context));
                }
                #endregion

                return result;
            }
        }
        #endregion
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseDamageType = agentUnit.template.DamageType;
                _basePhysicalResistance = agentUnit.template.PhysicalResistance;
                _baseMagicResistance = agentUnit.template.MagicResistance;
                _basePhysicalPenetration = agentUnit.template.PhysicalPenetration;
                _baseMagicPenetration = agentUnit.template.MagicPenetration;
                _baseCriticalHitChance = agentUnit.template.CriticalHitChance;
                _baseCriticalHitDamage = agentUnit.template.CriticalHitDamage;
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseDamageType = enemyUnit.enemyData.template.DamageType;
                _basePhysicalResistance = enemyUnit.enemyData.template.PhysicalResistance;
                _baseMagicResistance = enemyUnit.enemyData.template.MagicResistance;
                _basePhysicalPenetration = enemyUnit.enemyData.template.PhysicalPenetration;
                _baseMagicPenetration = enemyUnit.enemyData.template.MagicPenetration;
                _baseCriticalHitChance = enemyUnit.enemyData.template.CriticalHitChance;
                _baseCriticalHitDamage = enemyUnit.enemyData.template.CriticalHitDamage;
            }
        }

        /// <summary>
        /// 유닛 기본 공격에 의한 피해일 때, 데미지 계산
        /// </summary>
        internal int GetDamage(Unit attackedUnit, EDamageType damageType)
        {
            int finalATK = attackedUnit.GetAbility<AttackAbility>().finalATK;

            // 저항력 & 관통력
            float finalDamage = GetDamageByDamageType(attackedUnit, finalATK, damageType);

            // 공격하는 유닛의 데미지
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage += attackedUnitOfDamageCalculateAbility.finalDamageAdditional;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // 공격받는 유닛의 데미지
            finalDamage += finalReceiveDamageAdditional;
            finalDamage *= finalReceiveDamageIncrease;
            finalDamage *= finalReceiveDamageMultiplier;

            // 치명타가 터졌다면
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // 치명타 데미지
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// 유닛 스킬 공격에 의한 피해일 때, 데미지 계산
        /// (기본 데미지가 이미 정해져 있음)
        /// </summary>
        internal int GetDamage(Unit attackedUnit, int damage, EDamageType damageType)
        {
            // 저항력 & 관통력
            float finalDamage = GetDamageByDamageType(attackedUnit, damage, damageType);

            // 공격하는 유닛의 데미지
            var attackedUnitOfDamageCalculateAbility = attackedUnit.GetAbility<DamageCalculateAbility>();
            finalDamage += attackedUnitOfDamageCalculateAbility.finalDamageAdditional;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageIncrease;
            finalDamage *= attackedUnitOfDamageCalculateAbility.finalDamageMultiplier;

            // 공격받는 유닛의 데미지
            finalDamage += finalReceiveDamageAdditional;
            finalDamage *= finalReceiveDamageIncrease;
            finalDamage *= finalReceiveDamageMultiplier;

            // 치명타가 터졌다면
            if (attackedUnitOfDamageCalculateAbility.finalIsCriticalHit)
            {
                // 치명타 데미지
                finalDamage *= attackedUnitOfDamageCalculateAbility.finalCriticalHitDamage;
            }

            return (int)finalDamage;
        }

        /// <summary>
        /// 아이템, 유물 등의 공격에 의한 피해일 때, 데미지 계산
        /// </summary>
        internal int GetDamage(int damage, EDamageType damageType)
        {
            // 저항력
            float finalDamage = GetDamageByResistance(damage, damageType);

            // 공격받는 유닛의 데미지
            finalDamage += finalReceiveDamageAdditional;
            finalDamage *= finalReceiveDamageIncrease;
            finalDamage *= finalReceiveDamageMultiplier;

            return (int)finalDamage;
        }

        #region 저항력·관통력
        /// <summary>
        /// 저항력만 적용
        /// </summary>
        private int GetDamageByResistance(int finalATK, EDamageType damageType)
        {
            int damage = finalATK;

            switch (damageType)
            {
                case EDamageType.PhysicalDamage:
                    damage = (int)(finalATK * (100 - finalPhysicalResistance) * 0.01f);
                    break;
                case EDamageType.MagicDamage:
                    damage = (int)(finalATK * (100 - finalMagicResistance) * 0.01f);
                    break;
            }

            return damage;
        }

        /// <summary>
        /// 저항력과 관통력 모두 적용
        /// </summary>
        private int GetDamageByDamageType(Unit attackedUnit, int finalATK, EDamageType damageType)
        {
            int damage = finalATK;

            switch (damageType)
            {
                case EDamageType.PhysicalDamage:
                    int finalPhysicalPenetration = attackedUnit.GetAbility<DamageCalculateAbility>().finalPhysicalPenetration;
                    damage = (int)(finalATK * (100 - (finalPhysicalResistance - finalPhysicalPenetration)) * 0.01f);
                    break;
                case EDamageType.MagicDamage:
                    int finalMagicPenetration = attackedUnit.GetAbility<DamageCalculateAbility>().finalMagicPenetration;
                    damage = (int)(finalATK * (100 - (finalMagicResistance - finalMagicPenetration)) * 0.01f);
                    break;
            }

            return damage;
        }
        #endregion
    }
}
