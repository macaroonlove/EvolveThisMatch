using Cysharp.Threading.Tasks;
using EvolveThisMatch.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Templates/Unit/Agent", fileName = "Agent_", order = 0)]
    public class AgentTemplate : ScriptableObject, IDataWindowEntry
    {
        [HideInInspector, SerializeField] private int _id;
        [HideInInspector, SerializeField] private AgentRarityTemplate _rarity;
        [HideInInspector, SerializeField] private JobTemplate _job;
        [HideInInspector, SerializeField] private List<SynergyTemplate> _synergy = new List<SynergyTemplate>();
        [HideInInspector, SerializeField] private string _displayName;

        [HideInInspector, SerializeField] private EMoveType _moveType;
        [HideInInspector, SerializeField] private float _moveSpeed;
        [HideInInspector, SerializeField] private float _chaseRange;
        [HideInInspector, SerializeField] private float _chaseFailRange;

        [HideInInspector, SerializeField] private EAttackType _attackType;
        [HideInInspector, SerializeField] private EDamageType _damageType;
        [HideInInspector, SerializeField] private int _atk;
        [HideInInspector, SerializeField] private float _attackTerm;
        [HideInInspector, SerializeField] private float _attackRange;

        [HideInInspector, SerializeField] private bool _isProjectileAttack;
        [HideInInspector, SerializeField] private GameObject _projectilePrefab;
        [HideInInspector, SerializeField] private ESpawnPoint _spawnPoint;

        [HideInInspector, SerializeField] private int _physicalPenetration;
        [HideInInspector, SerializeField] private int _magicPenetration;

        [HideInInspector, SerializeField] private float _criticalHitChance;
        [HideInInspector, SerializeField] private float _criticalHitDamage;

        [HideInInspector, SerializeField] private int _maxHP;
        [HideInInspector, SerializeField] private int _physicalResistance;
        [HideInInspector, SerializeField] private int _magicResistance;
        [HideInInspector, SerializeField] private int _hpRecoveryPerSec;

        [HideInInspector, SerializeField] private List<SkillTemplate> _skillTemplates;

        [HideInInspector]
        public List<SkinTemplate> skins = new List<SkinTemplate>();

        private List<SkinTemplate> _loadedSkins = new List<SkinTemplate>();
        private int _selectedSkinId => SaveManager.Instance.agentData.GetSelectedAgentSkinId(id);

        #region 프로퍼티
        public int id => _id;
        public AgentRarityTemplate rarity => _rarity;
        public JobTemplate job => _job;
        public IReadOnlyList<SynergyTemplate> synergy => _synergy;
        public string displayName => _displayName;

        public Sprite sprite => (skins.Count == 0) ? null : skins[0]?.faceSprite;
        public Vector2 faceCenterPosition => (skins.Count == 0) ? Vector2.zero : skins[0].faceCenterPosition;
        public GameObject prefab => skins[_selectedSkinId]?.battleTemplate.prefab;
        public GameObject overUIPrefab
        {
            get
            {
                int skinId = _selectedSkinId;
                if (skinId == -1) skinId = 0;

                return skins[skinId]?.lobbyTemplate?.overUIPrefab;
            }
        }

        public EMoveType MoveType => _moveType;
        public float MoveSpeed => _moveSpeed;
        public float ChaseRange => _chaseRange;
        public float ChaseFailRange => _chaseFailRange;

        public EAttackType AttackType => _attackType;
        public EDamageType DamageType => _damageType;
        public int ATK => _atk;
        public float AttackTerm => _attackTerm;
        public float AttackRange => _attackRange;

        public bool isProjectileAttack => _isProjectileAttack;
        public GameObject projectilePrefab => _projectilePrefab;
        public ESpawnPoint spawnPoint => _spawnPoint;

        public int PhysicalPenetration => _physicalPenetration;
        public int MagicPenetration => _magicPenetration;

        public float CriticalHitChance => _criticalHitChance;
        public float CriticalHitDamage => _criticalHitDamage;

        public int MaxHP => _maxHP;
        public int PhysicalResistance => _physicalResistance;
        public int MagicResistance => _magicResistance;
        public int HPRecoveryPerSec => _hpRecoveryPerSec;

        public IReadOnlyList<SkillTemplate> skillTemplates => _skillTemplates;

        public FX casterFX => skins[_selectedSkinId]?.battleTemplate.casterFX;
        public FX targetFX => skins[_selectedSkinId]?.battleTemplate.targetFX;
        #endregion

        #region 값 변경 메서드
        internal void SetId(int id) => _id = id;
        internal void SetRarity(AgentRarityTemplate rarity) => _rarity = rarity;
        internal void SetJob(JobTemplate job) => _job = job;
        public void SetDisplayName(string name) => _displayName = name;

        internal void SetMoveType(EMoveType moveType) => _moveType = moveType;
        internal void SetMoveSpeed(float speed) => _moveSpeed = speed;
        internal void SetChaseRange(float range) => _chaseRange = range;
        internal void SetChaseFailRange(float range) => _chaseFailRange = range;

        internal void SetAttackType(EAttackType attackType) => _attackType = attackType;
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
        #endregion

        #region Load
        public async UniTask LoadAllSkinLobbyTemplate()
        {
            var skinIds = SaveManager.Instance.agentData.GetAllAgentSkinId(id);

            var tasks = new UniTask[skinIds.Count];

            for (int i = 0; i < skinIds.Count; i++)
            {
                tasks[i] = LoadSkinLobbyTemplate(skinIds[i]);
            }

            await UniTask.WhenAll(tasks);
        }

        public async UniTask LoadSelectedSkinLobbyTemplate()
        {
            int skinId = _selectedSkinId;

            await LoadSkinLobbyTemplate(skinId);
        }

        private async UniTask LoadSkinLobbyTemplate(int skinId)
        {
            var skin = FindSkin(skins, skinId);

            if (skin != null)
            {
                await skin.LoadSkinLobbyTemplate();

                if (!_loadedSkins.Contains(skin))
                {
                    _loadedSkins.Add(skin);
                }
            }
        }

        public async UniTask LoadSkinBattleTemplate()
        {
            int skinId = _selectedSkinId;

            var skin = FindSkin(skins, skinId);

            if (skin != null)
            {
                await skin.LoadSkinBattleTemplate();

                if (!_loadedSkins.Contains(skin))
                {
                    _loadedSkins.Add(skin);
                }
            }
        }
        #endregion

        #region Release
        public void ReleaseSkinLobbyTemplate()
        {
            foreach (var skin in _loadedSkins)
            {
                skin.ReleaseSkinLobbyTemplate();
            }
        }

        public void ReleaseSkinBattleTemplate()
        {
            foreach (var skin in _loadedSkins)
            {
                skin.ReleaseSkinBattleTemplate();
            }
        }
        #endregion

        #region 유틸리티
        private SkinTemplate FindSkin(List<SkinTemplate> skins, int skinId)
        {
            for (int i = 0; i < skins.Count; i++)
            {
                if (skins[i].id == skinId)
                {
                    return skins[i];
                }
            }

            return null;
        }
        #endregion
    }
}

