using FrameWork.UIBinding;

namespace FrameWork.Tooltip
{
    public abstract class TooltipStyle : UIBase
    {
#if UNITY_EDITOR
        public abstract TooltipData CreateField();
#endif
        public abstract void ApplyData(TooltipData data);
    }
}