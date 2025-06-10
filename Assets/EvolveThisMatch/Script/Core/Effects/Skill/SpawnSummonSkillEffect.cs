using UnityEditor;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class SpawnSummonSkillEffect : SkillEffect
    {
        [SerializeField] private bool _isInfinity;
        [SerializeField] private float _duration;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private SummonTemplate _summon;

        public override string GetDescription()
        {
            return "소환수 소환";
        }

        internal override void SkillImpact(Unit casterUnit, Unit targetUnit)
        {
            var summonCreateSystem = BattleManager.Instance.GetSubSystem<SummonCreateSystem>();
            Vector3 spawnPosition = targetUnit.transform.position + _offset;

            if (_isInfinity)
            {
                summonCreateSystem.CreateUnit(_summon, spawnPosition, summoner: casterUnit);
            }
            else
            {
                summonCreateSystem.CreateUnit(_summon, spawnPosition, _duration, casterUnit);
            }
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, 140, rect.height);
            var valueRect = new Rect(rect.x + 140, rect.y, rect.width - 140, rect.height);

            GUI.Label(labelRect, "무한지속 사용 여부");
            _isInfinity = EditorGUI.Toggle(valueRect, _isInfinity);
            if (!_isInfinity)
            {
                labelRect.y += 20;
                valueRect.y += 20;
                GUI.Label(labelRect, "지속시간");
                _duration = EditorGUI.FloatField(valueRect, _duration);
            }

            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "오프셋");
            _offset = EditorGUI.Vector3Field(valueRect, GUIContent.none, _offset);


            labelRect.y += 20;
            valueRect.y += 20;
            GUI.Label(labelRect, "소환수");
            _summon = (SummonTemplate)EditorGUI.ObjectField(valueRect, _summon, typeof(SummonTemplate), false);
        }

        public override int GetNumRows()
        {
            int rowNum = base.GetNumRows() + 1;

            if (!_isInfinity)
            {
                rowNum++;
            }

            return rowNum;
        }
#endif
    }
}