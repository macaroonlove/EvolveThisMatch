using FrameWork.UIBinding;

namespace FrameWork.Tooltip
{
    public abstract class TooltipStyle : UIBase
    {
#if UNITY_EDITOR
        internal abstract TooltipData CreateField();
#endif
        internal abstract void ApplyData(TooltipData data);
    }
}