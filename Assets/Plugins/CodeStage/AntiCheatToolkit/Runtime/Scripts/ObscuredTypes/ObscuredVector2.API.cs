#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public partial struct ObscuredVector2 : IEquatable<ObscuredVector2>, IEquatable<Vector2>
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator ObscuredVector2(Vector2 value)
		{
			return new ObscuredVector2(value);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector2(ObscuredVector2 value)
		{
			return value.InternalDecrypt();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public static implicit operator Vector3(ObscuredVector2 value)
		{
			var v = value.InternalDecrypt();
			return new Vector3(v.x, v.y, 0.0f);
		}
		
		public override bool Equals(object other)
		{
			return other is ObscuredVector2 o && Equals(o) ||
				   other is Vector2 r && Equals(r);
		}

		public bool Equals(ObscuredVector2 other)
		{
			return currentCryptoKey == other.currentCryptoKey ? hiddenValue.Equals(other.hiddenValue) : 
				InternalDecrypt().Equals(other.InternalDecrypt());
		}

		public bool Equals(Vector2 other)
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

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}
		
		public void Normalize()
		{
			var temp = InternalDecrypt();
			temp.Normalize();
			SetEncrypted(Encrypt(normalized, currentCryptoKey), currentCryptoKey);
		}
		
		public Vector2 normalized => InternalDecrypt().normalized;
	}
}