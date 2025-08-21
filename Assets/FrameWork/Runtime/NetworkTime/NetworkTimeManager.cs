using Cysharp.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.NetworkTime
{
    public class NetworkTimeManager : PersistentSingleton<NetworkTimeManager>
    {
        private DateTime _lastSyncedTime;
        private float _lastSyncLocalTime;

        protected override void Initialize()
        {
            _ = GetUtcNow();
        }

        #region 서버 시간 받아오기
        /// <summary>
        /// UTC 기준 현재 시간 가져오기 (NTP 우선 > HTTP fallback > 로컬)
        /// </summary>
        public async UniTask<DateTime> GetUtcNow()
        {
            // 이미 동기화된 경우, 로컬 오프셋 적용
            if (_lastSyncedTime != default)
            {
                double delta = Time.realtimeSinceStartup - _lastSyncLocalTime;
                return _lastSyncedTime.AddSeconds(delta);
            }

            // 1순위: NTP
            try
            {
                DateTime ntpTime = await GetTimeFromNtp();
                CacheTime(ntpTime);
                return ntpTime;
            }
            catch 
            {
#if UNITY_EDITOR
                Debug.LogWarning("NTP 실패 → HTTP로 fallback");
#endif
            }

            // 2순위: HTTP
            try
            {
                DateTime httpTime = await GetTimeFromHTTP();
                CacheTime(httpTime);
                return httpTime;
            }
            catch 
            {
#if UNITY_EDITOR
                Debug.LogWarning("HTTP 실패 → 로컬 시간 사용");
#endif
            }

            return DateTime.UtcNow;
        }

        #region 한국 시간
        public async UniTask<DateTime> GetKoreanNow()
        {
            var utcNow = await GetUtcNow();
            var koreanZone = GetKoreanTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, koreanZone);
        }

        private TimeZoneInfo GetKoreanTimeZone()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
#else
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul");
#endif
        }
        #endregion

        private void CacheTime(DateTime utcNow)
        {
            _lastSyncedTime = utcNow;
            _lastSyncLocalTime = Time.realtimeSinceStartup;
        }

        #region NTP로 시간 받아오기
        private async UniTask<DateTime> GetTimeFromNtp()
        {
            return await UniTask.RunOnThreadPool(() =>
            {
                const int ntpPort = 123;
                byte[] ntpData = new byte[48];
                ntpData[0] = 0x1B;

                // 서버 IP 가져오기
                var addresses = Dns.GetHostEntry("time.google.com").AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], ntpPort);

                using (var socket = new UdpClient())
                {
                    // NTP 요청 전송
                    socket.Send(ntpData, ntpData.Length, ipEndPoint);
                    var response = socket.Receive(ref ipEndPoint);

                    // 서버 시간 추출
                    const byte serverReplyTime = 40;
                    ulong intPart = BitConverter.ToUInt32(response, serverReplyTime);
                    ulong fractPart = BitConverter.ToUInt32(response, serverReplyTime + 4);
                    intPart = SwapEndianness(intPart);
                    fractPart = SwapEndianness(fractPart);

                    // DateTime으로 변환
                    ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                    DateTime ntpEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    return ntpEpoch.AddMilliseconds((long)milliseconds);
                }
            });
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                          ((x & 0x0000ff00) << 8) +
                          ((x & 0x00ff0000) >> 8) +
                          ((x & 0xff000000) >> 24));
        }
        #endregion

        #region HTTP로 시간 받아오기
        private async UniTask<DateTime> GetTimeFromHTTP()
        {
            using (UnityWebRequest req = UnityWebRequest.Get("https://timeapi.io/api/Time/current/zone?timeZone=UTC"))
            {
                await req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    throw new Exception("HTTP 시간 API 실패");

                // 시간 받아오기 (DateTime을 JSON으로 받아옴 => 역직렬화)
                string json = req.downloadHandler.text;
                var time = JsonUtility.FromJson<TimeApiResponse>(json);

                // UTC 기준의 DateTime으로 변환
                return DateTime.Parse(time.dateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
        }

        [Serializable]
        private class TimeApiResponse { public string dateTime; }
        #endregion
        #endregion

        #region 남은 시간 받아오기
        /// <summary>
        /// Minute: 마지막 구매 시간으로부터 남은 시간
        /// 나머지: 매일/매주/매달 12시 기준으로 남은 시간
        /// </summary>
        /// <returns>양수라면 남은 시간 존재/음수라면 남은 시간 없음</returns>
        public async UniTask<TimeSpan> GetRemainTime(DateTime lastBuyTime, ECycleType cycleType, int interval)
        {
            var now = await GetKoreanNow();

            DateTime nextResetTime = lastBuyTime;

            switch (cycleType)
            {
                case ECycleType.Minute:
                    nextResetTime = lastBuyTime.AddMinutes(interval);
                    break;
                case ECycleType.Daily:
                    nextResetTime = now.Date.AddDays(interval);
                    break;
                case ECycleType.Weekly:
                    int monday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    
                    if (monday == 0) monday = 7;

                    nextResetTime = now.Date.AddDays(monday + (7 * (interval - 1)));
                    break;
                case ECycleType.Monthly:
                    nextResetTime = new DateTime(now.Year, now.Month, 1).AddMonths(interval).Date;
                    break;
            }

            return nextResetTime - now;
        }
        #endregion
    }

    public enum ECycleType
    {
        Minute,
        Daily,
        Weekly,
        Monthly,
    }
}