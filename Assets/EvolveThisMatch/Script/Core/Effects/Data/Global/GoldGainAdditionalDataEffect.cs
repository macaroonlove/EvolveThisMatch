using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            if (value == 0)
            {
                return $"Ãß°¡ °ñµå È¹µæ·®À» ¼³Á¤ÇØÁÖ¼¼¿ä.";
            }
            else if (value > 0)
            {
                return $"°ñµå È¹µæ ½Ã, {value} Ãß°¡ È¹µæ";
            }
            else
            {
                return $"°ñµå È¹µæ ½Ã, {Mathf.Abs(value)} ¸¸Å­ Â÷°¨µÇ¾î È¹µæ";
            }
        }
    }
}