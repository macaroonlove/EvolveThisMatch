namespace ScriptableObjectArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(ObscuredIntVariable), true)]
    public class ObscuredIntVariableEditor : BaseVariableEditor
    {
        private SerializedProperty _displayName;
        private SerializedProperty _icon;
        private SerializedProperty _event;

        protected override void OnEnable()
        {
            base.OnEnable();

            _displayName = serializedObject.FindProperty("_displayName");
            _icon = serializedObject.FindProperty("_icon");
            _event = serializedObject.FindProperty("_event");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_displayName);
            EditorGUILayout.PropertyField(_icon);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_event);

            serializedObject.ApplyModifiedProperties();
        }
    }
}