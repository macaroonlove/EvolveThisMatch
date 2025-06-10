using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/FX/FXLibrary", fileName = "FXLibrary_", order = 0)]
    public class FXLibrary : FX
    {
        [SerializeField] private List<FX> _fxList = new List<FX>();

        public override void Play(Unit target)
        {
            foreach (var fx in _fxList)
            {
                fx.Play(target);
            }
        }

        public override void Play(Vector3 pos)
        {
            foreach (var fx in _fxList)
            {
                fx.Play(pos);
            }
        }
    }
}