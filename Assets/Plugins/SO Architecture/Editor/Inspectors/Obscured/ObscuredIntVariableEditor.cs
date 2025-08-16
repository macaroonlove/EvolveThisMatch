namespace ScriptableObjectArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(ObscuredIntVariable), true)]
    public class ObscuredIntVariableEditor : BaseVariableEditor
    {
        private SerializedProperty _displayName;
        private SerializedProperty _iconText;
        private SerializedProperty _icon;
        private SerializedProperty _iconBG;
        private SerializedProperty _event;

        protected override void OnEnable()
        {
            base.OnEnable();

            _displayName = serializedObject.FindProperty("_displayName");
            _iconText = serializedObject.FindProperty("_iconText");
            _icon = serializedObject.FindProperty("_icon");
            _iconBG = serializedObject.FindProperty("_iconBG");
            _event = serializedObject.FindProperty("_event");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_displayName);
            EditorGUILayout.PropertyField(_icon);
            EditorGUILayout.PropertyField(_iconText);
            EditorGUILayout.PropertyField(_iconBG);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_event);

            serializedObject.ApplyModifiedProperties();
        }
    }
}