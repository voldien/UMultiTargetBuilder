using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildMultiPlatform
{

	public static class AndroidUtil
	{
		public static List<string> RefreshDevices()
		{
			List<string> devices = new List<string>();
			try
			{
				/*	Query device list.*/
				string errrMesg = "";
				string extractedDeviceList = UnityEditor.Android.ADB.GetInstance().Run(new string[] { "devices" }, errrMesg);

				/*	*/
				string[] deviceLines = extractedDeviceList.Split(Environment.NewLine.ToCharArray());

				for (int i = 1; i < deviceLines.Length; i++)    /*	Ignore the first verbose line.	*/
				{

					/*	*/
					int length = deviceLines[i].IndexOfAny(new char[] { '\t', ' ' }, 0);
					if (length > 0)
					{
						string resultDevice = deviceLines[i].Substring(0, length);

						if (resultDevice.Length > 0)
						{

							devices.Add(resultDevice);
						}
					}

				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}

			return devices;
		}
	}
}