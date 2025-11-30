using System;

namespace FrameWork
{
    public static class NumberExtensions
    {
        private static readonly string[] _unitSymbols = {
            "", "K", "M", "B", "T", "Q", "R", "S", "O", "N",
            "P", "A", "B", "C", "D", "E", "F", "G", "H", "I"
        };

        public static string Format(this int num, int maxLength = -1, int turncate = 0)
        {
            if (num < 1000 || (maxLength > 0 && num.ToString().Length <= maxLength))
                return num.ToString("N0");

            int symbolIndex = 0;
            double value = num;

            while (value >= 1000 && symbolIndex < _unitSymbols.Length - 1)
            {
                value /= 1000;
                symbolIndex++;
            }

            double factor = Math.Pow(10, turncate);
            value = Math.Floor(value * factor) / factor;

            string formatted = value % 1 == 0
                ? ((int)value).ToString()
                : value.ToString($"0.{new string('#', turncate)}");

            if (maxLength > 0 && formatted.Length > maxLength)
            {
                formatted = formatted.Substring(0, maxLength);
            }

            string result = formatted + _unitSymbols[symbolIndex];

            return result;
        }
    }
}