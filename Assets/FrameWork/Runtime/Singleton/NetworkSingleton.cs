//using Photon.Pun;
//using UnityEngine;

//namespace FrameWork
//{
//    public class NetworkSingleton<T> : MonoBehaviourPunCallbacks where T : NetworkSingleton<T>
//    {
//        protected static T instance;
//        public static T Instance
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    instance = new GameObject(nameof(T)).AddComponent<T>();
//                    instance.Initialize();
//                }
//                return instance;
//            }
//        }

//        protected virtual void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this as T;
//                instance.Initialize();
//            }
//        }

//        protected virtual void Start()
//        {
//            if (instance != this)
//                Destroy(gameObject);
//        }

//        protected virtual void Initialize()
//        {

//        }
//    }
//}