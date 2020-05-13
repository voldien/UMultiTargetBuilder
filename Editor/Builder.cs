using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace BuildMultiPlatform
{

	public static class Builder
	{

		[PostProcessBuild(1)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			Debug.Log(pathToBuiltProject);
		}

		[MenuItem("Build/Default Build", false, 1)]
		public static void PerformDefaultBuildContext()
		{

			BuildConfigTarget defaultTarget = new BuildConfigTarget();
			defaultTarget.name = "Default";
			//defaultTarget.scenes = getDefaultScenes();
			defaultTarget.target = EditorUserBuildSettings.activeBuildTarget;
			defaultTarget.targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			defaultTarget.options = BuildOptions.None;
			Builder.BuildTarget(defaultTarget);
		}


		[MenuItem("Build/Build Targets", true, 0)]
		public static bool ValidatePerformBuildContext()
		{
			return BuilderConfigSettings.GetOrCreateSettings().options.Length > 0;
		}

		[MenuItem("Build/Build Targets", false, 0)]
		public static void PerformBuildContext()
		{
			BuilderConfigSettings config = BuilderConfigSettings.GetOrCreateSettings();
			BuildFromConfig(config);
		}

		public static void PerformDefaultBuild()
		{
			/*	Find the default.	*/
		}

		public static void BuildFromConfig(BuilderConfigSettings config)
		{
			/*	*/
			BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			try
			{
				foreach (BuildConfigTarget buildConfigtarget in config.options)
				{
					InternalBuildTarget(buildConfigtarget);
				}
			}
			finally
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}
		}

		public static string GetTargetLocationAbsolutePath(BuildConfigTarget target)
		{
			BuilderConfigSettings settings = BuilderConfigSettings.GetOrCreateSettings();
			string path = null;

			/*	Determine the title.	*/
			string titleName = target.Title;
			// if(target.title.Length == 0)
			// 	titleName = PlayerSettings.productName;
			// else
			// 	titleName = target.title;

			/*	Compute the output filepath.	*/
			if (target.outputDirectory.Length > 0)
				path = Path.GetFullPath(string.Format("{0}/{1}/{2}", settings.rootOutputDirectory, target.outputDirectory, titleName));
			else
				path = Path.GetFullPath(string.Format("{0}/{1}", settings.rootOutputDirectory, titleName));

			if (!Path.HasExtension(path))
			{
				switch (target.target)
				{
					case UnityEditor.BuildTarget.StandaloneWindows:
					case UnityEditor.BuildTarget.StandaloneWindows64:
						path = string.Format("{0}{1}", path, ".exe");
						break;
					case UnityEditor.BuildTarget.StandaloneLinux64:
						path = string.Format("{0}{1}", path, ".x86_64");
						break;
					case UnityEditor.BuildTarget.Android:
						path = string.Format("{0}{1}", path, ".apk");
						break;
					default:
						break;
				}
			}

			/*	Validate the path is an absolute path.	*/
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			else
			{
				return path;
			}
		}

		public static void RunTarget(BuildConfigTarget target)
		{
			/*	*/
			BuilderConfigSettings settings = BuilderConfigSettings.GetOrCreateSettings();
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.FileName = GetTargetLocationAbsolutePath(target);
			proc.Start();
		}

		public static bool IsTargetRunable(BuildConfigTarget target)
		{
			/*	*/
			string path = GetTargetLocationAbsolutePath(target);
			if (File.Exists(path))
			{
				return true;
			}
			return false;
		}


		internal static void InternalBuildTarget(BuildConfigTarget buildTarget)
		{

			BuilderConfigSettings settings = BuilderConfigSettings.GetOrCreateSettings();
			if (buildTarget.enabled)
			{
				/*	Determine if supported on the target unity modules.	*/
				if (!Builder.isBuildTargetSupported(buildTarget))
				{
					Debug.LogError("Not build target " + buildTarget.name + "Not supported to be built with this Unity");
				}

				/*	Populate the build struct with the build target configuration.	*/
				EditorBuildSettingsScene[] targetScenes = getDefaultScenes();
				if (buildTarget.scenes == null)
				{
					// Use the default scenes.
				}
				else
				{
					if (buildTarget.scenes.Length > 0)
					{
						//targetScenes = buildTarget.scenes;
					}
				}
				BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
				buildPlayerOptions.locationPathName = GetTargetLocationAbsolutePath(buildTarget);
				buildPlayerOptions.scenes = new string[targetScenes.Length];
				buildPlayerOptions.target = buildTarget.target;
				buildPlayerOptions.targetGroup = buildTarget.targetGroup;
				buildPlayerOptions.options = buildTarget.options;

				int i = 0;
				foreach (EditorBuildSettingsScene scene in targetScenes)
				{
					buildPlayerOptions.scenes[i] = scene.path;
					i++;
				}

				BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
				BuildSummary summary = report.summary;


				/*	Present verbose result.	*/
				if (settings.verbose)
				{

				}
				Debug.LogFormat("", summary.totalTime.Seconds, summary.buildStartedAt, summary.buildEndedAt);
				switch (summary.result)
				{
					case BuildResult.Succeeded:
						Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
						break;
					case BuildResult.Failed:
						Debug.Log("Build failed");
						break;
					case BuildResult.Cancelled:
						Debug.Log("Build Cancled");
						break;
					default:
					case BuildResult.Unknown:
						Debug.Log("Build Unknown");
						break;
				}

				Debug.Log("Platform: " + summary.platform.ToString());
				Debug.LogFormat("", summary.options.ToString(), summary.platformGroup.ToString());
			}
		}

		//TODO relocate to the internal build and put the try finally in this code block.
		public static void BuildTarget(BuildConfigTarget buildTarget)
		{
			BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			try
			{
				InternalBuildTarget(buildTarget);
			}
			finally
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}
		}

		public static bool isBuildTargetSupported(BuildConfigTarget configOptionItem)
		{
			if (BuildPipeline.IsBuildTargetSupported(configOptionItem.targetGroup, configOptionItem.target))
			{
				/*	TODO add additional check.	*/
				return true;
			}
			return false;
		}

		static EditorBuildSettingsScene[] getDefaultScenes()
		{
			return EditorBuildSettings.scenes;
		}
	}

}