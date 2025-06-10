using UnityEngine;

namespace FrameWork
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T instance;
        
        public static T Instance => instance;
        public static T GetOrCreateInstance
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                    instance.Initialize();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Initialize()
        {

        }

        private static bool isApplicationQuitting = false;
        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}