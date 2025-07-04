using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    public class CircleRangeRenderer : MonoBehaviour
    {
        private Transform _rangeTile;
        private Follow _follow;

        internal void Initialize()
        {
            _rangeTile = transform.GetChild(0);
            _follow = GetComponentInChildren<Follow>();

            Hide();
        }

        internal void Show(Unit unit, float range)
        {
            _rangeTile.position = unit.transform.position;
            _rangeTile.localScale = Vector3.one * range;
            _follow.SetTarget(unit.transform, Vector3.zero);

            _rangeTile.gameObject.SetActive(true);
        }

        internal void Hide()
        {
            _rangeTile.gameObject.SetActive(false);
        }
    }
}