using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Wave", fileName = "WaveLibrary", order = 0)]
    public class WaveLibraryTemplate : ScriptableObject
    {
        public List<WaveTemplate> waves = new List<WaveTemplate>();
    }
}
