using System;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal struct FilesProgress
	{
		private float progress;
		private string fileName;

		public static FilesProgress Step(float progress, string fileName)
		{
			return new FilesProgress
			{
				progress = progress,
				fileName = fileName
			};
		}
		
		public static FilesProgress None()
		{
			return new FilesProgress
			{
				progress = -1,
				fileName = null
			};
		}

		public static IProgress<FilesProgress> CreateNew(string header)
		{
			return new Progress<FilesProgress>(value =>
			{
#if UNITY_EDITOR
				if (value.progress >= 0)
					UnityEditor.EditorUtility.DisplayProgressBar($"{header} {value.progress}%", $"{value.fileName} done",
						value.progress);
				else
					UnityEditor.EditorUtility.ClearProgressBar();
#endif
				
			});
		}
	}
}