#if UNITY_EDITOR
namespace EvolveThisMatch.Editor
{
    using System.IO;
    using EvolveThisMatch.Core;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditorInternal;
    using UnityEngine.AddressableAssets;

    [CustomEditor(typeof(AgentTemplate)), CanEditMultipleObjects]
    public class AgentTemplateEditor : Editor
    {
        private AgentTemplate _target;

        private SerializedProperty _id;
        private SerializedProperty _rarity;
        private SerializedProperty _job;
        private SerializedProperty _synergy;
        private SerializedProperty _displayName;

        private SerializedProperty _moveType;
        private SerializedProperty _moveSpeed;
        private SerializedProperty _chaseRange;
        private SerializedProperty _chaseFailRange;

        private SerializedProperty _attackType;
        private SerializedProperty _damageType;
        private SerializedProperty _atk;
        private SerializedProperty _attackTerm;
        private SerializedProperty _attackRange;

        private SerializedProperty _isProjectileAttack;
        private SerializedProperty _projectilePrefab;
        private SerializedProperty _spawnPoint;

        private SerializedProperty _physicalPenetration;
        private SerializedProperty _magicPenetration;

        private SerializedProperty _criticalHitChance;
        private SerializedProperty _criticalHitDamage;

        private SerializedProperty _maxHP;
        private SerializedProperty _physicalResistance;
        private SerializedProperty _magicResistance;
        private SerializedProperty _hpRecoveryPerSec;

