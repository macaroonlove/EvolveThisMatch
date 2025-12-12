using FrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class TileRangeRenderer : MonoBehaviour
    {
        private List<List<GameObject>> _tiles = new List<List<GameObject>>();

        internal IReadOnlyList<IReadOnlyList<GameObject>> tiles => _tiles;

        internal void Initialize()
        {
            _tiles.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var line = transform.GetChild(i);

                List<GameObject> row = new List<GameObject>();

                for (int j = 0; j < line.childCount; j++)
                {
                    var tile = line.GetChild(j).gameObject;
                    row.Add(tile);
                }

                _tiles.Add(row);
            }

            Hide();
        }

        /// <summary>
        /// 타일을 기준으로 보여주기
        /// </summary>
        internal void Show(Unit unit, TileRangeTemplate tileRangeTemplate)
        {
            var range = tileRangeTemplate.range;

            foreach (var point in range)
            {
                _tiles[point.x][point.y].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 라인을 기준으로 보여주기
        /// </summary>
        internal void Show(int lineCount)
        {
            for (int i = 0; i < lineCount; i++)
            {
                foreach (var tile in _tiles[i])
                {
                    tile.SetActive(true);
                }
            }
        }

        internal void Hide()
        {
            foreach (var line in _tiles)
            {
                foreach (var tile in line)
                {
                    tile.SetActive(false);
                }
            }
        }
    }
}