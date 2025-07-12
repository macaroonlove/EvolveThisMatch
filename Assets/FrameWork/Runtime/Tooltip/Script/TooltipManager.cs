using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Tooltip
{
    public class TooltipManager : Singleton<TooltipManager>
    {
        private Dictionary<GameObject, TooltipStyle> _tooltip = new Dictionary<GameObject, TooltipStyle>();
        private Transform _parent;
        private TooltipTrigger _showTrigger;

        protected override void Initialize()
        {
            _parent = transform.GetChild(0);
        }

        internal void Show(TooltipTrigger trigger)
        {
            _showTrigger = trigger;

            var prefab = trigger.tooltipStyle.gameObject;
            TooltipStyle style;

            if (_tooltip.TryGetValue(prefab, out style))
            {
                style.Show(true);
            }
            else
            {
                var instance = CreateTooltip(trigger.tooltipStyle);
                style = instance.GetComponent<TooltipStyle>();

                _tooltip.Add(prefab, style);
            }

            style.ApplyData(trigger.tooltipData);

            var rect = style.transform as RectTransform;
            SetAnchorPivot(rect, trigger.tooltipPosition);
            style.transform.position = trigger.transform.position;
            rect.anchoredPosition += trigger.tooltipOffset;
        }

        internal void Hide(TooltipTrigger trigger)
        {
            _showTrigger = null;

            if (trigger.tooltipStyle == null) return;

            var tooltipPrefab = trigger.tooltipStyle.gameObject;
            TooltipStyle style;

            if (_tooltip.TryGetValue(tooltipPrefab, out style))
            {
                style.Hide(true);
            }
        }

        internal void ReShow(TooltipTrigger trigger)
        {
            if (_showTrigger != null && _showTrigger == trigger)
            {
                Show(trigger);
            }
        }

        private GameObject CreateTooltip(TooltipStyle style)
        {
            var tooltipPrefab = style.gameObject;
            var instance = Instantiate(tooltipPrefab, _parent);

            return instance;
        }

        internal TooltipStyle GetTooltipStyle(TooltipTrigger trigger)
        {
            var key = trigger.tooltipStyle.gameObject;

            if (_tooltip.ContainsKey(key))
            {
                return _tooltip[key];
            }

            var instance = CreateTooltip(trigger.tooltipStyle);
            var style = instance.GetComponent<TooltipStyle>();
            style.Hide(true);

            _tooltip.Add(key, style);

            return style;
        }

        private void SetAnchorPivot(RectTransform rectTransform, TipPosition tipPosition)
        {
            Vector2 anchorPosition = Vector2.zero;

            switch (tipPosition)
            {
                case TipPosition.TopLeft:
                    anchorPosition = new Vector2(1, 0);
                    break;
                case TipPosition.TopCenter:
                    anchorPosition = new Vector2(0.5f, 0);
                    break;
                case TipPosition.TopRight:
                    anchorPosition = new Vector2(0, 0);
                    break;
                case TipPosition.MiddleLeft:
                    anchorPosition = new Vector2(1, 0.5f);
                    break;
                case TipPosition.MiddleCenter:
                    anchorPosition = new Vector2(0.5f, 0.5f);
                    break;
                case TipPosition.MiddleRight:
                    anchorPosition = new Vector2(0, 0.5f);
                    break;
                case TipPosition.BottomLeft:
                    anchorPosition = new Vector2(1, 1);
                    break;
                case TipPosition.BottomCenter:
                    anchorPosition = new Vector2(0.5f, 1);
                    break;
                case TipPosition.BottomRight:
                    anchorPosition = new Vector2(0, 1);
                    break;
            }

            rectTransform.anchorMin = anchorPosition;
            rectTransform.anchorMax = anchorPosition;
            rectTransform.pivot = anchorPosition;
        }
    }

    internal enum TipPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}