using UnityEngine;

namespace EvolveThisMatch.Core
{
    [CreateAssetMenu(menuName = "Templates/Effect/ATKIncreaseRuntimeDataEffect", fileName = "ATKIncreaseRuntimeDataEffect", order = 0)]
    public class ATKIncreaseRuntimeDataEffect : ATKIncreaseDataEffect, IRuntimeDataEffect
    {
        public string GetTitle() => "전투력 증가(%)";

        public void SetValue(float value)
        {
            _value = Mathf.Round(value * 0.01f * 10000f) / 10000f;
        }
    }
}