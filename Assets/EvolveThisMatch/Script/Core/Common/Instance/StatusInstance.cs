using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class StatusInstance
    {
        public Coroutine corutine;
        public float duration;
        public float startTime;
        
        public bool useCountLimit = false;
        public int count = int.MaxValue;

        public StatusInstance(float duration, float time)
        {
            this.duration = duration;
            this.startTime = time;
        }

        public bool IsCompete => duration == 0 || Time.time - startTime > duration;

        /// <summary>
        /// 기존거가 더 오래된 경우 true
        /// </summary>
        internal bool IsOld(float duration)
        {
            if (this.duration == 0) return true;
            if (duration == 0) return false;

            return (this.duration - Time.time - startTime) < duration;
        }
    }
}
