using CodeStage.AntiCheat.ObscuredTypes;
using FrameWork.Editor;
using FrameWork.PlayFabExtensions;
using ScriptableObjectArchitecture;
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

    [CreateAssetMenu(menuName = "Templates/SaveData/ItemSaveData", fileName = "ItemSaveData", order = 2)]
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

            TitleDataManager.LoadItemData(ref _artifactLevelUpRequirements, ref _tomeLevelUpRequirements);

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

            _artifactDataChangedGameEvent?.Raise();
            _tomeDataChangedGameEvent?.Raise();

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
        #region 아티팩트 추가 (로컬)
        [Header("아티팩트 데이터 변경 이벤트")]
        [SerializeField] private GameEvent _artifactDataChangedGameEvent;

        private int _artifactBatchDepth = 0;
        private bool _isArtifactDataChange = false;

        /// <summary>
        /// 아티팩트 추가
        /// </summary>
        public void AddArtifact(int id, int count = 1)
        {
            if (count <= 0) return;

            var modifyArtifact = FindArtifact(_data.ownedArtifacts, id);

            if (modifyArtifact == null)
            {
                var newArtifact = new ItemSaveData.Artifact(id);
                newArtifact.count = count;
                _data.ownedArtifacts.Add(newArtifact);
            }
            else
            {
                modifyArtifact.count += count;

                TryLevelupArtifact(modifyArtifact);
            }

            if (_artifactBatchDepth > 0)
                _isArtifactDataChange = true;
            else
                _artifactDataChangedGameEvent?.Raise();
        }

        #region 아티팩트 추가 묶어 보내기
        public void BeginAddAartifactBatch()
        {
            _artifactBatchDepth++;
        }

        public void EndAddArtifactBatch()
        {
            if (_artifactBatchDepth <= 0)
            {
                _artifactBatchDepth = 0;
                return;
            }

            _artifactBatchDepth--;

            if (_artifactBatchDepth == 0 && _isArtifactDataChange)
            {
                _isArtifactDataChange = false;
                _artifactDataChangedGameEvent?.Raise();
            }
        }
        #endregion
        #endregion

        #region 레벨업
        /// <summary>
        /// 레벨업 시도
        /// </summary>
        private void TryLevelupArtifact(ItemSaveData.Artifact modifyArtifact)
        {
            if (modifyArtifact == null) return;

            while (modifyArtifact.level < _artifactLevelUpRequirements.Length - 1)
            {
                int requiredCount = _artifactLevelUpRequirements[modifyArtifact.level];

                if (modifyArtifact.count >= requiredCount)
                {
                    modifyArtifact.count -= requiredCount;
                    modifyArtifact.level++;
                }
                else
                {
                    break;
                }
            }
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
        /// <summary>
        /// 아티팩트 찾기
        /// </summary>
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
        #region 고서 추가 (로컬)
        [Header("고서 데이터 변경 이벤트")]
        [SerializeField] private GameEvent _tomeDataChangedGameEvent;

        private int _tomeBatchDepth = 0;
        private bool _isTomeDataChange = false;

        /// <summary>
        /// 고서 추가
        /// </summary>
        public void AddTome(int id, int count = 1)
        {
            if (count <= 0) return;

            var modifyTome = FindTome(_data.ownedTomes, id);

            if (modifyTome == null)
            {
                var newTome = new ItemSaveData.Tome(id);
                newTome.count = count;
                _data.ownedTomes.Add(newTome);
            }
            else
            {
                modifyTome.count += count;

                TryLevelupTome(modifyTome);
            }

            if (_tomeBatchDepth > 0)
                _isTomeDataChange = true;
            else
                _tomeDataChangedGameEvent?.Raise();
        }

        #region 아티팩트 추가 묶어 보내기
        public void BeginAddTomeBatch()
        {
            _tomeBatchDepth++;
        }

        public void EndAddTomeBatch()
        {
            if (_tomeBatchDepth <= 0)
            {
                _tomeBatchDepth = 0;
                return;
            }

            _tomeBatchDepth--;

            if (_tomeBatchDepth == 0 && _isTomeDataChange)
            {
                _isTomeDataChange = false;
                _tomeDataChangedGameEvent?.Raise();
            }
        }
        #endregion
        #endregion

        #region 레벨업
        /// <summary>
        /// 레벨업 시도
        /// </summary>
        private void TryLevelupTome(ItemSaveData.Tome modifyTome)
        {
            if (modifyTome == null) return;

            while (modifyTome.level < _tomeLevelUpRequirements.Length - 1)
            {
                int requiredCount = _tomeLevelUpRequirements[modifyTome.level];

                if (modifyTome.count >= requiredCount)
                {
                    modifyTome.count -= requiredCount;
                    modifyTome.level++;
                }
                else
                {
                    break;
                }
            }
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
        #endregion

        #region 유틸리티
        /// <summary>
        /// 고서 찾기
        /// </summary>
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