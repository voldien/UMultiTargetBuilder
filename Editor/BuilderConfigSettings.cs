using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildMultiPlatform
{
	[Serializable]
	public class BuilderConfigSettings : ScriptableObject
	{


		[Tooltip("Root Directory where all build will be located"), SerializeField]
		public string rootOutputDirectory;
		[SerializeField, HideInInspector, Tooltip("Root Directory where all build will be located")]
		public BuildConfigTarget[] options;

		[SerializeField, Tooltip("Verbosity of the building process")]
		public bool verbose;

		public static string GetSettingFilePath()
		{
			//TOOD handle the path for the asset file.
			return "Assets/Editor/BuildConfigSettings.asset";
		}

		internal static BuilderConfigSettings GetOrCreateSettings()
		{
			BuilderConfigSettings settings = AssetDatabase.LoadAssetAtPath<BuilderConfigSettings>(GetSettingFilePath());
			if (settings == null)
			{
				/*	Create default setting object.	*/
				settings = ScriptableObject.CreateInstance<BuilderConfigSettings>();
				settings.rootOutputDirectory = "";
				settings.verbose = true;
				AssetDatabase.CreateAsset(settings, GetSettingFilePath());
				AssetDatabase.SaveAssets();
			}

			return settings;
		}


		public static bool IsSettingsAvailable()
		{
			return File.Exists(GetSettingFilePath());
		}


		[MenuItem("Build/Config/Generate Config", true, 15)]
		public static bool IsSettingsAvailableContextMenu()
		{
			return IsSettingsAvailable() == false;
		}

		[MenuItem("Build/Config/Generate Config", false, 15)]
		public static void GenerateConfig()
		{
			BuilderConfigSettings.GetOrCreateSettings();
			/*	Update the provider.	*/
		}

		public static BuilderConfigSettings loadFromPath(string path)
		{
			return null;
		}

		internal static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}
	}
}