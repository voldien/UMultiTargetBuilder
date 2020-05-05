using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class BuildConfigTarget
{
// #if UNITY_EDITOR
// 	//Put gui elements for custom inspector here
// 	[HideInInspector, NonSerialized]
// 	public bool Foldout = true;
// #endif
	[Tooltip(""), SerializeField]
	public String outputDirectory;
	[Tooltip("")]
	public String name;
	[Tooltip(""), SerializeField]
	public bool enabled = true;
	[SerializeField, Tooltip("")] // Tooltip("Set of Build Options."), InspectorName("Build Configuration Options")
	public bool useDefaultScenes = true;
	[SerializeField,Obsolete("")]
	public BuildPlayerOptions buildPlayerOptions;
	[SerializeField]
	public EditorBuildSettingsScene[] scenes;
	[SerializeField, Tooltip("")]
	public BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
	[SerializeField, Tooltip("")]
	public BuildTarget target = BuildTarget.StandaloneLinux64;
	[SerializeField, Tooltip("")]
	public BuildOptions options = BuildOptions.None;
}