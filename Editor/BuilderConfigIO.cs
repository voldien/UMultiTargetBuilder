using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using UnityEditor;

namespace BuildMultiPlatform
{
    public static class BuilderConfigIO
    {
        //TODO Change to use AssetDatabase, created copy, get the fully path and use system file move.
        // For load, change the editor path of settings and invoke update asset.
        public static void LoadConfigSetting(string path)
        {
			if (File.Exists(path))
            {
				/*  Dialog.*/
				string assetPath = BuilderConfigSettings.GetSettingFilePath();
				string projectPath = Application.dataPath.Replace("/Assets", "");   //TODO improve.
				string FullPath = string.Format("{0}/{1}", projectPath, assetPath);
                if(File.Exists(FullPath)){
                    if(EditorUtility.DisplayDialog("Overwrite", "Are you sure you want to overwrite the settings", "Yes", "No")){
    					File.Copy(path, FullPath, true);
						AssetDatabase.ImportAsset(assetPath);
					}
				}else
					throw new ArgumentException(string.Format("Invalid path {0}", FullPath));
            }
            else
				throw new ArgumentException(string.Format("Invalid path {0}", path));
		}

        public static void SaveConfigSetting(string path)
        {
			string assetPath = BuilderConfigSettings.GetSettingFilePath();
			File.Copy(assetPath, path);
        }
    }
}