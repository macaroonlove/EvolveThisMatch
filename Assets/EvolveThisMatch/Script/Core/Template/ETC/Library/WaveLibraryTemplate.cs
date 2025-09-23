using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Wave", fileName = "WaveLibrary", order = 0)]
    public class WaveLibraryTemplate : ScriptableObject
    {
        [SerializeField] private string _title;
        [SerializeField] private List<WaveChapter> _waves = new List<WaveChapter>();

        public string title => _title;
        public IReadOnlyList<WaveChapter> waves => _waves;
    }

    [Serializable]
    public class WaveChapter
    {
        [SerializeField] private string _chapterName;
        [SerializeField] private List<WaveTemplate> _waves = new List<WaveTemplate>();

        public string chapterName => _chapterName;
        public IReadOnlyList<WaveTemplate> waves => _waves;
    }
}
