using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 전투에서 사용하는 시간을 관리하는 클래스
    /// </summary>
    public class TimeSystem : MonoBehaviour, IBattleSystem
    {
        private float _startTime;

        public float currentTime => Time.time - _startTime;

        public void Initialize()
        {
            _startTime = Time.time;
        }

        public void Deinitialize()
        {

        }
    }
}