using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameWork.Tooltip
{
    [System.Serializable]
    public class TooltipData
    {
        [SerializeField] private List<string> _stringKeys = new List<string>();
        [SerializeField] private List<string> _spriteKeys = new List<string>();

        [SerializeField] private List<string> _stringValues = new List<string>();
        [SerializeField] private List<Sprite> _spriteValues = new List<Sprite>();

        private Dictionary<string, string> _stringData = new Dictionary<string, string>();
        private Dictionary<string, Sprite> _spriteData = new Dictionary<string, Sprite>();

        internal Dictionary<string, string> getAllString => _stringData;
        internal Dictionary<string, Sprite> getAllSprite => _spriteData;

        public void InitializeData()
        {
            _stringData = _stringKeys.Zip(_stringValues, (key, value) => new { key, value })
                             .ToDictionary(x => x.key, x => x.value);
            _spriteData = _spriteKeys.Zip(_spriteValues, (key, value) => new { key, value })
                              .ToDictionary(x => x.key, x => x.value);
        }

        #region 데이터 수정
        internal void SetString(string key, string value)
        {
            _stringData[key] = value;
        }

        internal void SetSprite(string key, Sprite value)
        {
            _spriteData[key] = value;
        }
        #endregion

#if UNITY_EDITOR
        #region 데이터 추가(Editor)
        internal void AddString(string key, string value)
        {
            if (_stringData.ContainsKey(key)) return;

            _stringKeys.Add(key);
            _stringValues.Add(value);
        }

        internal void AddSprite(string key, Sprite value)
        {
            if (_spriteData.ContainsKey(key)) return;

            _spriteKeys.Add(key);
            _spriteValues.Add(value);
        }
        #endregion

        #region 데이터 수정(Editor)
        internal void SetStringEditor(string key, string value)
        {
            int index = _stringKeys.IndexOf(key);

            if (index == -1)
            {
                _stringKeys.Add(key);
                _stringValues.Add(value);
            }
            else
            {
                _stringKeys[index] = key;
                _stringValues[index] = value;
            }
        }

        internal void SetSpriteEditor(string key, Sprite value)
        {
            int index = _spriteKeys.IndexOf(key);

            if (index == -1)
            {
                _spriteKeys.Add(key);
                _spriteValues.Add(value);
            }
            else
            {
                _spriteKeys[index] = key;
                _spriteValues[index] = value;
            }
        }
        #endregion
#endif

        internal string GetString(string key) => _stringData.ContainsKey(key) ? _stringData[key] : string.Empty;
        internal Sprite GetSprite(string key) => _spriteData.ContainsKey(key) ? _spriteData[key] : null;

        internal bool IsInitialize()
        {
            return _stringKeys.Count > 0 || _spriteKeys.Count > 0;
        }

        internal bool IsInitializeData()
        {
            return _stringData.Count > 0 || _spriteData.Count > 0;
        }
    }
}