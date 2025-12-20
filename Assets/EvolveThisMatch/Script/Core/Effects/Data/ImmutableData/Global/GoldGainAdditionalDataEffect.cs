using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainAdditionalDataEffect : ImmutableDataEffect<int>
    {
        public override string GetDescription()
        {
            if (_value == 0)
            {
                return "Ãß°¡ °ñµå È¹µæ·®À» ¼³Á¤ÇØÁÖ¼¼¿ä.";
            }
            else if (_value > 0)
            {
                return $"°ñµå È¹µæ ½Ã, {_value} Ãß°¡ È¹µæ";
            }
            else
            {
                return $"°ñµå È¹µæ ½Ã, {Mathf.Abs(_value)} ¸¸Å­ Â÷°¨µÇ¾î È¹µæ";
            }
        }
    }
}