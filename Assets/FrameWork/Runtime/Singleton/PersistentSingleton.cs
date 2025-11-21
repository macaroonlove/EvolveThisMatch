using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FrameWork
{
    public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        protected static T instance;
        private bool isInitialize = false;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();

                    if (instance == null)
                    {
                        AutoLoadAsync();
                    }
                }
                return instance;
            }
        }

        #region 자동 로딩
        public static UniTask AutoLoadTask => AutoLoad();

        private static void AutoLoadAsync()
        {
            AutoLoad().Forget();
        }

        private static async UniTask AutoLoad()
        {
            if (instance != null) return;

            string key = typeof(T).Name;

            // 어드레서블에 키가 존재한다면
            var locations = await Addressables.LoadResourceLocationsAsync(key).ToUniTask();

            if (locations.Count > 0)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);

                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var obj = Instantiate(handle.Result);
                    instance = obj.GetComponent<T>();
                }
                else
                {
                    CraeteNewInstance();
                }
            }
            else
            {
                CraeteNewInstance();
            }

            if (!instance.isInitialize)
            {
                instance.Initialize();
                instance.SetupDontDestroy();

                instance.isInitialize = true;
            }
        }

        private static void CraeteNewInstance()
        {
            GameObject newInstance = new GameObject(typeof(T).Name);
            instance = newInstance.AddComponent<T>();
        }
        #endregion

        protected virtual void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this as T;
                if (!instance.isInitialize)
                {
                    instance.Initialize();
                    instance.SetupDontDestroy();

                    instance.isInitialize = true;
                }
            }
            else
            {
                if (this != instance)
                {
#if UNITY_EDITOR
                    Debug.Log($"{typeof(T).Name} : 이미 싱글톤이 존재하여 삭제됩니다.");
#endif
                    Destroy(gameObject);
                }
            }
        }

        private void SetupDontDestroy()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                transform.SetParent(null);
#if UNITY_EDITOR
                Debug.LogError($"{typeof(T).Name} : 파괴되지 않는 오브젝트는 부모가 있으면 씬 전환 시 삭제될 수 있습니다.");
#endif
            }
        }

        protected virtual void Initialize()
        {

        }
    }
}