using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork
{
    /// <summary>
    /// 게임 시작 시, PeristentSingleton을 상속받은 클래스를 로드하는 클래스
    /// </summary>
    public class PersistentLoad
    {
        public static bool isLoaded = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void AutoLoadAll()
        {
            var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.activeSelf).ToArray();

            foreach (var obj in gameObjects)
            {
                obj.SetActive(false);
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<UniTask> loadTasks = new List<UniTask>();

            foreach (var assembly in assemblies)
            {
                var singletonTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType)
                .Where(t => t.BaseType.GetGenericTypeDefinition() == typeof(PersistentSingleton<>))
                .ToList();

                foreach (var type in singletonTypes)
                {
                    PropertyInfo instanceProperty = type.GetProperty("AutoLoadTask", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    if (instanceProperty != null)
                    {
                        var task = (UniTask)instanceProperty.GetValue(null);
                        loadTasks.Add(task);
                    }
                }
            }

            await UniTask.WhenAll(loadTasks);

            isLoaded = true;

            await UniTask.Yield();

            foreach (var obj in gameObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}