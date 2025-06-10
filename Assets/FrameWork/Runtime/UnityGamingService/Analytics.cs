using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace FrameWork.UGS
{
    public class Analytics : PersistentSingleton<Analytics>
    {
        private bool isInit;

        private void Start()
        {
            if (Instance.isInit == false)
            {
                Instance.InitializeUGS();
            }
        }

        private async void InitializeUGS()
        {
            try
            {
                await UnityServices.InitializeAsync();
#if UNITY_EDITOR
                Debug.Log($"사용자 ID로 UGS Analytics를 시작했습니다: {AnalyticsService.Instance.GetAnalyticsUserID()}");
#endif

                isInit = true;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public static void Record(string eventName, Dictionary<string, object> parameters)
        {
            if (Instance.isInit == false)
            {
                Instance.InitializeUGS();
            }

            var customEvent = new CustomEvent(eventName);

            foreach (var parameter in parameters)
            {
                customEvent.Add(parameter.Key, parameter.Value);
            }

            try
            {
                AnalyticsService.Instance.RecordEvent(customEvent);
#if UNITY_EDITOR
                Debug.Log($"기록 완료.\nEventName: {eventName}");
#endif
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"기록 실패.\n오류내용: {e.Message}");
#endif
            }
        }
    }
}