using System;
using UnityEngine;

namespace FrameWork.PlayFabExtensions
{
    public class Random
    {
        private ulong state;

        public Random(int seed)
        {
            if (seed == 0)
            {
                throw new ArgumentException("시드는 0이 될 수 없음.", nameof(seed));
            }
            this.state = (ulong)seed;
        }

        private ulong NextULong()
        {
            ulong x = this.state;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            this.state = x & 0xFFFFFFFFFFFFFFFFUL; // 64비트 마스킹
            return this.state;
        }

        /// <summary>
        /// 최소값(포함)과 최대값(포함) 사이의 정수를 반환
        /// </summary>
        public int Next(int min, int max)
        {
            long range = (long)max - min + 1;
            ulong randomUlong = this.NextULong();
            var result = (int)((randomUlong % (ulong)range) + (ulong)min);
            return result;
        }

        /// <summary>
        /// 0과 1 사이의 부동 소수점 난수를 반환
        /// </summary>
        public double NextZeroOne()
        {
            // 상위 53비트를 사용
            return (double)(this.NextULong() >> 11) / (1UL << 53);
        }
    }
}