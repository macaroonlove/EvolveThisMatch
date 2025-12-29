using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class SpawnSummonEffectLogic : IMutableValueBindingProvider
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private MutableValue _durationMutableValue;
        [SerializeField] private ElementalValue _durationElementalValue;
        [SerializeField] private float _duration;
        [SerializeField] private bool _isProbability;
        [SerializeField] private MutableValue _probabilityMutableValue;
        [SerializeField] private ElementalValue _probabilityElementalValue;
        [SerializeField] private int _probability;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private SummonTemplate _summon;

        #region MutableValue 처리
        public void Initialize()
        {
            _durationMutableValue = new MutableValue();
            _durationElementalValue = new ElementalValue();
            _probabilityMutableValue = new MutableValue();
            _probabilityElementalValue = new ElementalValue();
        }

        public bool TryGetBindValue(string bindKey, EffectContext context, out string value)
        {
            if (_durationMutableValue.bindKey == bindKey)
            {
                value = _durationMutableValue.GetValueString(_duration, context);
                return true;
            }

            if (_probabilityMutableValue.bindKey == bindKey)
            {
                value = _probabilityMutableValue.GetValueString(_probability, context);
                return true;
            }

            value = null;
            return false;
        }
        #endregion

        #region 설명
        public string GetDescription()
        {
            string result = "";

            if (_isProbability)
            {
                float probability = _probabilityMutableValue.GetPreviewValue(_probability);
                probability = _probabilityElementalValue.GetPreviewValue(probability);
                result += $" {probability}%의 확률로 ";
            }

            if (_isInfinity)
            {
                result += "무한지속";
            }
            else
            {
                float duration = _durationMutableValue.GetPreviewValue(_duration);
                duration = _durationElementalValue.GetPreviewValue(duration);
                result += $"{duration}초 동안 유지되는";
            }

            return result + $" {_summon.displayName}를 소환합니다.";
        }
        #endregion

        public void Execute(EffectContext context, Unit casterUnit, Vector3 spawnPosition)
        {
            if (casterUnit == null) return;
            if (TryProbability(context) == false) return;

            spawnPosition += _offset;

            if (_isInfinity)
            {
                BattleManager.Instance.GetSubSystem<SummonCreateSystem>().CreateUnit(_summon, spawnPosition, summoner: casterUnit);
            }
            else
            {
                float duration = _durationMutableValue.GetValue(_duration, context);
                duration = _durationElementalValue.GetValue(duration);
                BattleManager.Instance.GetSubSystem<SummonCreateSystem>().CreateUnit(_summon, spawnPosition, duration, casterUnit);
            }
        }

        private bool TryProbability(EffectContext context)
        {
            if (_isProbability == false) return true;

            var rand = UnityEngine.Random.Range(0, 100);
            int probability = _probabilityMutableValue.GetValue(_probability, context);
            probability = _probabilityElementalValue.GetValue(probability);
            return rand <= probability;
        }

        #region 그리기
#if UNITY_EDITOR
        public void Draw(Rect rect)
        {
            EffectDrawUtility.DrawRow(ref rect, "무한지속 사용 여부", valueRect =>
            {
                _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            });

            if (!_isInfinity)
            {
                rect.y += 5;
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _durationMutableValue, _durationElementalValue, "지속시간", valueRect =>
                {
                    _duration = EditorGUI.FloatField(valueRect, _duration);
                    _duration = Mathf.Max(_duration, 0);
                });
                rect.y += 5;
            }

            rect.y += 20;
            EffectDrawUtility.DrawRow(ref rect, "확률 사용 여부", valueRect =>
            {
                _isProbability = EditorGUI.Toggle(valueRect, _isProbability);
            });

            if (_isProbability)
            {
                rect.y += 5;
                EffectDrawUtility.DrawBoxedScaledValue(ref rect, _probabilityMutableValue, _probabilityElementalValue, "확률", valueRect =>
                {
                    _probability = (int)EditorGUI.Slider(valueRect, _probability, 0, 100);
                });
                rect.y += 5;
            }

            EffectDrawUtility.DrawRow(ref rect, "오프셋", valueRect =>
            {
                _offset = EditorGUI.Vector3Field(valueRect, GUIContent.none, _offset);
            });

            EffectDrawUtility.DrawRow(ref rect, "소환수", valueRect =>
            {
                _summon = (SummonTemplate)EditorGUI.ObjectField(valueRect, _summon, typeof(SummonTemplate), false);
            });
        }
        public int GetNumRows()
        {
            int rowNum = 0;

            if (!_isInfinity) rowNum += 2;
            if (_isProbability) rowNum += 2;

            rowNum += _durationMutableValue.GetNumRows();
            rowNum += _durationElementalValue.GetNumRows();
            rowNum += _probabilityMutableValue.GetNumRows();
            rowNum += _probabilityElementalValue.GetNumRows();

            return rowNum;
        }
#endif
        #endregion
    }
}