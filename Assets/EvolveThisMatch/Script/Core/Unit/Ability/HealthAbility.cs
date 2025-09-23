using DamageNumbersPro;
using FrameWork.Editor;
using FrameWork.GameSettings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EvolveThisMatch.Core
{
    public class HealthAbility : AlwaysAbility
    {
        private PoolSystem _poolSystem;

        private PassiveSkillAbility _passiveSkillAbility;
        private DeployAbility _deployAbility;
        private BuffAbility _buffAbility;
        private AbnormalStatusAbility _abnormalStatusAbility;

        private int _baseMaxHP;
        private int _currentHP;
        private int _baseHPRecoveryPerSec;
        private float _hpRecoveryCooldown = 1;
        private bool _isDamageVisible;

        internal int currentHP => _currentHP;
        internal bool isAlive => _currentHP > 0;

        #region 보호막 필드
        private class ShieldInstance
        {
            public Coroutine coroutine;
            public int id;
            public int amount;
            public float duration;

            // 무한 지속 보호막
            public ShieldInstance(int amount, float duration)
            {
                this.amount = amount;
                this.duration = duration;
            }

            // 지속시간이 있는 보호막
            public ShieldInstance(Coroutine coroutine, int id, int amount, float duration)
            {
                this.coroutine = coroutine;
                this.id = id;
                this.amount = amount;
                this.duration = duration;
            }
        }

        [SerializeField, Label("보호막")] private GameObject _shieldFX;

        private List<ShieldInstance> _shields = new List<ShieldInstance>();
        private GameObject _shieldObject;
        private int _shieldIdCounter = 0;

        internal int shieldCount => _shields.Count;
        internal int shieldAmount => _shields.Sum(shield => shield.amount);
        #endregion

        internal event UnityAction<int, int> onDamage;
        internal event UnityAction<int> onChangedHealth;
        internal event UnityAction<int> onChangedShield;
        internal event UnityAction onDeath;

        [Header("팝업 텍스트")]
        [SerializeField, Label("물리 데미지량 팝업")] private DamageNumber _physicalDamagePopup;
        [SerializeField, Label("마법 데미지량 팝업")] private DamageNumber _magicDamagePopup;
        [SerializeField, Label("고정 데미지량 팝업")] private DamageNumber _trueDamagePopup;
        [SerializeField, Label("회복량 팝업")] private DamageNumber _healPopup;
        [SerializeField, Label("보호막 흡수량 팝업")] private DamageNumber _shieldPopup;

        #region 계산 스탯
        // 최대 HP
        internal int finalMaxHP
        {
            get
            {
                float result = _baseMaxHP;

                #region 추가·차감
                foreach (var effect in _buffAbility.MaxHPAdditionalDataEffects)
                {
                    result += effect.Key.GetValue(effect.Value);
                }
                #endregion

                #region 증가·감소
                float increase = 1;

                foreach (var effect in _buffAbility.MaxHPIncreaseDataEffects)
                {
                    increase += effect.Key.GetValue(effect.Value);
                }

                result *= increase;
                #endregion

                #region 상승·하락
                foreach (var effect in _buffAbility.MaxHPMultiplierDataEffects)
                {
                    result *= effect.Key.GetValue(effect.Value);
                }
                #endregion

                return (int)result;
            }
        }

        // 고정될 최소 체력
        // 고정될 체력이 여러 개 있다면 가장 큰 것을 채택
        private int finalMinHP
        {
            get
            {
                int maxMinHP = 0;

                foreach (var effect in _buffAbility.SetMinHPEffects)
                {
                    maxMinHP = Mathf.Max(maxMinHP, effect.value);
                }

                return maxMinHP;
            }
        }

        // 초당 HP 회복량
        private int finalHPRecoveryPerSec
        {
            get
            {
                int result = _baseHPRecoveryPerSec;

                // 최대 체력의 % 만큼 초당 회복력 추가
                foreach (var effect in _buffAbility.HPRecoveryPerSecByMaxHPIncreaseDataEffects)
                {
                    result += (int)(effect.Key.GetValue(effect.Value) * finalMaxHP);
                }

                // 최대 체력의 % 만큼 초당 회복력 추가
                foreach (var effect in _abnormalStatusAbility.HPRecoveryPerSecByMaxHPIncreaseDataEffects)
                {
                    result += (int)(effect.Key.GetValue(effect.Value) * finalMaxHP);
                }

                return result;
            }
        }

        #region 추가 회복량
        private int healingAdditional
        {
            get
            {
                int result = 0;

                foreach (var effect in _buffAbility.HealingAdditionalDataEffects)
                {
                    result += effect.Key.GetValue(effect.Value);
                }

                return result;
            }
        }

        private float healingIncrease
        {
            get
            {
                float result = 1;

                foreach (var effect in _buffAbility.HealingIncreaseDataEffects)
                {
                    result += effect.Key.GetValue(effect.Value);
                }

                return result;
            }
        }

        private float healingMultiplier
        {
            get
            {
                float result = 1;

                foreach (var effect in _buffAbility.HealingMultiplierDataEffects)
                {
                    result *= effect.Key.GetValue(effect.Value);
                }

                return result;
            }
        }
        #endregion

        internal bool finalIsHealAble
        {
            get
            {
                // 풀피라면
                if (_currentHP == finalMaxHP) return false;

                // 회복 불가 상태이상에 걸렸다면
                if (_abnormalStatusAbility.UnableToHealEffects.Count > 0) return false;

                return true;
            }
        }
        #endregion

        internal override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();

            _passiveSkillAbility = unit.GetAbility<PassiveSkillAbility>();
            _buffAbility = unit.GetAbility<BuffAbility>();
            _abnormalStatusAbility = unit.GetAbility<AbnormalStatusAbility>();

            if (unit is AgentUnit agentUnit)
            {
                _baseMaxHP = agentUnit.template.MaxHP;
                _baseHPRecoveryPerSec = agentUnit.template.HPRecoveryPerSec;
                _deployAbility = unit.GetAbility<DeployAbility>();
            }
            else if (unit is EnemyUnit enemyUnit)
            {
                _baseMaxHP = enemyUnit.enemyData.hp;
                _baseHPRecoveryPerSec = enemyUnit.enemyData.template.HPRecoveryPerSec;
            }

            SetHP(finalMaxHP);

            _isDamageVisible = GameSettingsManager.DamageVisible;
            GameSettingsManager.DamageVisibleChanged += DamageVisibleChanged;
        }

        internal override void Deinitialize()
        {
            GameSettingsManager.DamageVisibleChanged -= DamageVisibleChanged;
        }

        private void DamageVisibleChanged(bool isOn)
        {
            _isDamageVisible = isOn;
        }

        internal override void UpdateAbility()
        {
            if (_deployAbility != null && _deployAbility.isSortie == true) return;

            // 초당 회복 쿨타임 감소
            if (_hpRecoveryCooldown > 0)
            {
                _hpRecoveryCooldown -= Time.deltaTime;
                return;
            }

            var hpRecoveryAmount = finalHPRecoveryPerSec;
            
            if (hpRecoveryAmount > 0)
            {
                // 회복 불가인 상태라면 무시
                if (finalIsHealAble == false) return;

                SetHP(_currentHP + hpRecoveryAmount);
                _hpRecoveryCooldown = 1;
            }
            else if (hpRecoveryAmount < 0)
            {
                Damaged(-hpRecoveryAmount, EDamageType.TrueDamage, 1000);
                _hpRecoveryCooldown = 1;
            }
        }

        #region HP 변경
        internal bool Damaged(int damage, EDamageType damageType, int id)
        {
            // 죽었으면 무시
            if (isAlive == false) return false;

            // 실드에 막히는 데미지를 제외
            var lostHealth = DamagedShield(damage);
            lostHealth = Mathf.Max(0, lostHealth);

            // 실드의 흡수량
            var absorption = damage - lostHealth;
            if (absorption > 0)
            {
                ShieldPopup(absorption);
            }

            // 잃을 HP 가 있을 때
            if (lostHealth > 0)
            {
                SetHP(_currentHP - lostHealth);
                onDamage?.Invoke(id, lostHealth);

                DamagePopup(lostHealth, damageType);

                return true;
            }

            return false;
        }

        internal void Healed(int value, Unit casterUnit = null)
        {
            // 회복 불가인 상태라면 무시
            if (finalIsHealAble == false) return;

            float healingAmount = value;

            // 추가 회복량 적용
            healingAmount += healingAdditional;
            healingAmount *= healingIncrease;
            healingAmount *= healingMultiplier;

            var lastHp = Mathf.RoundToInt(_currentHP + healingAmount);

            HealPopup(healingAmount);

            SetHP(lastHp);

            foreach (var effect in _passiveSkillAbility.healEventEffects)
            {
                effect.Execute(unit, casterUnit, 1);
            }
        }

        private void SetHP(int hp)
        {
            _currentHP = Mathf.Clamp(hp, finalMinHP, finalMaxHP);
            if (_currentHP == 0)
            {
                onDeath?.Invoke();
                unit.OnDeath();
                return;
            }
            onChangedHealth?.Invoke(_currentHP);
        }
        #endregion

        #region 보호막 변경
        private int DamagedShield(int damage)
        {
            int finalDamage = damage;

            // 실드가 있을 떄
            if (shieldCount > 0)
            {
                int totalShield = shieldAmount;

                // 보호막으로 방어 했을때
                if (totalShield >= damage)
                {
                    // 최근에 추가된 보호막부터 차감
                    for (int i = shieldCount - 1; i >= 0; i--)
                    {
                        var shield = _shields[i];
                        int remainingShield = shield.amount - damage;

                        if (remainingShield >= 0)
                        {
                            shield.amount = remainingShield;
                            onChangedShield?.Invoke(totalShield - damage);
                            return 0;
                        }
                        else
                        {
                            damage -= shield.amount;

                            if (shield.coroutine != null)
                            {
                                StopCoroutine(shield.coroutine);
                            }
                            _shields.RemoveAt(i);
                            UpdateShield();
                        }
                    }

                    #region 오래된 보호막부터 차감
                    //for (int i = 0; i < shieldCount; i++)
                    //{
                    //    var shield = _shields[i];
                    //    int remainingShield = shield.amount - damage;
                    //    if (remainingShield >= 0)
                    //    {
                    //        shield.amount = remainingShield;
                    //        onChangedShield?.Invoke(totalShield - damage);
                    //        return 0;
                    //    }
                    //    else
                    //    {
                    //        damage -= shield.amount;

                    //        if (shield.coroutine != null)
                    //        {
                    //            StopCoroutine(shield.coroutine);
                    //        }
                    //        _shields.RemoveAt(i);
                    //        UpdateShield();
                    //        i--;
                    //    }
                    //}
                    #endregion
                }
                // 보호막으로 방어하지 못하고 피가 까일 때
                else
                {
                    _shields.Clear();
                    UpdateShield();
                    onChangedShield?.Invoke(0);
                }

                finalDamage -= totalShield;
            }

            return finalDamage;
        }

        internal void AddShield(int amount)
        {
            //죽었으면 무시
            if (isAlive == false) return;

            _shields.Add(new ShieldInstance(amount, int.MaxValue));
            UpdateShield();
        }

        internal void AddShield(int amount, float duration)
        {
            //죽었으면 무시
            if (isAlive == false) return;

            var coroutine = StartCoroutine(CoShield(_shieldIdCounter, duration));
            _shields.Add(new ShieldInstance(coroutine, _shieldIdCounter, amount, duration));
            UpdateShield();
            _shieldIdCounter++;
        }

        private void UpdateShield()
        {
            onChangedShield?.Invoke(shieldAmount);

            if (_shieldFX == null) return;

            if (shieldCount > 0)
            {
                _shieldObject = _poolSystem.Spawn(_shieldFX, this.transform);
            }
            else
            {
                _poolSystem.DeSpawn(_shieldObject);
            }
        }

        private IEnumerator CoShield(int id, float duration)
        {
            yield return new WaitForSeconds(duration);

            for (int i = 0; i < shieldCount; i++)
            {
                if (_shields[i].id == id)
                {
                    _shields.RemoveAt(i);
                    break;
                }
            }
            UpdateShield();
        }
        #endregion

        #region 팝업 텍스트
        private void DamagePopup(float damage, EDamageType damageType)
        {
            if (this == null) return;
            if (!_isDamageVisible) return;

            DamageNumber popup;

            switch (damageType)
            {
                case EDamageType.PhysicalDamage:
                    popup = _physicalDamagePopup?.Spawn(transform.position, damage);
                    break;
                case EDamageType.MagicDamage:
                    popup = _magicDamagePopup?.Spawn(transform.position, damage);
                    break;
                default:
                    popup = _trueDamagePopup?.Spawn(transform.position, damage);
                    break;
            }

            popup?.SetScale(0.5f);
        }

        private void HealPopup(float heal)
        {
            if (this == null) return;
            if (!_isDamageVisible) return;

            DamageNumber popup = _healPopup?.Spawn(transform.position, heal);

            popup?.SetScale(0.5f);
        }

        private void ShieldPopup(float absorption)
        {
            if (this == null) return;
            if (!_isDamageVisible) return;

            DamageNumber popup = _shieldPopup?.Spawn(transform.position, absorption);

            popup?.SetScale(0.5f);
        }
        #endregion

        #region 스킬 사용
        internal bool TryExecuteSkill(int needHP)
        {
            // 필요한 마나보다 현재 마나가 적다면
            if (_currentHP < needHP) return false;

            int finalHP = _currentHP - needHP;

            SetHP(finalHP);

            return true;
        }

        /// <summary>
        /// 체력이 충분하다면 True, 충분하지 않다면 False
        /// </summary>
        internal bool CheckHP(int needHP)
        {
            return _currentHP >= needHP;
        }
        #endregion
    }
}