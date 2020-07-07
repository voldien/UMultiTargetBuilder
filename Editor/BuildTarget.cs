using UnityEngine;
using UnityEditor;
using System;

namespace BuildMultiPlatform
{
	[Serializable]
	public class BuildTarget : ICloneable
	{
		[Tooltip("The name of the executable."), SerializeField]
		public string title = "";
		[Tooltip(""), SerializeField]
		public string company = "";
		[Tooltip("Relative output directory."), SerializeField]
		public string outputDirectory = "";
		[Tooltip("The name of the target. (Used in the editor only)")]
		public string name = "";
		[Tooltip("Enabled for building when invoking build all targets."), SerializeField]
		public bool enabled = true;
		[SerializeField, Tooltip("Use the default scenes specified by the build setting.")]
		public bool useDefaultScenes = true;
		[SerializeField, Tooltip("List of all scenes when using non default scenes.")]
		public SceneAsset[] scenes = new SceneAsset[0];
		[SerializeField, Tooltip("The target group.")]
		public BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
		[SerializeField, Tooltip("Specified target.")]
		public UnityEditor.BuildTarget target = UnityEditor.BuildTarget.StandaloneLinux64;
		[SerializeField, Tooltip("Build options.")]
		public BuildOptions options = BuildOptions.None;


		public string Title { get { if (this.title.Length == 0) return PlayerSettings.productName; else return this.title; } }
		public object Clone()
		{
			BuildTarget copy = new BuildTarget();
			copy.title = this.title;
			copy.company = this.company;
			copy.enabled = this.enabled;
			copy.outputDirectory = this.outputDirectory;
			copy.name = this.name;
			copy.target = this.target;
			copy.targetGroup = this.targetGroup;
			copy.useDefaultScenes = this.useDefaultScenes;
			copy.options = this.options;
			copy.scenes = (SceneAsset[])this.scenes.Clone();
			return (object)copy;
		}
	}

}