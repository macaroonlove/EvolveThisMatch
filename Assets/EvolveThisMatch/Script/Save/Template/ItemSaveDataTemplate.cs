using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
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

        private static ObscuredInt[] _artifactLevelUpRequirements = { 0, 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };
        private static ObscuredInt[] _tomeLevelUpRequirements = { 0, 1, 3, 5, 7, 10, 15, 30, 50, 90, 150 };

        public List<ItemSaveData.Artifact> ownedArtifacts => _data.ownedArtifacts;
        public List<ItemSaveData.Tome> ownedTomes => _data.ownedTomes;

        public override void SetDefaultValues()
        {
            _data = new ItemSaveData();

            isLoaded = true;
        }

        public override bool Load(string json)
        {
            _data = JsonUtility.FromJson<ItemSaveData>(json);

            if (_data != null)
            {
                isLoaded = true;

                TitleDataManager.LoadItemData(ref _artifactLevelUpRequirements, ref _tomeLevelUpRequirements);
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

        /// <summary>
        /// 아티팩트 레벨에 따른 최대 요구 개수 반환
        /// </summary>
        public int GetMaxArtifactCountByLevel(int level)
        {
            if (level < 0 || level >= _artifactLevelUpRequirements.Length)
                return -1;

            return _artifactLevelUpRequirements[level];
        }

        /// <summary>
        /// 고서 레벨에 따른 최대 요구 개수 반환
        /// </summary>
        public int GetMaxTomeCountByLevel(int level)
        {
            if (level < 0 || level >= _tomeLevelUpRequirements.Length)
                return -1;

            return _tomeLevelUpRequirements[level];
        }
    }
}