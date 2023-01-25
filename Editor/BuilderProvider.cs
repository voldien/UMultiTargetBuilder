using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuildMultiPlatform
{
	class BuilderSettingsProvider : SettingsProvider
	{
		[NonSerialized]
		private Vector2 scroll = Vector2.zero;
		private SerializedObject m_BuilderConfigSettings;
		private SerializedProperty m_configurations;
		private SerializedProperty m_outpdirectory;
		private SerializedProperty m_verbose;
		[NonSerialized]
		private ReorderableList m_list;
		[NonSerialized]
		private int selectedConfigIndex = -1;   /*	Select none.	*/

		private GUIStyle errorStyle;
		private GUIStyle warningStyle;

		public class Styles
		{
			public static GUIContent rootOutputDirectory = new GUIContent("Root output directory", "The root directory that all the target build will be stored.");
			public static GUIContent buildConfigs = new GUIContent("Build Configuration Set", "Set of build configuration.");
			public static GUIContent Verbose = new GUIContent("Verbose", "Output is verbosity.");
			public static GUIContent choosePath = new GUIContent("Choose Path", "Choose the output directory for where all the target will be save to.");
			public static GUIContent createPath = new GUIContent("Create Path", "Choose the output directory for where all the target will be save to.");
			/*  */
			public static GUIContent buildTargets = new GUIContent("Build Targets", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build all targets.");
			public static GUIContent buildTargetsScriptOnly = new GUIContent("Build Targets (Scripts", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build all targets script only");
			public static GUIContent build = new GUIContent("Build", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build target.");
			public static GUIContent buildScript = new GUIContent("Script Build", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build target script only");
			public static GUIContent openPath = new GUIContent("Open Path", null, "Open File Explorerer where the targets files are located, if they exists.");
			/*	*/
			public static GUIContent ClearScenes = new GUIContent("Clear Scenes", "");
			public static GUIContent SetDefaultScenes = new GUIContent("Set Default Scenes", "");
			/*	*/
			public static GUIContent run = new GUIContent("Run", (Texture)EditorGUIUtility.IconContent("PlayButton").image, "Run the target.");
			/*	*/
			public static GUIContent export = new GUIContent("Export", "Export settings.");
			public static GUIContent import = new GUIContent("Import", "Import settings.");
			/*	*/
			public static GUIContent addFirstTarget = new GUIContent("Add Target", "Add first target.");
			public static GUIContent add = new GUIContent("Add", "Add additional target.");
			public static GUIContent addCopy = new GUIContent("Add Copy", "Add additional as a copy of the selected.");
			public static GUIContent remove = new GUIContent("Remove", "Remove selected target.");

			public static GUIContent TargetTab = new GUIContent("Targets", "");
			public static GUIContent RunTargets = new GUIContent("Runnable Targets");
		}


		public BuilderSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
			: base(path, scope)
		{
			errorStyle = new GUIStyle();
			errorStyle.normal.textColor = Color.red;

			warningStyle = new GUIStyle();
			warningStyle.normal.textColor = Color.yellow;
		}

		public static bool IsSettingsAvailable()
		{
			return File.Exists(BuilderSettings.GetSettingFilePath());
		}

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			m_BuilderConfigSettings = BuilderSettings.GetSerializedSettings();

			/*	*/
			m_configurations = m_BuilderConfigSettings.FindProperty("targets");
			m_outpdirectory = m_BuilderConfigSettings.FindProperty("rootOutputDirectory");
			m_verbose = m_BuilderConfigSettings.FindProperty("verbose");

			/*	Only create if the options variable found.	*/
			if (m_configurations != null)
			{

				/*	*/
				m_list = new ReorderableList(m_BuilderConfigSettings, m_configurations, true, true, false, false)
				{
					drawHeaderCallback = DrawListHeader,
					drawElementCallback = DrawListElement,
					elementHeightCallback = getElementHeight,
				};
			}

			/*	Autoselect the first if any build target exists.	*/
			if (BuilderSettings.GetOrCreateSettings().targets.Length > 0)
			{
				this.selectedConfigIndex = 0;
				this.m_list.index = this.selectedConfigIndex;// Select(this.selectedConfigIndex);
			}
		}

		private float getElementHeight(int index)
		{
			return (float)EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 1.5f;
		}

		private Texture GetPlatformGroupTexture(SerializedProperty targetGroup)
		{
			string builtin_icon_name = "";
			switch ((BuildTargetGroup)targetGroup.intValue)
			{
				case BuildTargetGroup.Android:
					builtin_icon_name = "BuildSettings.Android.Small";
					break;
				case BuildTargetGroup.Standalone:
					builtin_icon_name = "BuildSettings.Standalone.Small";
					break;
				case BuildTargetGroup.WebGL:
					builtin_icon_name = "BuildSettings.Web.Small";
					break;
				case BuildTargetGroup.iOS:
					builtin_icon_name = "BuildSettings.iPhone.Small";
					break;
				case BuildTargetGroup.WSA:
					builtin_icon_name = "BuildSettings.Web.Small";
					break;
				case BuildTargetGroup.XboxOne:
					builtin_icon_name = "BuildSettings.XboxOne.Small";
					break;
				case BuildTargetGroup.PS4:
					builtin_icon_name = "BuildSettings.PS4.Small";
					break;
				case BuildTargetGroup.tvOS:
					builtin_icon_name = "BuildSettings.iPhone.Small";
					break;
				case BuildTargetGroup.Switch:
					builtin_icon_name = "BuildSettings.Switch.Small";
					break;
				case BuildTargetGroup.Stadia:
					builtin_icon_name = "BuildSettings.Standalone.Small";
					break;
				case BuildTargetGroup.Unknown:
				default:
					/*	Invalid state or could not find the target icon.	*/
					return null;
			}
			return (Texture)EditorGUIUtility.IconContent(builtin_icon_name).image;
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = m_configurations.GetArrayElementAtIndex(index);

			int iconWidth = 35;
			int textWidth = 140;
			int toggleWidth = 15;
			int characterWidth = 10;

			/*	Fetch all properties.   */
			SerializedProperty name = item.FindPropertyRelative("name");
			SerializedProperty enabled = item.FindPropertyRelative("enabled");
			SerializedProperty targetGroup = item.FindPropertyRelative("targetGroup");

			string displayName = "Name:";
			Texture icon = GetPlatformGroupTexture(targetGroup);

			/*	Compute the view rect.  */
			Rect iconRect = new Rect(rect.x + 0, rect.y, iconWidth, EditorGUIUtility.singleLineHeight);
			Rect enabledRect = new Rect(rect.x + iconWidth - 10, rect.y, toggleWidth * 2, EditorGUIUtility.singleLineHeight);
			Rect displayNameRect = new Rect(rect.x + iconWidth + toggleWidth, rect.y, displayName.Length * characterWidth, EditorGUIUtility.singleLineHeight);
			Rect nameRect = new Rect(rect.x + iconWidth + toggleWidth + displayName.Length * characterWidth, rect.y, textWidth, EditorGUIUtility.singleLineHeight);


			/*	*/
			EditorGUI.LabelField(iconRect, new GUIContent(icon, targetGroup.ToString()));
			EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("", "Enable target for building."));
			EditorGUI.LabelField(displayNameRect, displayName);
			EditorGUI.PropertyField(nameRect, name, GUIContent.none);

			/*	If item selected.	*/
			if (isFocused && isActive)
				this.selectedConfigIndex = index;
		}

		private void DrawListHeader(Rect rect)
		{
			//rect = EditorGUI.IndentedRect(rect);
			GUI.Label(rect, string.Format("Build Targets: {0}", this.m_configurations.arraySize), EditorStyles.boldLabel);
		}

		public override void OnDeactivate()
		{

		}

		private void DisplayLeftBuildTargets()
		{
			/*	Begin the vertical left view for adding, selecting and removing build targets.	*/
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(230.0f), GUILayout.ExpandHeight(true));

			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(350.0f), GUILayout.MinWidth(280.0f));
			if (GUILayout.Button(Styles.add))
			{
				/*	Add default instance.	*/
				int index = m_configurations.arraySize;
				m_configurations.InsertArrayElementAtIndex(index);

				m_BuilderConfigSettings.ApplyModifiedProperties();

				/*	Set the default value.	*/
				BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;
				settings.targets[index] = new BuildTarget();
				settings.targets[index].title = PlayerSettings.productName;
				settings.targets[index].name = string.Format("Target {0}", index);
				m_BuilderConfigSettings.ApplyModifiedPropertiesWithoutUndo();
				m_BuilderConfigSettings.Update();

				if (m_configurations.arraySize == 1)
				{
					selectedConfigIndex = 0;
					m_list.index = selectedConfigIndex;	// Select(selectedConfigIndex);
				}

			}

			/*	Determine if any valid item is currently selected.	*/
			bool isItemSelected = selectedConfigIndex >= 0 && selectedConfigIndex < m_configurations.arraySize;

			EditorGUI.BeginDisabledGroup(!isItemSelected);
			if (GUILayout.Button(Styles.addCopy))
			{
				/*	Add default instance.	*/
				int index = m_configurations.arraySize;
				/*	Add copy based on the current selected index.	*/
				m_configurations.InsertArrayElementAtIndex(index);
				m_BuilderConfigSettings.ApplyModifiedProperties();

				BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;
				BuildTarget selected = settings.targets[selectedConfigIndex];
				settings.targets[index] = (BuildTarget)selected.Clone();

				m_BuilderConfigSettings.ApplyModifiedPropertiesWithoutUndo();
				m_BuilderConfigSettings.Update();
			}

			if (GUILayout.Button(Styles.remove))
			{
				/*	Add copy based on tehcurrent selected.	*/
				m_configurations.DeleteArrayElementAtIndex(selectedConfigIndex);
				selectedConfigIndex = selectedConfigIndex - 1;
				m_list.index = selectedConfigIndex; // Select(selectedConfigIndex);
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();

			/*	*/
			EditorGUILayout.Separator();
			m_list.DoLayoutList();

			EditorGUILayout.EndVertical();
			/*	End	the vertical left view for adding, selecting and removing build targets.	*/
		}

		private void DisplayRunList()
		{
			EditorGUILayout.LabelField(Styles.RunTargets, EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("GroupBox");
			using (new EditorGUI.IndentLevelScope(1))
			{
				/*	Iterate through each item.	*/
				BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;
				foreach (BuildTarget target in settings.targets)
				{

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(string.Format("{0}, ({1})", target.name, target.Title));

					EditorGUI.BeginDisabledGroup(!Builder.IsTargetRunable(target));
					if (GUILayout.Button(Styles.run))
					{
						try
						{
							Builder.RunTarget(target);
						}
						catch (Exception ex)
						{
							string dialog_title = string.Format("Failed Running '{}'", target.name);
							string dialog_message = string.Format("Failed running target {0} on the path {1} \n Error {2}", target.name, Builder.GetTargetLocationAbsolutePath(target), ex.Message);
							EditorUtility.DisplayDialog(dialog_title, dialog_message, "OK");
						}
					}
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}
			}

			EditorGUILayout.EndVertical();
		}

		private void DisplayGUIHeader()
		{
			/*	Display global properties on all of the build targets.	*/
			EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(m_outpdirectory, Styles.rootOutputDirectory);
			if (GUILayout.Button(Styles.choosePath))
			{
				string path = EditorUtility.OpenFolderPanel("Choose Output Directory", m_outpdirectory.stringValue, "");
				if (Directory.Exists(path))
				{
					m_outpdirectory.stringValue = path;
				}
			}

			if (!Directory.Exists(m_outpdirectory.stringValue)) ////&& Path.IsPathFullyQualified(m_outpdirectory.stringValue))
			{
				if (GUILayout.Button(Styles.createPath))
				{
					Directory.CreateDirectory(m_outpdirectory.stringValue);
				}
			}

			EditorGUILayout.EndHorizontal();
			if (m_outpdirectory.stringValue.Length != 0)
			{
				if (!Directory.Exists(m_outpdirectory.stringValue))
				{
					EditorGUILayout.HelpBox(string.Format("Directory '{0}' is not a valid directory path.", m_outpdirectory.stringValue), MessageType.Error);
				}
			}

			EditorGUILayout.PropertyField(m_verbose, Styles.Verbose);
		}

		private void DisplayTargetView()
		{

			BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;

			if (m_configurations.arraySize == 0)
			{
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();
				GUILayout.Label("Start Build Configuration by Creating a Target.");

				if (GUILayout.Button(Styles.addFirstTarget))
				{
					/*	Add default instance.	*/
					int index = m_configurations.arraySize;
					m_configurations.InsertArrayElementAtIndex(index);

					m_BuilderConfigSettings.ApplyModifiedProperties();

					/*	Set the default value.	*/
					settings.targets[index] = new BuildTarget();
					settings.targets[index].title = PlayerSettings.productName;
					settings.targets[index].name = string.Format("Target {0}", index);
					m_BuilderConfigSettings.ApplyModifiedPropertiesWithoutUndo();
					m_BuilderConfigSettings.Update();

					if (m_configurations.arraySize == 1)
					{
						selectedConfigIndex = 0;
						m_list.Select(selectedConfigIndex);
					}

				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				GUILayout.FlexibleSpace();
				GUILayout.EndArea();
			}
			else
			{
				/*	*/
				EditorGUILayout.LabelField("Target Settings", EditorStyles.boldLabel);
				EditorGUILayout.BeginHorizontal("Box");

				DisplayLeftBuildTargets();

				//GUILayout.FlexibleSpace();

				/*  */
				if (m_configurations.arraySize > 0)
				{
					EditorGUILayout.BeginVertical("GroupBox",
						 GUILayout.MinHeight(600), GUILayout.MaxHeight(EditorGUI.GetPropertyHeight(m_configurations, true))); //TODO add height from property.	
					EditorGUILayout.LabelField("");

					/*	Displace the build target if selected and is a valid index.	*/
					if (m_configurations.arraySize > 0)
					{
						if (selectedConfigIndex >= 0 && selectedConfigIndex < m_configurations.arraySize)
						{
							/*	Draw main build target property.	*/
							EditorGUILayout.PropertyField(m_configurations.GetArrayElementAtIndex(selectedConfigIndex), GUIContent.none, true);

							using (new EditorGUI.IndentLevelScope(3))
							{
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.Space();

								EditorGUI.BeginDisabledGroup(settings.targets[selectedConfigIndex].useDefaultScenes);
								if (GUILayout.Button(Styles.SetDefaultScenes, GUILayout.MaxWidth(120)))
								{
									SerializedProperty scenes = m_BuilderConfigSettings.FindProperty("scenes");
									EditorBuildSettingsScene[] defScenes = Builder.GetDefaultScenes();
									settings.targets[selectedConfigIndex].scenes = new SceneAsset[defScenes.Length];
									for (int i = 0; i < defScenes.Length; i++)
									{
										settings.targets[selectedConfigIndex].scenes[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(defScenes[i].path);
									}
								}
								if (GUILayout.Button(Styles.ClearScenes, GUILayout.MaxWidth(120)))
								{
									SerializedProperty scenes = m_configurations.GetArrayElementAtIndex(selectedConfigIndex).FindPropertyRelative("scenes");
									scenes.ClearArray();
									//
								}
								EditorGUI.EndDisabledGroup();
								EditorGUILayout.EndHorizontal();

								/*	Draw build buttons.	*/
								BuildTarget optionItem = BuilderSettings.GetOrCreateSettings().targets[selectedConfigIndex];
								bool isTargetSupported = Builder.IsBuildTargetSupported(optionItem);
								bool isValidConfig = true;
								if (!isTargetSupported)
								{
									EditorGUILayout.LabelField("Target Group and Target is not valid configuration", this.errorStyle);
								}

								EditorGUILayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup(!(isTargetSupported && isValidConfig));
								if (GUILayout.Button(Styles.build, GUILayout.MaxWidth(120)))
								{
									Builder.BuildTarget(optionItem);
								}
								if (GUILayout.Button(Styles.buildScript, GUILayout.MaxWidth(120)))
								{
									Builder.BuildTargetScriptOnly(optionItem);
								}

								/*	*/
								EditorGUI.BeginDisabledGroup(!Directory.Exists(Path.GetDirectoryName(Builder.GetTargetLocationAbsolutePath(optionItem))));
								if (GUILayout.Button(Styles.openPath, GUILayout.MaxWidth(120)))
								{
									EditorUtility.RevealInFinder(Builder.GetTargetLocationAbsolutePath(optionItem));
								}
								EditorGUI.EndDisabledGroup();

								EditorGUI.EndDisabledGroup();
								EditorGUILayout.EndHorizontal();



								try
								{
									string outputPathLabel = string.Format("Executable filepath: {0}", Builder.GetTargetLocationAbsolutePath(optionItem), EditorStyles.boldLabel);
									EditorGUILayout.LabelField(outputPathLabel);
								}
								catch (Exception ex)
								{
									Color currentColor = EditorStyles.label.normal.textColor;
									EditorStyles.label.normal.textColor = Color.red;
									EditorGUILayout.LabelField(string.Format("Invalid setttings: {0}.", ex.Message));
									EditorStyles.label.normal.textColor = currentColor;
								}
							}
						}
					}

					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndHorizontal();


				EditorGUILayout.Separator();
			}
		}

		private void DisplayRunable()
		{
			BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;
			/*  Draw quick run UI.	*/
			if (m_configurations.arraySize > 0)
			{
				DisplayRunList();
			}
		}

		private int tab = 0;
		public override void OnGUI(string searchContext)
		{

			m_BuilderConfigSettings.Update();
			BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;

			using (new EditorGUI.IndentLevelScope(1))
			{

				/*	*/
				this.scroll = EditorGUILayout.BeginScrollView(scroll, false, true);

				/*	*/
				DisplayGUIHeader();

				EditorGUILayout.Space();

				tab = GUILayout.Toolbar(tab, new GUIContent[] { Styles.TargetTab, Styles.RunTargets });
				switch (tab)
				{
					default:
					case 0:
						DisplayTargetView();
						break;
					case 1:
						DisplayRunable();
						break;
				}

				EditorGUILayout.EndScrollView();

				/*	Summary information.	*/
				EditorGUILayout.BeginVertical();
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(string.Format("Number of targets: {0}", settings.targets.Length.ToString()));
				EditorGUILayout.LabelField(string.Format("Number of enabled build targets: {0}", GetNrEnabledTargets()));


				/*	Check if any has invalid.	*/
				int nbadpath = 0;
				int ntargetpath = 0;
				foreach (BuildTarget target in settings.targets)
				{
					if (!Builder.IsBuildTargetSupported(target))
					{
						ntargetpath++;
					}
					if (Builder.IsValidTargetPath(target))
					{
						nbadpath++;
					}
				}
				if (nbadpath > 0)
				{
					EditorGUILayout.LabelField(string.Format("Number of invalid path targets: {0}", nbadpath));
				}
				if (ntargetpath > 0)
				{
					EditorGUILayout.LabelField(string.Format("Number of invalid target configuratons: {0}", ntargetpath));
				}


				EditorGUILayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup(m_configurations.arraySize == 0);
				/*	Build all buttons.	*/
				if (GUILayout.Button(Styles.buildTargets))
				{
					Builder.BuildFromConfig((BuilderSettings)settings);
				}
				if (GUILayout.Button(Styles.buildTargetsScriptOnly))
				{
					Builder.BuildFromConfigScriptOnly((BuilderSettings)settings);
				}


				/*	Export and import buttons.	*/
				const string ext = "asset";
				if (GUILayout.Button(Styles.export))
				{
					/*	Export.	*/
					string path = EditorUtility.SaveFilePanel("Choose export file path", Directory.GetCurrentDirectory(), string.Format("{0}", Application.productName), ext);
					/*	*/
					if (path.Length != 0)
					{
						try
						{
							BuilderIO.SaveConfigSetting(path);
						}
						catch (Exception ex)
						{
							EditorUtility.DisplayDialog("Error when exporting the settings", ex.Message, "Ok");
						}
						finally
						{

						}
					}
				}
				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button(Styles.import))
				{
					/*	Import.	*/
					string path = EditorUtility.OpenFilePanel("Choose import file path", Directory.GetCurrentDirectory(), ext);
					/*	*/
					if (path.Length != 0)
					{
						try
						{
							BuilderIO.LoadConfigSetting(path);
						}
						catch (Exception ex)
						{
							EditorUtility.DisplayDialog("Error when importing settings", ex.Message, "Ok");
						}
						finally
						{

						}

					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				m_BuilderConfigSettings.ApplyModifiedProperties();
			}   /*	Reset indent.	*/
		}

		private int GetNrEnabledTargets()
		{
			int nth = 0;
			BuilderSettings settings = (BuilderSettings)m_BuilderConfigSettings.targetObject;
			if (m_configurations.isArray)
			{
				foreach (BuildTarget target in settings.targets)
				{
					if (target.enabled)
						nth++;
				}
			}
			return nth;
		}

#if UNITY_2018_1_OR_NEWER
		[SettingsProvider]
		public static SettingsProvider CreateBuildConfigurationSettingsProvider()
		{
			if (!IsSettingsAvailable())
			{
				/*	Create setting object if it does not exist.	*/
				BuilderSettings.GetOrCreateSettings();
			}
			SettingsProvider provider = new BuilderSettingsProvider("Project/MultiTarget Build Settings", SettingsScope.Project);

			provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
			return provider;
		}
#endif
		[MenuItem("Window/MultiTarget Build/Settings", false, 1)]
		public static void OpenSettings()
		{
			SettingsService.OpenProjectSettings("Project/MultiTarget Build Settings");
		}
	}
}