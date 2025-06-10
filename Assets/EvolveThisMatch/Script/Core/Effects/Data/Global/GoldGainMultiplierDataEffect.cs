using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainMultiplierDataEffect : DataEffect<float>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"°ñµå È¹µæ·®À» »ó½Â¡¤ÇÏ¶ô ½ÃÄÑÁÖ¼¼¿ä.";
            }
            else if (value > 0)
            {
                return $"°ñµå È¹µæ·®  {value * 100}% »ó½Â";
            }
            else
            {
                return $"°ñµå È¹µæ·®  {Mathf.Abs(value) * 100}% ÇÏ¶ô";
            }
        }
    }
}