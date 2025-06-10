using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class TextureExtensions
    {
        /// <summary>
        /// Texture2D의 크기를 리사이징
        /// </summary>
        public static Texture2D ResizeTexture(this Texture2D original, int width, int height)
        {
            // RenderTexture를 사용하여 리사이징을 처리
            RenderTexture rt = new RenderTexture(width, height, 24);
            Graphics.Blit(original, rt); 

            // RenderTexture를 텍스처2D로 변환
            Texture2D newTexture = new Texture2D(width, height);
            RenderTexture.active = rt;
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            newTexture.Apply();

            // RenderTexture의 후처리
            RenderTexture.active = null;
            rt.Release();

            return newTexture;
        }
    }

}
