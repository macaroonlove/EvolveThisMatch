using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Unit/Enemy", fileName = "Enemy_", order = 1)]
    public class EnemyTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private EnemyRarityTemplate _rarity;
        [HideInInspector, SerializeField] private string _displayName;

        [HideInInspector, SerializeField] private Sprite _sprite;
        [HideInInspector, SerializeField] private GameObject _prefab;

        [HideInInspector, SerializeField] private EMoveType _moveType;
        [HideInInspector, SerializeField] private float _moveSpeed;

        [HideInInspector, SerializeField] private EDamageType _damageType;
        [HideInInspector, SerializeField] private int _atk;
        [HideInInspector, SerializeField] private float _attackTerm;
        [HideInInspector, SerializeField] private float _attackRange;

        [HideInInspector, SerializeField] private int _physicalPenetration;
        [HideInInspector, SerializeField] private int _magicPenetration;

        [HideInInspector, SerializeField] private float _criticalHitChance;
        [HideInInspector, SerializeField] private float _criticalHitDamage;

        [HideInInspector, SerializeField] private int _maxHP;
        [HideInInspector, SerializeField] private int _physicalResistance;
        [HideInInspector, SerializeField] private int _magicResistance;
        [HideInInspector, SerializeField] private int _hpRecoveryPerSec;

        [HideInInspector, SerializeField] private EManaRecoveryType _manaRecoveryType;
        [HideInInspector, SerializeField,] private int _maxMana;
        [HideInInspector, SerializeField] private int _startMana;
        [HideInInspector, SerializeField] private int _manaRecoveryPerSec;

        [HideInInspector, SerializeField] private List<SkillTemplate> _skillTemplates;

        [HideInInspector, SerializeField] private FX _casterFX;
        [HideInInspector, SerializeField] private FX _targetFX;

        #region 프로퍼티
        public int id => _id;
        public RarityTemplate rarity => _rarity;
        public string displayName => _displayName;

        public Sprite sprite => _sprite;
        public GameObject prefab => _prefab;

        public EMoveType MoveType => _moveType;
        public float MoveSpeed => _moveSpeed;

        public EDamageType DamageType => _damageType;
        public int ATK => _atk;
        public float AttackTerm => _attackTerm;
        public float AttackRange => _attackRange;

        public int PhysicalPenetration => _physicalPenetration;
        public int MagicPenetration => _magicPenetration;

        public float CriticalHitChance => _criticalHitChance;
        public float CriticalHitDamage => _criticalHitDamage;

        public int MaxHP => _maxHP;
        public int PhysicalResistance => _physicalResistance;
        public int MagicResistance => _magicResistance;
        public int HPRecoveryPerSec => _hpRecoveryPerSec;

        public EManaRecoveryType ManaRecoveryType => _manaRecoveryType;
        public int MaxMana => _maxMana;
        public int StartMana => _startMana;
        public int ManaRecoveryPerSec => _manaRecoveryPerSec;

        public IReadOnlyList<SkillTemplate> skillTemplates => _skillTemplates;

        public FX casterFX => _casterFX;
        public FX targetFX => _targetFX;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        internal void SetRarity(EnemyRarityTemplate rarity) => _rarity = rarity;
        public void SetDisplayName(string name) => _displayName = name;

        internal void SetMoveType(EMoveType moveType) => _moveType = moveType;
        internal void SetMoveSpeed(float speed) => _moveSpeed = speed;

        internal void SetDamageType(EDamageType damageType) => _damageType = damageType;
        internal void SetATK(int atk) => _atk = atk;
        internal void SetAttackTerm(float term) => _attackTerm = term;
        internal void SetAttackRange(float range) => _attackRange = range;

        internal void SetPhysicalPenetration(int penetration) => _physicalPenetration = penetration;
        internal void SetMagicPenetration(int penetration) => _magicPenetration = penetration;

        internal void SetCriticalHitChance(float chance) => _criticalHitChance = chance;
        internal void SetCriticalHitDamage(float damage) => _criticalHitDamage = damage;

        internal void SetMaxHP(int hp) => _maxHP = hp;
        internal void SetPhysicalResistance(int resistance) => _physicalResistance = resistance;
        internal void SetMagicResistance(int resistance) => _magicResistance = resistance;
        internal void SetHPRecoveryPerSec(int recovery) => _hpRecoveryPerSec = recovery;

        internal void SetManaRecoveryType(EManaRecoveryType type) => _manaRecoveryType = type;
        internal void SetMaxMana(int mana) => _maxMana = mana;
        internal void SetStartMana(int mana) => _startMana = mana;
        internal void SetManaRecoveryPerSec(int recovery) => _manaRecoveryPerSec = recovery;
        #endregion
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using EvolveThisMatch.Core;
    using UnityEditor;

    [CustomEditor(typeof(EnemyTemplate)), CanEditMultipleObjects]
    public class EnemyTemplateEditor : Editor
    {
        private SerializedProperty _id;
        private SerializedProperty _rarity;
        private SerializedProperty _displayName;

        private SerializedProperty _sprite;
        private SerializedProperty _prefab;

        private SerializedProperty _moveType;
        private SerializedProperty _moveSpeed;

        private SerializedProperty _damageType;
        private SerializedProperty _atk;
        private SerializedProperty _attackTerm;
        private SerializedProperty _attackRange;

        private SerializedProperty _physicalPenetration;
        private SerializedProperty _magicPenetration;

        private SerializedProperty _criticalHitChance;
        private SerializedProperty _criticalHitDamage;

        private SerializedProperty _maxHP;
        private SerializedProperty _physicalResistance;
        private SerializedProperty _magicResistance;
        private SerializedProperty _hpRecoveryPerSec;

        private SerializedProperty _manaRecoveryType;
        private SerializedProperty _maxMana;
        private SerializedProperty _startMana;
        private SerializedProperty _manaRecoveryPerSec;

        private SerializedProperty _skillTemplates;

        private SerializedProperty _casterFX;
        private SerializedProperty _targetFX;

        private void OnEnable()
        {
            _id = serializedObject.FindProperty("_id");
            _rarity = serializedObject.FindProperty("_rarity");
            _displayName = serializedObject.FindProperty("_displayName");
            _sprite = serializedObject.FindProperty("_sprite");
            _prefab = serializedObject.FindProperty("_prefab");

            _moveType = serializedObject.FindProperty("_moveType");
            _moveSpeed = serializedObject.FindProperty("_moveSpeed");

            _damageType = serializedObject.FindProperty("_damageType");
            _atk = serializedObject.FindProperty("_atk");
            _attackTerm = serializedObject.FindProperty("_attackTerm");
            _attackRange = serializedObject.FindProperty("_attackRange");

            _physicalPenetration = serializedObject.FindProperty("_physicalPenetration");
            _magicPenetration = serializedObject.FindProperty("_magicPenetration");

            _criticalHitChance = serializedObject.FindProperty("_criticalHitChance");
            _criticalHitDamage = serializedObject.FindProperty("_criticalHitDamage");

            _maxHP = serializedObject.FindProperty("_maxHP");
            _physicalResistance = serializedObject.FindProperty("_physicalResistance");
            _magicResistance = serializedObject.FindProperty("_magicResistance");
            _hpRecoveryPerSec = serializedObject.FindProperty("_hpRecoveryPerSec");

            _manaRecoveryType = serializedObject.FindProperty("_manaRecoveryType");
            _maxMana = serializedObject.FindProperty("_maxMana");
            _startMana = serializedObject.FindProperty("_startMana");
            _manaRecoveryPerSec = serializedObject.FindProperty("_manaRecoveryPerSec");

            _skillTemplates = serializedObject.FindProperty("_skillTemplates");

            _casterFX = serializedObject.FindProperty("_casterFX");
            _targetFX = serializedObject.FindProperty("_targetFX");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            _sprite.objectReferenceValue = EditorGUILayout.ObjectField(_sprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Width(108), GUILayout.Height(108));

            EditorGUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("식별번호", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("등급", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_rarity, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("유닛 이름", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("프리팹", GUILayout.Width(80));
            EditorGUILayout.PropertyField(_prefab, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("이동 방식", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_moveType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("이동 속도", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_moveSpeed, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("데미지 타입", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_damageType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_atk, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 간격", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_attackTerm, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 사거리", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_attackRange, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("물리 관통력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_physicalPenetration, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("마법 관통력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_magicPenetration, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("치명타 확률", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_criticalHitChance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("치명타 데미지", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_criticalHitDamage, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("최대 체력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_maxHP, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("방어력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_physicalResistance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("마법저항력", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_magicResistance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("초당 체력 회복량", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_hpRecoveryPerSec, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("마나 회복 방식", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_manaRecoveryType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("최대 마나", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_maxMana, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("시작 마나", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_startMana, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_manaRecoveryType.enumValueIndex != (int)EManaRecoveryType.None)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("초당 마나 회복량", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_manaRecoveryPerSec, GUIContent.none);
                GUILayout.EndHorizontal();
            }
            else if (_manaRecoveryType.enumValueIndex == (int)EManaRecoveryType.Attack || _manaRecoveryType.enumValueIndex == (int)EManaRecoveryType.Hit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("마나 회복량", GUILayout.Width(192));
                EditorGUILayout.PropertyField(_manaRecoveryPerSec, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_skillTemplates, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 시, 시전자 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_casterFX, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 시, 대상자 FX", GUILayout.Width(192));
            EditorGUILayout.PropertyField(_targetFX, GUIContent.none);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif