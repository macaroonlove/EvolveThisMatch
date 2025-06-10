#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using Detectors;
	using UnityEditor;

	using Common;
	using PostProcessors;
	using UnityEngine;
	using Processors;

	internal static class ACTkMenuItems
	{
		// ---------------------------------------------------------------
		//  Main menu items
		// ---------------------------------------------------------------

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Settings...", false, 100)]
		private static void ShowSettingsWindow()
		{
			ACTkSettings.Show();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Injection Detector Whitelist Editor...", false, 1000)]
		private static void ShowAssembliesWhitelistWindow()
		{
			UserWhitelistEditor.ShowWindow();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Calculate external build hashes", false, 1200)]
		private static async void HashExternalBuild()
		{
			var buildHashes = await CodeHashGeneratorPostprocessor.CalculateExternalBuildHashesAsync(null, true);
			if (buildHashes == null || buildHashes.FileHashes.Count == 0)
			{
				Debug.LogError(ACTk.LogPrefix + "External build hashing was not successful. " +
				               "See previous log messages for possible details.");
			}
		}
		
		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Configure proguard-user.txt", false, 1201)]
		private static void CheckProGuard()
		{
			BuildPreProcessor.CheckProGuard(true);
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate obscured types on prefabs...", false, 1500)]
		private static void MigrateObscuredTypesOnPrefabs()
		{
			MigrateUtils.MigrateObscuredTypesOnPrefabs("ObscuredFloat", "ObscuredDouble", "ObscuredVector2", "ObscuredVector3", "ObscuredQuaternion");
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate obscured types in opened scene(s)...", false, 1501)]
		private static void MigrateObscuredTypesInScene()
		{
			MigrateUtils.MigrateObscuredTypesInScene("ObscuredFloat", "ObscuredDouble", "ObscuredVector2", "ObscuredVector3", "ObscuredQuaternion");
		}		
		
		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Validate/Validate obscured types in assets...", false, 1500)]
		private static void ValidateObscuredTypesInAssets()
		{
			ObscuredTypesValidator.ValidateProjectAssets();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Validate/Validate obscured types in opened scene(s)...", false, 1501)]
		private static void ValidateObscuredTypesInOpenedScenes()
		{
			ObscuredTypesValidator.ValidateOpenedScenes();
		}
		
		/* will be needed when obsolete string internals will be deprecated along with automatic migration */

		//[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate ObscuredString on prefabs...", false, 1600)]
		private static void MigrateObscuredStringOnPrefabs()
		{
			MigrateUtils.MigrateObscuredTypesOnPrefabs("ObscuredString");
		}

		//[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Migrate/Migrate ObscuredString in opened scene(s)...", false, 1601)]
		private static void MigrateObscuredStringInScene()
		{
			MigrateUtils.MigrateObscuredTypesInScene("ObscuredString");
		}

		// ---------------------------------------------------------------
		//  GameObject menu items
		// ---------------------------------------------------------------

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + "All detectors", false, 0)]
		private static void AddAllDetectorsToScene()
		{
			AddInjectionDetectorToScene();
			AddObscuredCheatingDetectorToScene();
			AddSpeedHackDetectorToScene();
			AddWallHackDetectorToScene();
			AddTimeCheatingDetectorToScene();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + InjectionDetector.ComponentName, false, 1)]
		private static void AddInjectionDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<InjectionDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + ObscuredCheatingDetector.ComponentName, false, 1)]
		private static void AddObscuredCheatingDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<ObscuredCheatingDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + SpeedHackDetector.ComponentName, false, 1)]
		private static void AddSpeedHackDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<SpeedHackDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + WallHackDetector.ComponentName, false, 1)]
		private static void AddWallHackDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<WallHackDetector>();
		}

		[MenuItem(ACTkEditorConstants.GameObjectMenuPath + TimeCheatingDetector.ComponentName, false, 1)]
		private static void AddTimeCheatingDetectorToScene()
		{
			DetectorTools.SetupDetectorInScene<TimeCheatingDetector>();
		}
	}
}