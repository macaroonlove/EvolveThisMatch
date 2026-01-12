using DG.Tweening.Core.Easing;
using EvolveThisMatch.Core;
using EvolveThisMatch.Save;
using System.Linq;
using UnityEngine;

namespace EvolveThisMatch.Lobby
{
    public sealed class UITomeEquipModel
    {
        private TomeEquipData[] _datas = new TomeEquipData[3];

        public int count => _datas.Length;

        public void InitializeEquipItem()
        {
            var ownedTomes = SaveManager.Instance.itemData.ownedTomes;
            var equipTomes = SaveManager.Instance.formationData.equipTomes;

            // 보유한 고서의 아이디
            var ownedTomeDic = ownedTomes.ToDictionary(a => a.id);

            for (int i = 0; i < count; i++)
            {
                // 장착한 상태라면
                if (equipTomes[i] != -1 && ownedTomeDic.TryGetValue(equipTomes[i], out var owned))
                {
                    var template = GameDataManager.Instance.GetTomeTemplateById(owned.id);
                    _datas[i] = new TomeEquipData(template, owned);
                }
            }
        }

        public int Equip(int index, TomeEquipData data)
        {
            int id = -1;
            if (_datas[index].template != null)
            {
                id = _datas[index].template.id;
            }
            _datas[index] = data;

            return id;
        }

        public TomeEquipData GetEquipData(int index)
        {
            if (index < 0 || index >= _datas.Length) return default;

            return _datas[index];
        }

        public TomeEquipItemViewState BuildState(int index)
        {
            if (index < 0 || index >= _datas.Length) return default;

            var data = _datas[index];

            Sprite icon = (data.template == null) ? null : data.template.sprite;

            return new TomeEquipItemViewState(icon);
        }
    }
}