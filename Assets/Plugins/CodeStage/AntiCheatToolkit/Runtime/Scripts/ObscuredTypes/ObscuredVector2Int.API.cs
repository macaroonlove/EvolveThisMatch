#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector2Int : IEquatable<ObscuredVector2Int>, IEquatable<Vector2Int>
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredVector2Int(Vector2Int value)
		{
			return new ObscuredVector2Int(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector2Int(ObscuredVector2Int value)
		{
			return value.InternalDecrypt();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector2(ObscuredVector2Int value)
		{
			return value.InternalDecrypt();
		}
		
		public override bool Equals(object other)
		{
			return other is ObscuredVector2Int o && Equals(o) ||
				   other is Vector2Int r && Equals(r);
		}

		public bool Equals(ObscuredVector2Int other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}

		public bool Equals(Vector2Int other)
		{
			return InternalDecrypt().Equals(other);
		}	

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}
	}
}