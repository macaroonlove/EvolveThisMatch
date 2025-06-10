using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public abstract class DataEffect<T> : Effect
    {
        [SerializeField] protected T _value;

        public T value => _value;

#if UNITY_EDITOR

        public override int GetNumRows()
        {
            return 0;
        }

        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height);

            GUI.Label(labelRect, "값");
            if (typeof(T) == typeof(int))
            {
                _value = (T)(object)EditorGUI.IntField(valueRect, (int)(object)_value);
            }
            else if (typeof(T) == typeof(float))
            {
                _value = (T)(object)EditorGUI.FloatField(valueRect, (float)(object)_value);
            }
            // 이곳에 다른 타입에 대한 처리를 추가할 수 있습니다.
        }
#endif
    }
}