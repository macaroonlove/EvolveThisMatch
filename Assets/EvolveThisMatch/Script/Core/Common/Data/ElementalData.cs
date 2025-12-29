using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class ElementalData
    {
        public int level { get; private set; }

        private int _pendingToken;
        private int _pendingFrame;

        public int PrepareElementalIncrease()
        {
            _pendingToken = Random.Range(int.MinValue, int.MaxValue);
            _pendingFrame = Time.frameCount;

            return _pendingToken;
        }

        public int ApplyElementalIncrease(int token)
        {
            // 비정상적인 속성 레벨업을 시도하고 있습니다.
            if (token != _pendingToken) return -1;
            if (Time.frameCount != _pendingFrame) return -1;

            // 최대 레벨
            if (level >= 5) return -2;

            level++;
            _pendingToken = 0;

            return 1;
        }
    }
}