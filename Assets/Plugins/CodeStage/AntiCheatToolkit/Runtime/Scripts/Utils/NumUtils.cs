#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.Utils
{
    internal static class NumUtils
    {
        public static bool CompareFloats(float f1, float f2, float epsilon = float.Epsilon)
        {
            return f1.Equals(f2) || Math.Abs(f1 - f2) < epsilon;
        }
    }
}