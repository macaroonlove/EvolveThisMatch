using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HeaderColorAttribute))]
    public class HeaderColorAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            HeaderColorAttribute headerColorAttribute = (HeaderColorAttribute)attribute;

            // 원본 색 캐싱
            Color originalColor = GUI.contentColor;

            // 원하는 색으로 바꾸기
            GUI.contentColor = headerColorAttribute.color;

            // y축 위치를 +5
            position.y += 5;

            // 굵은 글씨체로 라벨 필드 그려주기
            EditorGUI.LabelField(position, headerColorAttribute.header, EditorStyles.boldLabel);

            // 다음 필드부터 다시 원본 색을 주기
            GUI.contentColor = originalColor;
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight + 10f;
        }
    }
#endif
}