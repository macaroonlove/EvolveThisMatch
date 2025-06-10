using FrameWork.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    /// <summary>
    /// 액티브 아이템 효과를 적용시키는 시스템
    /// </summary>
    public class ActiveItemSystem : MonoBehaviour, ICoreSystem
    {
        [SerializeField, ReadOnly] private List<ActiveItemTemplate> _selectedItems = new List<ActiveItemTemplate>();

        [SerializeField, ReadOnly] private List<ActiveItemTemplate> _items = new List<ActiveItemTemplate>();

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private List<ActiveItemTemplate> _debugItems = new List<ActiveItemTemplate>();
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
        public void AddItem(ActiveItemTemplate template)
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
        public ActiveItemTemplate GetSelectedItem(int index)
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