using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork
{
    [CreateAssetMenu(menuName = "Templates/TileRange", fileName = "TileRange", order = 0)]
    public class TileRangeTemplate : ScriptableObject
    {
        [ContextMenuItem("Sort", "SortRange")]
        public List<Vector2Int> range = new List<Vector2Int>();

        void SortRange()
        {
            //sort range
            range.Sort((a, b) =>
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
                range.Add(new Vector2Int(v1, v2));
                SortRange();
            }
        }

        public void Remove(int v1, int v2)
        {
            for (int i = 0; i < range.Count; i++)
            {
                if (range[i].x == v1 && range[i].y == v2)
                {
                    range.RemoveAt(i);
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

        //internal List<Vector2Int> GetOrientedList(EUnitDirection direction)
        //{
        //    var list = new List<Vector2Int>();
        //    foreach (var item in range)
        //    {
        //        var dir = new Vector3(item.x, 0, item.y);
        //        switch (direction)
        //        {
        //            case EUnitDirection.Right:
        //                list.Add(item);
        //                break;
        //            case EUnitDirection.Bottom:
        //                {
        //                    Vector3 vector = Quaternion.AngleAxis(90, Vector3.up) * dir;
        //                    list.Add(new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z)));
        //                }
        //                break;
        //            case EUnitDirection.Left:
        //                {
        //                    Vector3 vector = Quaternion.AngleAxis(180, Vector3.up) * dir;
        //                    list.Add(new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z)));
        //                }
        //                break;
        //            case EUnitDirection.Top:
        //                {
        //                    Vector3 vector = Quaternion.AngleAxis(-90, Vector3.up) * dir;
        //                    list.Add(new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z)));
        //                }
        //                break;
        //        }
        //    }
        //    return list;
        //}

#if UNITY_EDITOR
        public void ExportJson(bool isRefresh = true)
        {
            var json = JsonUtility.ToJson(this, true);
            var path = $"Assets/StreamingAssets/AgentOverride/{name}_Range.json";
            File.WriteAllText(path, json);
            if (isRefresh)
            {
                AssetDatabase.Refresh();
            }
            Debug.Log($"파일 생성 : {path}");
        }

#endif

        internal void DEBUG_Override(TileRangeTemplate newTemplate)
        {
            this.range = newTemplate.range;
        }
    }
}