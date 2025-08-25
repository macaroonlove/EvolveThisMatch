using Cysharp.Threading.Tasks;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EvolveThisMatch.Lobby
{
    [Serializable]
    public class AgentSkinGachaData : GachaData
    {
        [Serializable]
        public class PickUpData
        {
            public SkinTemplate template;
            public float rarity;
        }

        [Header("아군 스킨 등급별 확률정보")]
        [SerializeField, Range(0, 100.0f)] private int _legendRarity;
        [SerializeField, Range(0, 100.0f)] private int _epicRarity;
        [SerializeField, Range(0, 100.0f)] private int _rareRarity;
        [SerializeField, Range(0, 100.0f)] private int _commonRarity;

        [Header("픽업 스킨"), Tooltip("픽업 스킨은 등급별 확률에서 제외됩니다.")]
        [SerializeField] private List<PickUpData> _pickupList = new List<PickUpData>();

        private float[] _rarityProbabilities;
        private List<SkinTemplate> _legendSkins;
        private List<SkinTemplate> _epicSkins;
        private List<SkinTemplate> _rareSkins;
        private List<SkinTemplate> _commonSkins;

        private List<SkinTemplate> _gachaList = new List<SkinTemplate>();
        private List<PickUpData> _legentPickUpList = new List<PickUpData>();

        internal override async void Initialize(UIGachaResultCanvas gachaResultCanvas)
        {
            await UniTask.WaitUntil(() => PersistentLoad.isLoaded);

            base.Initialize(gachaResultCanvas);

            _rarityProbabilities = new float[4];
            _rarityProbabilities[0] = _legendRarity;
            _rarityProbabilities[1] = _epicRarity;
            _rarityProbabilities[2] = _rareRarity;
            _rarityProbabilities[3] = _commonRarity;

            if (GameDataManager.Instance.agentSkinTemplates.Count == 0) return;

            // 픽업 스킨을 제외한 스킨 리스트
            var skinList = GameDataManager.Instance.agentSkinTemplates.Where(x => !_pickupList.Any(y => y.template == x.Key)).Select(x => x.Key);

            _legentPickUpList = _pickupList.Where(p => p.template.rarity.rarity == ESkinRarity.Legend).ToList();
            _legendSkins = skinList.Where(template => template.rarity.rarity == ESkinRarity.Legend).ToList();
            _epicSkins = skinList.Where(template => template.rarity.rarity == ESkinRarity.Epic).ToList();
            _rareSkins = skinList.Where(template => template.rarity.rarity == ESkinRarity.Rare).ToList();
            _commonSkins = skinList.Where(template => template.rarity.rarity == ESkinRarity.Common).ToList();
        }

        internal override void PickUp(int gachaCount)
        {
            _gachaList.Clear();

            for (int i = 0; i < gachaCount; i++)
            {
                var skin = GetRandomSkin();
                _gachaList.Add(skin);
                var agentTemplate = GameDataManager.Instance.agentSkinTemplates[skin];
                SaveManager.Instance.agentData.AddAgentSkin(agentTemplate.id, skin.id);
            }

            _ = SaveManager.Instance.SaveData(SaveKey.Profile);

            _gachaResultCanvas.Show(_gachaList);
        }

        private SkinTemplate GetRandomSkin()
        {
            int rarityIndex;

            if (_confirmedPickUpVariable != null)
            {
                if (_confirmedPickUpVariable.Value > 0)
                {
                    _confirmedPickUpVariable.AddValue(-1);

                    var skinTemplate = GetPickUp(_pickupList, 100);

                    if (skinTemplate != null) return skinTemplate;

                    rarityIndex = GetRandomRarityIndex();
                }
                else
                {
                    _confirmedPickUpVariable.Value = 50;

                    var skinTemplate = GetPickUp(_legentPickUpList, 100);

                    if (skinTemplate != null) return skinTemplate;

                    rarityIndex = 0;
                }
            }
            else
            {
                var skinTemplate = GetPickUp(_pickupList, 100);

                if (skinTemplate != null) return skinTemplate;

                rarityIndex = GetRandomRarityIndex();
            }

            _additionalVariable.AddValue(1);

            // 일반 유닛 뽑기
            List<SkinTemplate> skins;
            switch (rarityIndex)
            {
                case 0: skins = _legendSkins; break;
                case 1: skins = _epicSkins; break;
                case 2: skins = _rareSkins; break;
                default: skins = _commonSkins; break;
            }

            var index = Random.Range(0, skins.Count);
            return skins[index];
        }

        private SkinTemplate GetPickUp(List<PickUpData> pickUpList, float maxProbability)
        {
            // 픽업 유닛 뽑기
            float rand = Random.Range(0, maxProbability);
            float cumulativeProbability = 0;

            foreach (var pickUp in pickUpList)
            {
                cumulativeProbability += pickUp.rarity;
                if (rand <= cumulativeProbability)
                {
                    return pickUp.template;
                }
            }

            return null;
        }

        private int GetRandomRarityIndex()
        {
            float rand = Random.Range(0, 100.0f);
            float cumulativeProbability = 0;

            for (int i = 0; i < _rarityProbabilities.Length; i++)
            {
                cumulativeProbability += _rarityProbabilities[i];
                if (rand <= cumulativeProbability)
                {
                    return i;
                }
            }

            return _rarityProbabilities.Length - 1;
        }
    }
}