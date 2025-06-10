using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace FrameWork
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                var temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

        public static T Min<T>(this List<T> list) where T : IComparable<T>
        {
            if (list.Count == 0) return default(T);

            T min = list[0];
            foreach (var item in list)
            {
                if (item.CompareTo(min) < 0)
                {
                    min = item;
                }
            }

            return min;
        }

        public static T Max<T>(this List<T> list) where T : IComparable<T>
        {
            if (list.Count == 0) return default(T);

            T max = list[0];
            foreach (var item in list)
            {
                if (item.CompareTo(max) > 0)
                {
                    max = item;
                }
            }

            return max;
        }
    }
}