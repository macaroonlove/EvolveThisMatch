namespace EvolveThisMatch.Core
{
    [System.Serializable]
    public class ApplyTypeByAmountData
    {
        public EApplyType applyType;
        public float amount;
    }

    [System.Serializable]
    public class ApplyType_TargetOnlyByAmountData
    {
        public EApplyType_TargetOnly applyType;
        public float amount;
    }
}