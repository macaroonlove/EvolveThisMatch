#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if UNITY_ANDROID

using System;
using CodeStage.AntiCheat.Common;
using UnityEngine;

namespace CodeStage.AntiCheat.Utils
{
	internal static class AndroidRoutines
	{
		private const string RoutinesClassPath = "net.codestage.actk.androidnative.ACTkAndroidRoutines";

		private static readonly Lazy<AndroidJavaClass> RoutinesClass = new Lazy<AndroidJavaClass>(InitJavaClass);

		public static long GetSystemNanoTime()
		{
			return RoutinesClass.Value?.CallStatic<long>("GetSystemNanoTime") ?? DateTime.UtcNow.Ticks;
		}
		
		public static string GetPackageInstallerName()
		{
			return RoutinesClass.Value?.CallStatic<string>("GetPackageInstallerName");
		}
		
		private static AndroidJavaClass InitJavaClass()
		{
			try
			{
				return new AndroidJavaClass(RoutinesClassPath);
			}
			catch (Exception e)
			{
				ACTk.PrintExceptionForSupport($"Couldn't create instance of the {nameof(AndroidJavaClass)}: {RoutinesClassPath}!\n" +
											  "Please make sure you are not obfuscating public ACTk's Java Plugin classes.", e);
			}
			return null;
		}
	}
}

#endif