using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Library/Wave", fileName = "WaveLibrary", order = 0)]
    public class WaveLibraryTemplate : ScriptableObject
    {
        [SerializeField] private List<WaveCategory> _categorys = new List<WaveCategory>();

        public IReadOnlyList<WaveCategory> categorys => _categorys;
    }

    [Serializable]
    public class WaveCategory
    {
        [SerializeField, Label("이야기 이름")] private string _title;
        [SerializeField, Label("시작 코인")] private int _startCoin;
        [SerializeField, Label("시작 재능의 파편")] private int _startCrystal;
        [SerializeField] private List<WaveChapter> _chapters = new List<WaveChapter>();

        public string title => _title;
        public int startCoin => _startCoin;
        public int startCrystal => _startCrystal;
        public IReadOnlyList<WaveChapter> chapters => _chapters;
    }

    [Serializable]
    public class WaveChapter
    {
        [SerializeField, Label("챕터 이름")] private string _chapterName;
        [SerializeField] private List<WaveTemplate> _waves = new List<WaveTemplate>();

        public string chapterName => _chapterName;
        public IReadOnlyList<WaveTemplate> waves => _waves;
    }
}