        private SerializedProperty _skillTemplates;

        private ReorderableList _skinList;
        private SkinTemplate _currentSkin;
        private bool _showSkinList = true;

        private void OnEnable()
        {
            _target = target as AgentTemplate;

            _id = serializedObject.FindProperty("_id");
            _rarity = serializedObject.FindProperty("_rarity");
            _job = serializedObject.FindProperty("_job");
            _synergy = serializedObject.FindProperty("_synergy");
            _displayName = serializedObject.FindProperty("_displayName");

            _moveType = serializedObject.FindProperty("_moveType");
            _moveSpeed = serializedObject.FindProperty("_moveSpeed");
            _chaseRange = serializedObject.FindProperty("_chaseRange");
            _chaseFailRange = serializedObject.FindProperty("_chaseFailRange");

            _attackType = serializedObject.FindProperty("_attackType");
            _damageType = serializedObject.FindProperty("_damageType");
            _atk = serializedObject.FindProperty("_atk");
            _attackTerm = serializedObject.FindProperty("_attackTerm");
            _attackRange = serializedObject.FindProperty("_attackRange");

            _isProjectileAttack = serializedObject.FindProperty("_isProjectileAttack");
            _projectilePrefab = serializedObject.FindProperty("_projectilePrefab");
            _spawnPoint = serializedObject.FindProperty("_spawnPoint");

            _physicalPenetration = serializedObject.FindProperty("_physicalPenetration");
            _magicPenetration = serializedObject.FindProperty("_magicPenetration");

            _criticalHitChance = serializedObject.FindProperty("_criticalHitChance");
            _criticalHitDamage = serializedObject.FindProperty("_criticalHitDamage");

            _maxHP = serializedObject.FindProperty("_maxHP");
            _physicalResistance = serializedObject.FindProperty("_physicalResistance");
            _magicResistance = serializedObject.FindProperty("_magicResistance");
            _hpRecoveryPerSec = serializedObject.FindProperty("_hpRecoveryPerSec");

            _skillTemplates = serializedObject.FindProperty("_skillTemplates");

            CreateSkinList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.Label("식별번호", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_id, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("등급", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_rarity, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("직군", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_job, GUIContent.none);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("시너지", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_synergy, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("유닛 이름", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_displayName, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킨", GUILayout.Width(140));
            _showSkinList = EditorGUILayout.Foldout(_showSkinList, "Skins", true);
            GUILayout.EndHorizontal();


            if (_showSkinList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(140));
                EditorGUI.indentLevel++;
                _skinList?.DoLayoutList();
                EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("이동 방식", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_moveType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("이동 속도", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_moveSpeed, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("추적 거리", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_chaseRange, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("추적 실패 거리", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_chaseFailRange, GUIContent.none);
            GUILayout.EndHorizontal();

            _chaseFailRange.floatValue = Mathf.Max(_chaseRange.floatValue, _chaseFailRange.floatValue);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 방식", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_attackType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("데미지 타입", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_damageType, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_atk, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 간격", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_attackTerm, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("공격 사거리", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_attackRange, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("투사체 사용 여부", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_isProjectileAttack, GUIContent.none);
            GUILayout.EndHorizontal();

            if (_isProjectileAttack.boolValue)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("투사체 프리팹", GUILayout.Width(140));
                EditorGUILayout.PropertyField(_projectilePrefab, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("스폰 위치", GUILayout.Width(140));
                EditorGUILayout.PropertyField(_spawnPoint, GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("물리 관통력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_physicalPenetration, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("마법 관통력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_magicPenetration, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("치명타 확률", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_criticalHitChance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("치명타 데미지", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_criticalHitDamage, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("최대 체력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_maxHP, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("방어력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_physicalResistance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("마법저항력", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_magicResistance, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("초당 체력 회복량", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_hpRecoveryPerSec, GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("스킬", GUILayout.Width(140));
            EditorGUILayout.PropertyField(_skillTemplates, GUIContent.none);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        #region SkinList
        private void CreateSkinList()
        {
            _skinList = new ReorderableList(_target.skins, typeof(SkinTemplate), true, true, true, true)
            {
                headerHeight = 0,
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _target.skins[index];

                    rect.y += 5;
                    rect.width -= 10;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    _target.skins[index] = (SkinTemplate)EditorGUI.ObjectField(rect, _target.skins[index], typeof(SkinTemplate), false);

                    if (element != null)
                    {
                        rect.y += 20;
                        element.Draw(rect);

                        if (GUI.changed)
                        {
                            EditorUtility.SetDirty(element);
                        }
                    }
                },
                onSelectCallback = (list) =>
                {
                    if (list.index >= 0 && list.index < _target.skins.Count)
                    {
                        _currentSkin = _target.skins[list.index];
                    }
                },
                onAddDropdownCallback = (buttonRect, list) =>
                {
                    CreateSkin();
                },
                onRemoveCallback = (list) =>
                {
                    if (!EditorUtility.DisplayDialog("경고!", "이 스킨을 삭제하시겠습니까?", "네", "아니요")) return;

                    if (list.index >= 0 && list.index < _target.skins.Count)
                    {
                        var skin = _target.skins[list.index];
                        DeleteSkin(skin);
                        _currentSkin = null;
                        ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    }
                },
                elementHeightCallback = (index) =>
                {
                    var element = _target.skins[index];
                    if (element == null) return 30;

                    return 210;
                }
            };
        }

        private void CreateSkin()
        {
            SkinTemplate skin = CreateInstance<SkinTemplate>();
            SkinLobbyTemplate skinLobby = CreateInstance<SkinLobbyTemplate>();
            SkinBattleTemplate skinBattle = CreateInstance<SkinBattleTemplate>();

            var displayName = _displayName.stringValue;
            var defaultPath = $"Assets/EvolveThisMatch/GameData/Unit/Agent/Skin/{displayName}";
            var defaultFileName = $"Skin_{displayName}_";

            if (!AssetDatabase.IsValidFolder(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                AssetDatabase.Refresh();
            }

            string path = EditorUtility.SaveFilePanelInProject($"스킨 추가", defaultFileName, "asset", "", defaultPath);

            if (!string.IsNullOrEmpty(path))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                skin.SetDisplayName(fileName.Replace(defaultFileName, ""));
                _target.skins.Add(skin);

                AssetDatabase.CreateAsset(skin, path);
                AssetDatabase.CreateAsset(skinLobby, Path.Combine(defaultPath, $"{fileName}_Lobby.asset"));
                AssetDatabase.CreateAsset(skinBattle, Path.Combine(defaultPath, $"{fileName}_Battle.asset"));

                #region 어드레서블 등록
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                var group = settings.FindGroup("SkinResource");

                void SetAddressable(Object asset, string address, out AssetReference reference)
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    var entry = settings.CreateOrMoveEntry(guid, group);
                    entry.address = address;
                    reference = new AssetReference(guid);
                };

                SetAddressable(skinLobby, $"{fileName}_Lobby", out skin.lobbyResource);
                SetAddressable(skinBattle, $"{fileName}_Battle", out skin.battleResource);
                EditorUtility.SetDirty(settings);
                EditorUtility.SetDirty(skin);
                #endregion

                EditorUtility.SetDirty(_target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void DeleteSkin(SkinTemplate skin)
        {
            if (skin == null) return;

            #region 어드레서블
            void DeleteAsset(AssetReference reference)
            {
                if (reference == null || string.IsNullOrEmpty(reference.AssetGUID)) return;

                string assetPath = AssetDatabase.GUIDToAssetPath(reference.AssetGUID);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }


            DeleteAsset(skin.lobbyResource);
            DeleteAsset(skin.battleResource);
            #endregion

            _target.skins.Remove(skin);

            string skinPath = AssetDatabase.GetAssetPath(skin);
            if (!string.IsNullOrEmpty(skinPath))
            {
                AssetDatabase.DeleteAsset(skinPath);
            }

            DestroyImmediate(skin, true);
            EditorUtility.SetDirty(_target);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}
#endif