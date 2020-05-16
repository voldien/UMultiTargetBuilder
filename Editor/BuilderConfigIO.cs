using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

namespace BuildMultiPlatform
{
    public static class BuilderConfigIO
    {
        public static BuilderConfigSettings LoadConfigSetting(string path)
        {
            if (File.Exists(path))
            {
				/*	*/
                BinaryFormatter binary = new BinaryFormatter();
                /*  */
                FileStream stream = File.Open(path, FileMode.Open);

                BuilderConfigSettings settings = (BuilderConfigSettings)binary.Deserialize(stream);
                stream.Close();
				
                return settings;
            }
			throw new ArgumentException("");
		}

        public static void SaveConfigSetting(string path, BuilderConfigSettings settings)
        {
            BinaryFormatter binary = new BinaryFormatter();

            FileStream stream = File.Open(path, FileMode.OpenOrCreate);
			/*	*/
            binary.Serialize(stream, settings);
            stream.Close();
        }
    }
}