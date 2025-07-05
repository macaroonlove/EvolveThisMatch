using UnityEngine;

namespace FrameWork
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Vector2 외적은 스칼라 값으로 반환됨
        /// </summary>
        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}