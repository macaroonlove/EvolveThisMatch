using UnityEngine;

namespace FrameWork.UIBinding
{
    public class Utils
    {
        internal static GameObject FindChild(GameObject obj, string name, bool recursive)
        {
            Transform transform = FindChild<Transform>(obj, name, recursive);
            if (transform != null) return transform.gameObject;
            else return null;
        }

        internal static T FindChild<T>(GameObject obj, string name, bool recursive) where T : UnityEngine.Object
        {
            if (obj == null) return null;

            if (!recursive)
            {
                Transform transform = obj.transform.Find(name);
                if (transform != null) return transform.GetComponent<T>();
            }
            else
            {
                foreach (T component in obj.GetComponentsInChildren<T>())
                    if (string.IsNullOrEmpty(name) || component.name.StartsWith(name))
                        return component;
            }
            return null;
        }

         internal static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T res = obj.GetComponent<T>();
            return res != null ? res : obj.AddComponent<T>();
        }
    }
}
