using System;
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
			//TODO add support for adding post processing action event.
			Debug.Log(pathToBuiltProject);
		}


		[MenuItem("Build/Builder/Default Build", false, 1)]
		public static void PerformDefaultBuildContext()
		{
			BuildTarget defaultTarget = new BuildTarget();
			defaultTarget.name = "Default";
			defaultTarget.target = EditorUserBuildSettings.activeBuildTarget;
			defaultTarget.targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			defaultTarget.options = BuildOptions.None;
			defaultTarget.outputDirectory = EditorUserBuildSettings.GetBuildLocation(defaultTarget.target);
			Builder.BuildTarget(defaultTarget);
		}

		[MenuItem("Build/Builder/Build Targets", true, 0)]
		public static bool ValidatePerformBuildContext()
		{
			BuilderSettings setting = BuilderSettings.GetOrCreateSettings();
			return setting.targets.Length > 0 && Directory.Exists(setting.rootOutputDirectory);
		}

		[MenuItem("Build/Builder/Build Targets", false, 0)]
		public static void PerformBuildContext()
		{
			BuilderSettings config = BuilderSettings.GetOrCreateSettings();
			BuildFromConfig(config);
		}

		[MenuItem("Build/Builder/Build Targets (Script only)", true, 1)]
		public static bool ValidatePerformBuildScriptOnlyContext()
		{
			return ValidatePerformBuildContext();
		}

		[MenuItem("Build/Builder/Build Targets (Script only)", false, 1)]
		public static void PerformBuildScriptOnlyContext()
		{
			BuilderSettings config = BuilderSettings.GetOrCreateSettings();
			BuildFromConfigScriptOnly(config);
		}

		public static void BuildFromConfig(BuilderSettings settings)
		{
			/*	Remember the state of the current build target.	*/
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

			try
			{
				for (int i = 0; i < settings.targets.Length; i++)
				{
					InternalBuildTarget(settings.targets[i]);
				}
			}
			finally
			{
				/*	Reset the build setting to the original state.	*/
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}
		}

		public static void BuildFromConfigScriptOnly(BuilderSettings settings)
		{
			/*	Remeber the state of the current build target.	*/
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			try
			{
				for (int i = 0; i < settings.targets.Length; i++)
				{
					InternalBuildScriptOnly(settings.targets[i]);
				}
			}
			finally
			{
				/*	Reset the build setting to the original state.	*/
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}

		}

		public static bool IsValidTargetPath(BuildTarget target)
		{
			string path = GetTargetLocationAbsolutePath(target);
			return Directory.Exists(path);
		}

		public static string GetTargetLocationAbsolutePath(BuildTarget target)
		{
			BuilderSettings settings = BuilderSettings.GetOrCreateSettings();

			string path = null;
			string root = settings.rootOutputDirectory;

			if (root.Length == 0)
			{
				/*	Create default directory from user home directory.	*/
				root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), PlayerSettings.productName);// string.Format("{0}/{1}", );
			}

			/*	Check if target path is rooted or relative.	*/

			/*	Compute the output filepath.	*/
			if (target.outputDirectory.Length > 0)
			{
				path = Path.GetFullPath(Path.Combine(Path.Combine(root, target.outputDirectory), target.Title));// string.Format("{0}/{1}/{2}", root, target.outputDirectory, target.Title));
			}
			else
			{
				path = Path.GetFullPath(Path.Combine(root, target.Title));
			}

			/*	Add extension in order to make the target work properly in its environment.	*/
			if (!Path.HasExtension(target.Title))
			{
				switch (target.target)
				{
					case UnityEditor.BuildTarget.StandaloneWindows:
					case UnityEditor.BuildTarget.StandaloneWindows64:
						path = string.Format("{0}{1}", path, ".exe");
						break;
					case UnityEditor.BuildTarget.StandaloneOSX:
						path = string.Format("{0}{1}", path, ".app");
						break;
					case UnityEditor.BuildTarget.StandaloneLinux64:
						path = string.Format("{0}{1}", path, ".x86_64");
						break;
					case UnityEditor.BuildTarget.Android:
						path = string.Format("{0}{1}", path, ".apk");
						break;
					case UnityEditor.BuildTarget.iOS:
						path = string.Format("{0}{1}", path, ".ipa");
						break;
					case UnityEditor.BuildTarget.WebGL:
						path = string.Format("{0}{1}", path, ".html");
						break;
					case UnityEditor.BuildTarget.WSAPlayer:
						path = string.Format("{0}{1}", path, ".appx");
						break;
					//TODO add support for other platforms.
					case UnityEditor.BuildTarget.PS4:
					case UnityEditor.BuildTarget.XboxOne:
					case UnityEditor.BuildTarget.tvOS:
					case UnityEditor.BuildTarget.Switch:
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
				throw new Exception("Could not construct valid global path.");
			}

		}

		public static void RunTarget(BuildTarget target)
		{
			/*	*/
			BuilderSettings settings = BuilderSettings.GetOrCreateSettings();
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.FileName = GetTargetLocationAbsolutePath(target);
			proc.Start();
		}

		public static bool IsTargetRunable(BuildTarget target)
		{
			/*	*/
			try
			{
				string path = GetTargetLocationAbsolutePath(target);
				if (File.Exists(path))
				{
					/*	TODO add support if path is executable and what platform the target is in respect to current.	*/
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		internal static void InternalBuildScriptOnly(BuildTarget buildTarget)
		{
			/*  Create an copy with no reference to the original buildTarget object.   */
			BuildTarget targetCopy = (BuildTarget)buildTarget.Clone();
			targetCopy.options |= BuildOptions.BuildScriptsOnly;
			InternalBuildTarget(targetCopy);
		}

		internal static void InternalBuildTarget(BuildTarget buildTarget)
		{
			BuilderSettings settings = BuilderSettings.GetOrCreateSettings();
			if (buildTarget.enabled)
			{
				/*	Determine if supported on the target unity modules.	*/
				if (!Builder.IsBuildTargetSupported(buildTarget))
				{
					Debug.LogError(string.Format("Build target {0} is not supported to be built with this Unity Version (Check if Modules are installed).", buildTarget.name));
					/*	Break from continue executing.	*/
					return;
				}

				if (IsValidTargetPath(buildTarget))
				{
					Debug.LogError(string.Format("Filepath: '{0}' is not valid.", GetTargetLocationAbsolutePath(buildTarget)));
					return;
				}

				/*	Populate the build struct with the build target configuration.	*/
				EditorBuildSettingsScene[] targetScenes = GetDefaultScenes();
				if (!buildTarget.useDefaultScenes)
				{

					/*	Create scenes for building.	*/
					targetScenes = new EditorBuildSettingsScene[buildTarget.scenes.Length];
					for (int j = 0; j < targetScenes.Length; j++)
					{
						string path = AssetDatabase.GetAssetPath(buildTarget.scenes[j]);
						Debug.Log(path);
						targetScenes[j] = new EditorBuildSettingsScene(path, true);
					}
				}


				/*	Pass argument to the building options.	*/
				BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
				buildPlayerOptions.locationPathName = GetTargetLocationAbsolutePath(buildTarget);
				buildPlayerOptions.scenes = new string[targetScenes.Length];
				buildPlayerOptions.target = buildTarget.target;
				buildPlayerOptions.targetGroup = buildTarget.targetGroup;
				buildPlayerOptions.options = buildTarget.options;
				buildPlayerOptions.extraScriptingDefines = buildTarget.ScriptingDefines;


				/*	assign all scenes.	*/
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
				Debug.LogFormat("{0} {1} {2}", summary.totalTime.Seconds, summary.buildStartedAt, summary.buildEndedAt);
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
				Debug.LogFormat("Build Options: {0} | Platform: {1}", summary.options.ToString(), summary.platformGroup.ToString());
			}
		}

		public static void BuildTarget(BuildTarget buildTarget)
		{
			/*	*/
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			try
			{
				InternalBuildTarget(buildTarget);
			}
			finally
			{
				/*	*/
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}
		}

		public static void BuildTargetScriptOnly(BuildTarget buildTarget)
		{
			/*	*/
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			StackPushBuildConfiguration();
			try
			{
				InternalBuildScriptOnly(buildTarget);
			}
			finally
			{
				/*	*/
				StackPopBuildConfiguration();
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
			}
		}

		public static bool IsBuildTargetSupported(BuildTarget configOptionItem)
		{
			if (BuildPipeline.IsBuildTargetSupported(configOptionItem.targetGroup, configOptionItem.target))
			{
				/*	TODO add additional check.	*/
				return true;
			}
			return false;
		}

		internal static EditorBuildSettingsScene[] GetDefaultScenes()
		{
			return EditorBuildSettings.scenes;
		}

		//TODO add support for increase cohesion.
		internal static void StackPushBuildConfiguration()
		{
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
		}

		internal static void StackPopBuildConfiguration()
		{
			UnityEditor.BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

			EditorUserBuildSettings.SwitchActiveBuildTargetAsync(currentGroup, currentTarget);
		}
	}

}