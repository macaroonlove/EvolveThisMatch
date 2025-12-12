using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    [CreateAssetMenu(menuName = "Templates/TileRange", fileName = "TileRange", order = 0)]
    public class TileRangeTemplate : ScriptableObject
    {
        [ContextMenuItem("Sort", "SortRange")]
        [SerializeField] private List<Vector2Int> _range = new List<Vector2Int>();

        public IReadOnlyList<Vector2Int> range => _range;

        private void SortRange()
        {
            //sort range
            _range.Sort((a, b) =>
            {
                return a.sqrMagnitude - b.sqrMagnitude;
            });
        }

        public int GetHeight()
        {
            if (range.Count == 0) return 0;

            //get max y value in range
            int maxY = int.MinValue;
            foreach (var item in range)
            {
                maxY = Math.Max(maxY, Math.Abs(item.y));
            }
            return maxY * 2 + 1;
        }

        public void Add(int v1, int v2)
        {
            if (IsContains(v1, v2) == false)
            {
                _range.Add(new Vector2Int(v1, v2));
                SortRange();
            }
        }

        public void Remove(int v1, int v2)
        {
            for (int i = 0; i < range.Count; i++)
            {
                if (range[i].x == v1 && range[i].y == v2)
                {
                    _range.RemoveAt(i);
                    break;
                }
            }
        }

        public bool IsContains(int v1, int v2)
        {
            foreach (var item in range)
            {
                if (item.x == v1 && item.y == v2)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsContains(Vector2Int vec)
        {
            foreach (var item in range)
            {
                if (item.x == vec.x && item.y == vec.y)
                {
                    return true;
                }
            }
            return false;
        }

        public void GetMinMaxWidth(out int minX, out int maxX)
        {
            minX = 0;
            maxX = 0;
            if (range.Count == 0) return;

            foreach (var item in range)
            {
                minX = Math.Min(minX, item.x);
                maxX = Math.Max(maxX, item.x);
            }
        }
    }
}