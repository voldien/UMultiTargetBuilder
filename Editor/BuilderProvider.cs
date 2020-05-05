using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

class BuilderSettingsProvider : SettingsProvider
{
	[NonSerialized]
	private Vector2 scroll = Vector2.zero;
	private SerializedObject m_BuilderConfigSettings;
	private SerializedProperty configurations;
	private SerializedProperty _outpdirectory;
	private SerializedProperty _verbose;
	private ReorderableList _list;
	[NonSerialized]
	private int selectedConfigIndex = -1;

	class Styles
	{
		public static GUIContent rootOutputDirectory = new GUIContent("Root output directory", "The root directory that all the target build will be stored.");
		public static GUIContent buildConfigs = new GUIContent("Build Configuration Set", "Set of build configuration.");
		public static GUIContent Verbose = new GUIContent("Verbose", "Output is verbosity.");
	}

	const string k_BuilderConfigSettingsPath = "Assets/Editor/BuildConfigSettings.asset";

	public BuilderSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
		: base(path, scope) { }

	public static bool IsSettingsAvailable()
	{
		return File.Exists(BuilderConfigSettings.GetSettingFilePath());
	}

	public override void OnActivate(string searchContext, VisualElement rootElement)
	{
		m_BuilderConfigSettings = BuilderConfigSettings.GetSerializedSettings();
		configurations = m_BuilderConfigSettings.FindProperty("options");
		_outpdirectory = m_BuilderConfigSettings.FindProperty("rootOutputDirectory");
		_verbose = m_BuilderConfigSettings.FindProperty("verbose");
		if (configurations != null)
		{

			/*	TODO validate.	*/
			_list = new ReorderableList(m_BuilderConfigSettings, configurations, true, true, false, false)
			{
				drawHeaderCallback = DrawListHeader,
				drawElementCallback = DrawListElement,
				elementHeightCallback = getElementHeight,
			};
		}

		/*	Autoselect the first if any build target exists.	*/
		if (BuilderConfigSettings.GetOrCreateSettings().options.Length > 0)
			this.selectedConfigIndex = 0;
	}

	private float getElementHeight(int index)
	{
		return (float)EditorGUIUtility.singleLineHeight;
	}

	private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		var item = configurations.GetArrayElementAtIndex(index);

		int textWidth = 140;

		string displayName = "Name:" + item.FindPropertyRelative("name").stringValue;
		EditorGUI.LabelField(new Rect(rect.x + 0, rect.y, textWidth, EditorGUIUtility.singleLineHeight), displayName);
		/*	*/
		if (isFocused && isActive)
			this.selectedConfigIndex = index;
	}

	private void DrawListHeader(Rect rect)
	{
		GUI.Label(rect, "Build Targets");
	}

	public override void OnDeactivate()
	{

	}

	private void DisplayLeftBuildTargets()
	{
		/*	Begin the vertical left view for adding, selecting and removing build targets.	*/
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(180.0f));

		/*	*/
		_list.DoLayoutList();

		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(180.0f));
		if (GUILayout.Button("Add"))
		{
			/*	Add default instance.	*/
			configurations.InsertArrayElementAtIndex(configurations.arraySize);
		}

		bool isItemSelected = selectedConfigIndex >= 0 && selectedConfigIndex < configurations.arraySize;

		EditorGUI.BeginDisabledGroup(!isItemSelected);
		if (GUILayout.Button("Add Copy"))
		{
			/*	Add copy based on tehcurrent selected.	*/
			var item = configurations.GetArrayElementAtIndex(selectedConfigIndex);
			var copy = item.Copy();
			configurations.InsertArrayElementAtIndex(item.arraySize);
			configurations.GetArrayElementAtIndex(item.arraySize - 1).managedReferenceValue = copy;
		}
		EditorGUI.EndDisabledGroup();

		EditorGUI.BeginDisabledGroup(!isItemSelected);
		if (GUILayout.Button("Remove"))
		{
			/*	Add copy based on tehcurrent selected.	*/
			configurations.DeleteArrayElementAtIndex(selectedConfigIndex);
			selectedConfigIndex = selectedConfigIndex - 1;
			//configurations.InsertArrayElementAtIndex(item.arraySize);
		}
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		/*	End	the vertical left view for adding, selecting and removing build targets.	*/
	}

	public override void OnGUI(string searchContext)
	{
		m_BuilderConfigSettings.Update();
		BuilderConfigSettings settings = (BuilderConfigSettings)m_BuilderConfigSettings.targetObject;

		this.scroll = EditorGUILayout.BeginScrollView(scroll);

		/*	Display global properties on all of the build targets.	*/
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(_outpdirectory, Styles.rootOutputDirectory);
		if (GUILayout.Button("Choose Path"))
		{
			string path = EditorUtility.OpenFolderPanel("Choose Output Directory", _outpdirectory.stringValue, "");
			if (Directory.Exists(path))
			{
				_outpdirectory.stringValue = path;
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.PropertyField(_verbose, Styles.Verbose);

		EditorGUILayout.BeginHorizontal();

		DisplayLeftBuildTargets();

		//EditorGUILayout.Separator();


		/**/
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("");
		if (selectedConfigIndex >= 0)
		{
			configurations.GetArrayElementAtIndex(selectedConfigIndex);
			EditorGUILayout.PropertyField(configurations.GetArrayElementAtIndex(selectedConfigIndex));

			BuildConfigTarget optionItem = BuilderConfigSettings.GetOrCreateSettings().options[selectedConfigIndex];
			EditorGUILayout.BeginHorizontal();
			bool isTargetSupported = Builder.isBuildTargetSupported(optionItem);
			EditorGUI.BeginDisabledGroup(!isTargetSupported);
			if (GUILayout.Button("Build"))
			{  //TODO add ICON
				Builder.BuildTarget(optionItem);
			}
			EditorGUILayout.LabelField(Builder.GetTargetLocationAbsolutePath(optionItem));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();

		/*	Summary information.	*/
		EditorGUILayout.LabelField("Number of targets: " + settings.options.Length.ToString());

		if (m_BuilderConfigSettings.hasModifiedProperties)
		{

		}
		m_BuilderConfigSettings.ApplyModifiedProperties();
	}

#if UNITY_2018_1_OR_NEWER
	[SettingsProvider]
	public static SettingsProvider CreateBuildConfigurationSettingsProvider()
	{
		if (!IsSettingsAvailable())
		{
			BuilderConfigSettings.GetOrCreateSettings();
		}
		SettingsProvider provider = new BuilderSettingsProvider("Project/Build Setting Configuration", SettingsScope.Project);

		provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
		return provider;
	}
#endif
}