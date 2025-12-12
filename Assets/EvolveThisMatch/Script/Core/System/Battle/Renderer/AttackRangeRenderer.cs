using FrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class AttackRangeRenderer : MonoBehaviour, IBattleSystem
    {
        private TileRangeRenderer _tileRangeRenderer;
        private CircleRangeRenderer _circleRangeRenderer;

        public IReadOnlyList<IReadOnlyList<GameObject>> tiles => _tileRangeRenderer.tiles;

        private void Awake()
        {
            _tileRangeRenderer = GetComponentInChildren<TileRangeRenderer>();
            _circleRangeRenderer = GetComponentInChildren<CircleRangeRenderer>();

            _tileRangeRenderer.Initialize();
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
            _tileRangeRenderer.Show(lineCount);
        }

        public void Show(Unit unit, TileRangeTemplate tileRangeTemplate)
        {
            _tileRangeRenderer.Show(unit, tileRangeTemplate);
        }

        public void Show(Unit unit, float range)
        {
            _circleRangeRenderer.Show(unit, range);
        }

        public void Hide()
        {
            _tileRangeRenderer.Hide();
            _circleRangeRenderer.Hide();
        }
    }
}