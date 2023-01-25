using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildMultiPlatform
{
	//TODO rename.
	[Serializable]
	public class BuilderSettings : ScriptableObject
	{
		[Tooltip("Root Directory where all build will be located"), SerializeField]
		public string rootOutputDirectory;
		[SerializeField, HideInInspector, Tooltip("Root Directory where all build will be located")]
		public BuildTarget[] targets;

		[SerializeField, Tooltip("Verbosity of the building process")]
		public bool verbose;

		public static string GetSettingFilePath()
		{
			return "Assets/Editor/com.linuxsenpai.multitargetbuilder/BuildSettings.asset";
		}

		internal static BuilderSettings GetOrCreateSettings()
		{
			BuilderSettings settings = AssetDatabase.LoadAssetAtPath<BuilderSettings>(GetSettingFilePath());
			if (settings == null)
			{
				/*	Create and make sure the directory exits.	*/
				if (!AssetDatabase.IsValidFolder("Assets/Editor"))
				{
					string guid = AssetDatabase.CreateFolder("Assets", "Editor");
				}
				if (!AssetDatabase.IsValidFolder("Assets/Editor/com.linuxsenpai.multitargetbuilder"))
				{
					string guid1 = AssetDatabase.CreateFolder("Assets/Editor", "com.linuxsenpai.multitargetbuilder");
				}
				/*	Create default setting object.	*/
				settings = ScriptableObject.CreateInstance<BuilderSettings>();
				settings.rootOutputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), PlayerSettings.productName); // string.Format("{0}/{1}",
				settings.verbose = true;
				settings.targets = new BuildTarget[0];
				AssetDatabase.CreateAsset(settings, GetSettingFilePath());
				AssetDatabase.SaveAssets();
			}

			return settings;
		}


		public static bool IsSettingsAvailable()
		{
			return File.Exists(GetSettingFilePath());
		}


		[MenuItem("Build/Builder/Generate Config", true, 15)]
		public static bool IsSettingsAvailableContextMenu()
		{
			return IsSettingsAvailable() == false;
		}

		[MenuItem("Build/Builder/Generate Config", false, 15)]
		public static void GenerateConfig()
		{
			BuilderSettings.GetOrCreateSettings();
		}

		internal static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}
	}
}