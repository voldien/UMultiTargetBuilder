using UnityEngine;
using UnityEditor;
using System;

namespace BuildMultiPlatform
{
	[Serializable]
	public class BuildTarget
	{
		[Tooltip("The name of the executable."), SerializeField]
		public string title;
		[Tooltip(""), SerializeField]
		public string company;
		[Tooltip("Relative output directory."), SerializeField]
		public string outputDirectory;
		[Tooltip("The name of the target. (Used in the editor only)")]
		public string name;
		[Tooltip("Enabled for building when invoking build all targets."), SerializeField]
		public bool enabled = true;
		[SerializeField, Tooltip("Use the default scenes specified by the build setting.")] // Tooltip("Set of Build Options."), InspectorName("Build Configuration Options")
		public bool useDefaultScenes = true;
		[SerializeField, Tooltip("List of all scenes when using non default scenes.")]
		public SceneTarget[] scenes;
		[SerializeField, Tooltip("The target group.")]
		public BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
		[SerializeField, Tooltip("Specified target.")]
		public UnityEditor.BuildTarget target = UnityEditor.BuildTarget.StandaloneLinux64;
		[SerializeField, Tooltip("Build options.")]
		public BuildOptions options = BuildOptions.None;
		public string Title { get { if (this.title.Length == 0) return PlayerSettings.productName; else return this.title; } }
	}
}