using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 액티브 아이템 효과를 적용시키는 시스템
    /// </summary>
    public class TomeSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField, ReadOnly] private List<TomeTemplate> _selectedItems = new List<TomeTemplate>();

        [SerializeField, ReadOnly] private List<TomeTemplate> _items = new List<TomeTemplate>();

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private List<TomeTemplate> _debugItems = new List<TomeTemplate>();
#endif

        public void Initialize()
        {
#if UNITY_EDITOR
            _selectedItems.AddRange(_debugItems);
#endif
        }

        public void Deinitialize()
        {
            
        }

        /// <summary>
        /// 아이템 추가
        /// </summary>
        public void AddItem(TomeTemplate template)
        {
            if (_items.Contains(template))
            {
#if UNITY_EDITOR
                Debug.LogError($"아이템이 중복되었습니다. {template.displayName}");
#endif
                return;
            }

            _items.Add(template);
        }

        /// <summary>
        /// 아이템 불러오기
        /// </summary>
        public TomeTemplate GetSelectedItem(int index)
        {
            if (index >= _selectedItems.Count) return null;

            return _selectedItems[index];
        }

        private void OnDestroy()
        {            
            _items.Clear();
        }
    }
}