using EvolveThisMatch.Save;
using System;
using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public abstract class MutableDataEffect<T> : DataEffect<T>
    {
        [SerializeField] protected T _value;
        [SerializeField] protected float _scaleFactor;
        [SerializeField] protected EMutableScaleBase _scaleBase;
        [SerializeField] protected int _previewScaleBase = 2;

        public virtual int GetPreviewValue(int value)
        {
            return (int)(value + (_previewScaleBase * _scaleFactor));
        }

        public virtual float GetPreviewValue(float value)
        {
            return value + (_previewScaleBase * _scaleFactor);
        }

        public abstract string GetDescription(MutableContext context);
        public abstract T GetValue(MutableContext context);

#if UNITY_EDITOR
        public override int GetNumRows() => 2;

        public override void Draw(Rect rect)
        {
            DrawScaleFactor(rect);
        }

        protected virtual void DrawScaleFactor(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 100, rect.height);
            var valueRect = new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height);

            GUI.Label(labelRect, "증가 계수");
            _scaleFactor = EditorGUI.FloatField(valueRect, _scaleFactor);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "기준값");
            _scaleBase = (EMutableScaleBase)EditorGUI.EnumPopup(valueRect, _scaleBase);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "기준값 미리보기");
            _previewScaleBase = EditorGUI.IntField(valueRect, _previewScaleBase);

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "초기 값");
            if (typeof(T) == typeof(int))
                _value = (T)(object)EditorGUI.IntField(valueRect, (int)(object)_value);
            else if (typeof(T) == typeof(float))
                _value = (T)(object)EditorGUI.FloatField(valueRect, (float)(object)_value);
            // 이곳에 다른 타입에 대한 처리를 추가할 수 있습니다.
        }
#endif
    }

    public class MutableContext
    {
        public AgentBattleData agentData;
        public AgentSaveData.Agent agentSaveData;
        public ItemSaveData.Artifact artifactSaveData;
        public ItemSaveData.Tome tomeSaveData;

        public int GetScaleValue(EMutableScaleBase scaleBase)
        {
            int value = -1;
            switch (scaleBase)
            {
                case EMutableScaleBase.AgentLevel:
                    value = agentSaveData != null ? agentSaveData.level : 1;
                    break;
                case EMutableScaleBase.AgentSync:
                    value = agentData != null ? agentData.sync : 1;
                    break;
                case EMutableScaleBase.ArtifactLevel:
                    value = artifactSaveData != null ? artifactSaveData.level : 1;
                    break;
                case EMutableScaleBase.TomeLevel:
                    value = tomeSaveData != null ? tomeSaveData.level : 1;
                    break;
            }


            if (value == -1)
            {
                value = 1;
#if UNITY_EDITOR
                Debug.LogWarning($"MutableContext: {scaleBase} 기준 데이터가 없습니다.");
#endif
            }

            return value;
        }
    }

    public enum EMutableScaleBase
    {
        AgentLevel,
        AgentSync,
        ArtifactLevel,
        TomeLevel,
    }
}