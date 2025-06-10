#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Globalization;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredDateTime))]
	internal class ObscuredDateTimeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative(nameof(ObscuredDateTime.hiddenValue));
			var cryptoKey = prop.FindPropertyRelative(nameof(ObscuredDateTime.currentCryptoKey));
			var inited = prop.FindPropertyRelative(nameof(ObscuredDateTime.inited));
			var fakeValue = prop.FindPropertyRelative(nameof(ObscuredDateTime.fakeValue));
			var fakeValueActive = prop.FindPropertyRelative(nameof(ObscuredDateTime.fakeValueActive));

			var currentCryptoKey = cryptoKey.longValue;
			var val = new DateTime(0);

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
					currentCryptoKey = cryptoKey.longValue = ObscuredDateTime.GenerateKey();
				hiddenValue.longValue = ObscuredDateTime.Encrypt(new DateTime(0), currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = ObscuredDateTime.Decrypt(hiddenValue.longValue, currentCryptoKey);
			}

			var labelRect = position;
			labelRect.width = position.width * 0.75f;
			label = EditorGUI.BeginProperty(labelRect, label, prop);
			EditorGUI.BeginChangeCheck();
			var dateString = val.ToString("o", DateTimeFormatInfo.InvariantInfo);
			var input = EditorGUI.DelayedTextField(labelRect, label, dateString);
			if (EditorGUI.EndChangeCheck())
			{
				DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out val);
				hiddenValue.longValue = ObscuredDateTime.Encrypt(val, currentCryptoKey);
				fakeValue.longValue = val.ToBinary();
				fakeValueActive.boolValue = true;
			}
			EditorGUI.EndProperty();
			
			var kindRect = position;
			kindRect.x = labelRect.xMax + 5;
			kindRect.width = position.width * 0.25f - 5;
			label = EditorGUI.BeginProperty(kindRect, GUIContent.none, prop);
			
			EditorGUI.BeginChangeCheck();
			var kind = val.Kind;
			var kindInput = (DateTimeKind)EditorGUI.EnumPopup(kindRect, label, kind);
			if (EditorGUI.EndChangeCheck())
			{
				val = DateTime.SpecifyKind(val, kindInput);
				hiddenValue.longValue = ObscuredDateTime.Encrypt(val, currentCryptoKey);
				fakeValue.longValue = val.ToBinary();
				fakeValueActive.boolValue = true;
			}
			EditorGUI.EndProperty();
		}
	}
}