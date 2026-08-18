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
		[Tooltip("The company name that will added to the executable."), SerializeField]
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
		public bool UseAdditionalScenes = false;
		[SerializeField, Tooltip("Overrided List of Scenes.")]
		public SceneAsset[] scenes = new SceneAsset[0];
		[SerializeField, Tooltip("List of Additional Scene to Add to Build.")]
		public SceneAsset[] additionalScenes = new SceneAsset[0];
		[SerializeField, Tooltip("The target group.")]
		public BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
		[SerializeField, Tooltip("Specified target.")]
		public UnityEditor.BuildTarget target = UnityEditor.BuildTarget.StandaloneLinux64;
		[SerializeField, Tooltip("Build options.")]
		public BuildOptions options = BuildOptions.None;

		[Tooltip("Allow to enable or disable using additional scripting definitions")]
		public bool UseAdditionalScriptingDefines = false;
		[Header("Compiler Additional Definitions")]
		[SerializeField, Tooltip("Predefined C# Macro applied to the build target during compilation.")]
		public string[] ScriptingDefines;

		public string Title { get { if (title.Length == 0) { return PlayerSettings.productName; } else { return title; } } }
		public object Clone()
		{
			BuildTarget copy = new BuildTarget();
			copy.title = title;
			copy.company = company;
			copy.enabled = enabled;
			copy.outputDirectory = outputDirectory;
			copy.name = name;
			copy.target = target;
			copy.targetGroup = targetGroup;
			copy.useDefaultScenes = useDefaultScenes;
			copy.options = options;
			copy.ScriptingDefines = ScriptingDefines;
			copy.scenes = (SceneAsset[])scenes.Clone();
			copy.additionalScenes = (SceneAsset[])additionalScenes.Clone();
			return (object)copy;
		}
	}

}
