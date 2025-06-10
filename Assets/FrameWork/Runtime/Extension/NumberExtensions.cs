using UnityEngine;

namespace FrameWork
{
    public static class NumberExtensions
    {
        private static readonly string[] _unitSymbols = { "", "K", "M", "B", "T" };

        public static string Format(this int num, int maxLength = -1)
        {
            if (num < 1000) return num.ToString("N0");

            int symbol = Mathf.Min(_unitSymbols.Length - 1, (int)(Mathf.Log10(num) / 3));
            double number = num / Mathf.Pow(1000, symbol);

            string result = number.ToString("0.#") + _unitSymbols[symbol];

            if (maxLength > 0 && result.Length > maxLength) return result.Substring(0, maxLength);
            else return result;
        }
    }
}