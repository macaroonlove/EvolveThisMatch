using System;
using UnityEngine;

namespace EvolveThisMatch.Core
{
    [Serializable]
    public class GoldGainAdditionalDataEffect : DataEffect<int>
    {
        public override string GetDescription()
        {
            return FormatDescription(_value);
        }

        public override string GetDescription(int level)
        {
            return FormatDescription(GetValue(level));
        }

        public override int GetValue(int level)
        {
            return _value + 1 * (level - 1);
        }

        private string FormatDescription(int value)
        {
            if (value == 0)
            {
                return "Ãß°¡ °ñµå È¹µæ·®À» ¼³Á¤ÇØÁÖ¼¼¿ä.";
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