#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.ObscuredTypes
{
	using System;
	using UnityEngine;
	using Utils;
	using Detectors;

	/// <summary>
	/// Use it instead of regular <c>int</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong><br/>
	/// Feel free to use regular types for all short-term operations and calculations while keeping obscured type only at the long-term declaration (i.e. class field).
	[Serializable]
	public partial struct ObscuredInt : IObscuredType
	{
		[SerializeField] internal int currentCryptoKey;
		[SerializeField] internal int hiddenValue;
		[SerializeField] internal int fakeValue;
		[SerializeField] internal bool fakeValueActive;
		[SerializeField] internal bool inited;

		private ObscuredInt(int value)
		{
			currentCryptoKey = GenerateKey();
			hiddenValue = Encrypt(value, currentCryptoKey);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
			var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : 0;
			fakeValueActive = detectorRunning;
#endif
			inited = true;
		}

		/// <summary>
		/// Encrypts passed value using passed key.
		/// </summary>
		/// Key can be generated automatically using GenerateKey().
		/// \sa Decrypt(), GenerateKey()
		public static int Encrypt(int value, int key)
		{
			return value ^ key;
		}

		/// <summary>
		/// Decrypts passed value you got from Encrypt() using same key.
		/// </summary>
		/// \sa Encrypt()
		public static int Decrypt(int value, int key)
		{
			return value ^ key;
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
		public static ObscuredInt FromEncrypted(int encrypted, int key)
		{
			var instance = new ObscuredInt();
			instance.SetEncrypted(encrypted, key);
			return instance;
		}

		/// <summary>
		/// Generates random key. Used internally and can be used to generate key for manual Encrypt() calls.
		/// </summary>
		/// <returns>Key suitable for manual Encrypt() calls.</returns>
		public static int GenerateKey()
		{
			return RandomUtils.GenerateIntKey();
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// <param name="key">Encryption key needed to decrypt returned value.</param>
		/// <returns>Encrypted value as is.</returns>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		/// \sa FromEncrypted(), SetEncrypted()
		public int GetEncrypted(out int key)
		{
			if (!inited)
				Init();
			
			key = currentCryptoKey;
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(int encrypted, int key)
		{
			inited = true;
			hiddenValue = encrypted;
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
		public int GetDecrypted()
		{
			return InternalDecrypt();
		}

		public void RandomizeCryptoKey()
		{
			hiddenValue = InternalDecrypt();
			currentCryptoKey = GenerateKey();
			hiddenValue = Encrypt(hiddenValue, currentCryptoKey);
		}

		private int InternalDecrypt()
		{
			if (!inited)
			{
				Init();
				return 0;
			}

			var decrypted = Decrypt(hiddenValue, currentCryptoKey);

			if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
			{
#if ACTK_DETECTION_BACKLOGS
				Debug.LogWarning(ObscuredCheatingDetector.LogPrefix + "Detection backlog:\n" +
				                             $"type: {nameof(ObscuredInt)}\n" +
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
			hiddenValue = Encrypt(0, currentCryptoKey);
			fakeValue = 0;
			fakeValueActive = false;
			inited = true;
		}

		//! @cond

		#region obsolete

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.")]
		public static void SetNewCryptoKey(int newKey) {}

		[Obsolete("This API is redundant and does not perform any actions. It will be removed in future updates.")]
		public void ApplyNewCryptoKey() {}

		[Obsolete("Please use new Encrypt(value, key) API instead.", true)]
		public static int Encrypt(int value) { throw new Exception(); }

		[Obsolete("Please use new Decrypt(value, key) API instead.", true)]
		public static int Decrypt(int value) { throw new Exception(); }

		[Obsolete("Please use new FromEncrypted(encrypted, key) API instead.", true)]
		public static ObscuredInt FromEncrypted(int encrypted) { throw new Exception(); }

		[Obsolete("Please use new GetEncrypted(out key) API instead.", true)]
		public int GetEncrypted() { throw new Exception(); }

		[Obsolete("Please use new SetEncrypted(encrypted, key) API instead.", true)]
		public void SetEncrypted(int encrypted) {}

		#endregion

		//! @endcond
	}
}