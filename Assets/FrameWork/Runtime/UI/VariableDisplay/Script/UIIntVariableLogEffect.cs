using DG.Tweening;
using FrameWork.UIBinding;
using EvolveThisMatch.Core;
using TMPro;
using UnityEngine;

namespace FrameWork.UI
{
    public class UIIntVariableLogEffect : UIBase
    {
        #region ¹ÙÀÎµù
        enum Objects
        {
            Log,
        }
        #endregion

        [SerializeField] private int _maxTextLength = 3;
        [SerializeField] private float _holdingTime = 1;
        [SerializeField] private float _moveTime = 1;
        [SerializeField] private Color _plus = Color.green;
        [SerializeField] private Color _minus = Color.red;

        private PoolSystem _poolSystem;
        private GameObject _logPrefab;
        private int prevValue = int.MinValue;

        private Sequence _cachedSequence;

        protected override void Initialize()
        {
            BindObject(typeof(Objects));

            _poolSystem = CoreManager.Instance.GetSubSystem<PoolSystem>();
            _logPrefab = GetObject((int)Objects.Log);
            _logPrefab.SetActive(false);
        }

        internal void Initialize(int value)
        {
            prevValue = value;
        }

        internal void OnChangeValue(int value)
        {
            var diff = value - prevValue;
            prevValue = value;

            var instance = _poolSystem.Spawn(_logPrefab, _holdingTime + _moveTime, transform);
            var text = instance.GetComponent<TextMeshProUGUI>();

            instance.SetActive(true);
            _cachedSequence?.Kill();
            
            if (diff > 0)
            {
                text.text = $"+{diff.Format(_maxTextLength)}";
                text.color = _plus;

                _cachedSequence = DOTween.Sequence();
                _cachedSequence.AppendInterval(_holdingTime);
                _cachedSequence.Append(text.DOColor(new(_plus.r, _plus.g, _plus.b, 0), _moveTime));
            }
            else
            {
                text.text = $"-{diff.Format(_maxTextLength)}";
                text.color = _minus;

                _cachedSequence = DOTween.Sequence();
                _cachedSequence.AppendInterval(_holdingTime);
                _cachedSequence.Append(text.DOColor(new(_minus.r, _minus.g, _minus.b, 0), _moveTime));
            }
        }
    }
}