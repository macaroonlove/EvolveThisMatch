using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainMultiplierDataEffect : ImmutableDataEffect<float>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "°ñµå È¹µæ·®À» »ó½Â¡¤ÇÏ¶ô ½ÃÄÑÁÖ¼¼¿ä.";
            }
            else if (_value > 0)
            {
                return $"°ñµå È¹µæ·®  {_value * 100}% »ó½Â";
            }
            else
            {
                return $"°ñµå È¹µæ·®  {Mathf.Abs(_value) * 100}% ÇÏ¶ô";
            }
        }
    }
}