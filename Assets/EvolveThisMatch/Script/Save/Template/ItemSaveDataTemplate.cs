using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Save
{
    [Serializable]
    public class ItemSaveData
    {
        [Tooltip("보유하고 있는 아티팩트들")]
        public List<Artifact> ownedArtifacts = new List<Artifact>();

        [Tooltip("보유하고 있는 고서들")]
        public List<Tome> ownedTomes = new List<Tome>();
        public int[] equipTomes = new int[] { -1, -1, -1 };

        #region 데이터 모델
        #region 아티팩트
        [Serializable]
        public class Artifact
        {
            public int id;
            public int count;
            public int level;

            public Artifact(int id)
            {
                this.id = id;
                this.count = 0;
                this.level = 1;
            }
        }
        #endregion

        #region 고서
        [Serializable]
        public class Tome
        {
            public int id;
            public int count;
            public int level;

            public Tome(int id)
            {
                this.id = id;
                this.count = 0;
                this.level = 1;
            }
        }
        #endregion
        #endregion
    }

    [CreateAssetMenu(menuName = "Templates/SaveData/ItemSaveData", fileName = "ItemSaveData", order = 0)]
    public class ItemSaveDataTemplate : SaveDataTemplate
    {
        [SerializeField, ReadOnly] private ItemSaveData _data;

        public bool isLoaded { get; private set; }

        public List<ItemSaveData.Artifact> ownedArtifacts => _data.ownedArtifacts;
        public List<ItemSaveData.Tome> ownedTomes => _data.ownedTomes;
        public int[] equipTomes => _data.equipTomes;

        public override void SetDefaultValues()
        {
            _data = new ItemSaveData();

            // 임시 아티팩트 추가
            for (int i = 0; i < 21; i++)
            {
                AddArtifact(i);
            }

            // 임시 고서 추가
            for (int i = 0; i < 4; i++)
            {
                AddTome(i);
            }

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ItemSaveData>(json);

            if (_data != null)
            {
                isLoaded = true;
            }

            return isLoaded;
        }

        public override string ToJson()
        {
            if (_data == null) return null;

            return JsonUtility.ToJson(_data);
        }

        public override void Clear()
        {
            _data = null;
            isLoaded = false;
        }

        #region 아티팩트
        /// <summary>
        /// 아티팩트 추가
        /// </summary>
        public void AddArtifact(int id, int count = 1)
        {
            if (count <= 0) return;

            var modifyArtifact = FindArtifact(_data.ownedArtifacts, id);

            // 아티팩트가 없었다면 추가
            if (modifyArtifact == null)
            {
                var newArtifact = new ItemSaveData.Artifact(id);
                newArtifact.count = count;
                _data.ownedArtifacts.Add(newArtifact);
            }
            // 아티팩트가 있었다면 개수 추가
            else
            {
                modifyArtifact.count += count;

                // 레벨업 시도
                TryLevelupArtifact(modifyArtifact);
            }
        }

        #region 레벨업
        /// <summary>
        /// 아티팩트가 레벨업하는데 요구하는 개수
        /// </summary>
        private static readonly ObscuredInt[] _artifactLevelUpRequirements = { 0, 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };

        /// <summary>
        /// 아티팩트 레벨업 시도
        /// </summary>
        private bool TryLevelupArtifact(ItemSaveData.Artifact modifyArtifact)
        {
            // 아티팩트가 있다면 && 최고 레벨이 아니라면
            if (modifyArtifact != null && modifyArtifact.level < _artifactLevelUpRequirements.Length - 1)
            {
                int requiredCount = _artifactLevelUpRequirements[modifyArtifact.level];

                if (modifyArtifact.count >= requiredCount)
                {
                    modifyArtifact.count -= requiredCount;
                    modifyArtifact.level++;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 아티팩트 레벨에 따른 최대 요구 개수 반환
        /// </summary>
        public int GetMaxArtifactCountByLevel(int level)
        {
            if (level < 0 || level >= _artifactLevelUpRequirements.Length)
                return -1;

            return _artifactLevelUpRequirements[level];
        }
        #endregion

        #region 유틸리티
        private ItemSaveData.Artifact FindArtifact(List<ItemSaveData.Artifact> artifacts, int artifactId)
        {
            for (int i = 0; i < artifacts.Count; i++)
            {
                if (artifacts[i].id == artifactId)
                {
                    return artifacts[i];
                }
            }
            return null;
        }
        #endregion
        #endregion

        #region 고서
        /// <summary>
        /// 고서 추가
        /// </summary>
        public void AddTome(int id, int count = 1)
        {
            if (count <= 0) return;

            var modifyTome = FindTome(_data.ownedTomes, id);

            // 고서가 없었다면 추가
            if (modifyTome == null)
            {
                var newTome = new ItemSaveData.Tome(id);
                newTome.count = count;
                _data.ownedTomes.Add(newTome);
            }
            // 고서가 있었다면 개수 추가
            else
            {
                modifyTome.count += count;

                // 레벨업 시도
                TryLevelupTome(modifyTome);
            }
        }

        /// <summary>
        /// 고서 장착
        /// </summary>
        public void EquipTome(int id, int index)
        {
            _data.equipTomes[index] = id;
        }

        #region 레벨업
        /// <summary>
        /// 고서가 레벨업하는데 요구하는 개수
        /// </summary>
        private static readonly ObscuredInt[] _tomeLevelUpRequirements = { 0, 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };

        /// <summary>
        /// 고서 레벨업 시도
        /// </summary>
        private bool TryLevelupTome(ItemSaveData.Tome modifyTome)
        {
            // 아티팩트가 있다면 && 최고 레벨이 아니라면
            if (modifyTome != null && modifyTome.level < _tomeLevelUpRequirements.Length - 1)
            {
                int requiredCount = _tomeLevelUpRequirements[modifyTome.level];

                if (modifyTome.count >= requiredCount)
                {
                    modifyTome.count -= requiredCount;
                    modifyTome.level++;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 아티팩트 레벨에 따른 최대 요구 개수 반환
        /// </summary>
        public int GetMaxTomeCountByLevel(int level)
        {
            if (level < 0 || level >= _tomeLevelUpRequirements.Length)
                return -1;

            return _tomeLevelUpRequirements[level];
        }
        #endregion

        #region 유틸리티
        private ItemSaveData.Tome FindTome(List<ItemSaveData.Tome> tomes, int tomeId)
        {
            for (int i = 0; i < tomes.Count; i++)
            {
                if (tomes[i].id == tomeId)
                {
                    return tomes[i];
                }
            }
            return null;
        }
        #endregion
        #endregion
    }
}