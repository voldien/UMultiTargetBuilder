using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

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
		defaultTarget.scenes = getDefaultScenes();
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
		foreach (BuildConfigTarget buildConfigtarget in config.options)
		{
			BuildTarget(buildConfigtarget);
		}
	}

	public static string GetTargetLocationAbsolutePath(BuildConfigTarget target)
	{
		BuilderConfigSettings settings = BuilderConfigSettings.GetOrCreateSettings();
		if (target.outputDirectory.Length > 0)
			return Path.GetFullPath(string.Format("{0}/{1}/{2}", settings.rootOutputDirectory, target.outputDirectory, PlayerSettings.productName));
		else
			return Path.GetFullPath(string.Format("{0}/{1}", settings.rootOutputDirectory, PlayerSettings.productName));
	}

	public static void RunTarget(BuildConfigTarget target)
	{
		BuilderConfigSettings settings = BuilderConfigSettings.GetOrCreateSettings();
		System.Diagnostics.Process proc = new System.Diagnostics.Process();
		proc.StartInfo.FileName = GetTargetLocationAbsolutePath(target);
		proc.Start();
	}

	public static void BuildTarget(BuildConfigTarget buildTarget)
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
					targetScenes = buildTarget.scenes;
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

	public static bool isBuildTargetSupported(BuildConfigTarget configOptionItem)
	{
		if (BuildPipeline.IsBuildTargetSupported(configOptionItem.targetGroup, configOptionItem.target))
		{
			/*	TODO add additional check.	*/
			return true;
		}
		return false;
	}

	internal static EditorBuildSettingsScene[] getDefaultScenes()
	{

		return EditorBuildSettings.scenes;
	}
}
