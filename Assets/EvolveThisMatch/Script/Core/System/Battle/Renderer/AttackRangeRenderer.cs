using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AttackRangeRenderer : MonoBehaviour, IBattleSystem
    {
        private LineRangeRenderer _lineRangeRenderer;
        private CircleRangeRenderer _circleRangeRenderer;

        internal IReadOnlyList<IReadOnlyList<Vector2>> lines => _lineRangeRenderer.lines;

        private void Awake()
        {
            _lineRangeRenderer = GetComponentInChildren<LineRangeRenderer>();
            _circleRangeRenderer = GetComponentInChildren<CircleRangeRenderer>();

            _lineRangeRenderer.Initialize();
            _circleRangeRenderer.Initialize();
        }

        public void Initialize()
        {
            
        }

        public void Deinitialize()
        {

        }

        public void Show(int lineCount)
        {
            _lineRangeRenderer.Show(lineCount);
        }

        public void Show(Unit unit, float range)
        {
            _circleRangeRenderer.Show(unit, range);
        }

        public void Hide()
        {
            _lineRangeRenderer.Hide();
            _circleRangeRenderer.Hide();
        }
    }
}