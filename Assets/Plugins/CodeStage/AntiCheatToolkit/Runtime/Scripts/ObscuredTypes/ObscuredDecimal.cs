#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.ObscuredTypes
{
	using Common;
	using Detectors;
	using System;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using Utils;

	/// <summary>
	/// Use it instead of regular <c>decimal</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Feel free to use regular types for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredDecimal : IObscuredType
	{
		[SerializeField] internal long currentCryptoKey;
		[SerializeField] internal ACTkByte16 hiddenValue;
		[SerializeField] internal bool inited;
		private decimal fakeValue;
		[SerializeField] internal bool fakeValueActive;

		private ObscuredDecimal(decimal value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = InternalEncrypt(value, currentCryptoKey);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
			var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : 0m;
			fakeValueActive = detectorRunning;
#endif
			inited = true;
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static decimal Encrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToDecimal(value, key);
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static decimal Decrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToDecimal(value, key);
		}

		/// <summary>
		/// Creates and fills obscured variable with raw encrypted value previously got from GetEncrypted().
		/// </summary>
		/// Literally does same job as SetEncrypted() but makes new instance instead of filling existing one,
		/// making it easier to initialize new variables from saved encrypted values.
		///
		/// <param name="encrypted">Raw encrypted value you got from GetEncrypted().</param>
		/// <param name="key">Encryption key you've got from GetEncrypted().</param>
		/// <returns>New obscured variable initialized from specified encrypted value.</returns>
		/// \sa GetEncrypted(), SetEncrypted()
		public static ObscuredDecimal FromEncrypted(decimal encrypted, long key)
		{
			var instance = new ObscuredDecimal();
			instance.SetEncrypted(encrypted, key);
			return instance;
		}

		/// <summary>
		/// Generates random key. Used internally and can be used to generate key for manual Encrypt() calls.
		/// </summary>
		/// <returns>Key suitable for manual Encrypt() calls.</returns>
		public static long GenerateKey()
		{
			return RandomUtils.GenerateLongKey();
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public decimal GetEncrypted(out long key)
		{
			if (!inited)
				Init();
			
			key = currentCryptoKey;
			return DecimalLongBytesUnion.ConvertB16ToDecimal(hiddenValue);
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(decimal encrypted, long key)
		{
			inited = true;
			hiddenValue = DecimalLongBytesUnion.ConvertDecimalToB16(encrypted);
			currentCryptoKey = key;

			if (ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValueActive = false;
				fakeValue = InternalDecrypt();
				fakeValueActive = true;
			}
			else
			{
				fakeValueActive = false;
			}
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public decimal GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
		}

		private static ACTkByte16 InternalEncrypt(decimal value, long key)
		{
			return DecimalLongBytesUnion.XorDecimalToB16(value, key);
		}

		private decimal InternalDecrypt()
		{
			if (!inited)
			{
				Init();
				return 0m;
			}

			var decrypted = DecimalLongBytesUnion.XorB16ToDecimal(hiddenValue, currentCryptoKey);

			if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
			{
#if ACTK_DETECTION_BACKLOGS
				Debug.LogWarning(ObscuredCheatingDetector.LogPrefix + "Detection backlog:\n" +
				                             $"type: {nameof(ObscuredDecimal)}\n" +
				                             $"decrypted: {decrypted}\n" +
				                             $"fakeValue: {fakeValue}");
#endif
				ObscuredCheatingDetector.Instance.OnCheatingDetected(this, decrypted, fakeValue);
			}

			return decrypted;
		}
		
		private void Init()
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = InternalEncrypt(0m, currentCryptoKey);
			fakeValue = 0m;
			fakeValueActive = false;
			inited = true;
		}

		//! @cond

		#region obsolete

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.")]
		public static void SetNewCryptoKey(long newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.")]
		public void ApplyNewCryptoKey() {}

		[Obsolete("Please use new Encrypt(value, key) API instead.", true)]
		public static decimal Encrypt(decimal value) { throw new Exception(); }

		[Obsolete("Please use new Decrypt(value, key) API instead.", true)]
		public static decimal Decrypt(decimal value) { throw new Exception(); }

		[Obsolete("Please use new FromEncrypted(encrypted, key) API instead.", true)]
		public static ObscuredDecimal FromEncrypted(decimal encrypted) { throw new Exception(); }

		[Obsolete("Please use new GetEncrypted(out key) API instead.", true)]
		public decimal GetEncrypted() { throw new Exception(); }

		[Obsolete("Please use new SetEncrypted(encrypted, key) API instead.", true)]
		public void SetEncrypted(decimal encrypted) {}

		#endregion

		//! @endcond

		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			[FieldOffset(0)]
			private decimal d;

			[FieldOffset(0)]
			private long l1;

			[FieldOffset(8)]
			private long l2;

			[FieldOffset(0)]
			private ACTkByte16 b16;

			internal static decimal XorDecimalToDecimal(decimal value, long key)
			{
				return FromDecimal(value).XorLongs(key).d;
			}

			internal static ACTkByte16 XorDecimalToB16(decimal value, long key)
			{
				return FromDecimal(value).XorLongs(key).b16;
			}

			internal static decimal XorB16ToDecimal(ACTkByte16 value, long key)
			{
				return FromB16(value).XorLongs(key).d;
			}

			internal static decimal ConvertB16ToDecimal(ACTkByte16 value)
			{
				return FromB16(value).d;
			}

			internal static ACTkByte16 ConvertDecimalToB16(decimal value)
			{
				return FromDecimal(value).b16;
			}

			private static DecimalLongBytesUnion FromDecimal(decimal value)
			{
				return new DecimalLongBytesUnion {d = value};
			}

			private static DecimalLongBytesUnion FromB16(ACTkByte16 value)
			{
				return new DecimalLongBytesUnion {b16 = value};
			}

			private DecimalLongBytesUnion XorLongs(long key)
			{
				l1 ^= key;
				l2 ^= key;
				return this;
			}
		}
	}
}