using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuilderConfiguration // TOOD rename and add for the rest.
{ }
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
		public static GUIContent choosePath = new GUIContent("Choose Path", "Choose the output directory for where all the target will be save to.");

		public static GUIContent build = new GUIContent("Build", "Build target.");
		public static GUIContent run = new GUIContent("Run", "");
		/*	*/
		public static GUIContent export = new GUIContent("Export", "");
		public static GUIContent import = new GUIContent("Import", "");
		public static GUIContent add = new GUIContent("Add", "Add additional target.");
		public static GUIContent addCopy = new GUIContent("Add Copy", "Add additional as a copy of the selected.");
		public static GUIContent remove = new GUIContent("Remove", "Remove selected target.");
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
		return (float)EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 1.5f;
	}

	private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		var item = configurations.GetArrayElementAtIndex(index);

		int textWidth = 140;
		int toggleWidth = 15;
		int characterWidth = 10;

		/*	*/
		SerializedProperty name = item.FindPropertyRelative("name");
		SerializedProperty enabled = item.FindPropertyRelative("enabled");

		string displayName = "Name:";

		/*	*/
		Rect enabledRect = new Rect(rect.x + 0, rect.y, toggleWidth * 2, EditorGUIUtility.singleLineHeight);
		Rect displayNameRect = new Rect(rect.x + toggleWidth, rect.y, displayName.Length * characterWidth, EditorGUIUtility.singleLineHeight);
		Rect nameRect = new Rect(rect.x + toggleWidth + displayName.Length * characterWidth, rect.y, textWidth, EditorGUIUtility.singleLineHeight);


		EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("", "Enable target for building."));
		EditorGUI.LabelField(displayNameRect, displayName);
		EditorGUI.PropertyField(nameRect, name, new GUIContent(""));

		/*	If item selected.	*/
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
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(230.0f));

		/*	*/
		_list.DoLayoutList();

		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(180.0f));
		if (GUILayout.Button(Styles.add))
		{
			/*	Add default instance.	*/
			configurations.InsertArrayElementAtIndex(configurations.arraySize);
		}

		/*	Determine if any valid item is currently selected.	*/
		bool isItemSelected = selectedConfigIndex >= 0 && selectedConfigIndex < configurations.arraySize;

		EditorGUI.BeginDisabledGroup(!isItemSelected);
		if (GUILayout.Button(Styles.addCopy))
		{
			/*	Add copy based on tehcurrent selected.	*/
			SerializedProperty item = configurations.GetArrayElementAtIndex(selectedConfigIndex);
			SerializedProperty copy = item.Copy();
			configurations.InsertArrayElementAtIndex(item.arraySize);
			//configurations.GetArrayElementAtIndex(item.arraySize).managedReferenceValue = copy.referenceValue;
		}
		EditorGUI.EndDisabledGroup();

		EditorGUI.BeginDisabledGroup(!isItemSelected);
		if (GUILayout.Button(Styles.remove))
		{
			/*	Add copy based on tehcurrent selected.	*/
			configurations.DeleteArrayElementAtIndex(selectedConfigIndex);
			selectedConfigIndex = selectedConfigIndex - 1;
		}
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		/*	End	the vertical left view for adding, selecting and removing build targets.	*/
	}

	private void DisplayRunList()
	{
		EditorGUILayout.BeginVertical();

		/*	Iterate through each item.	*/
		foreach (SerializedProperty item in m_BuilderConfigSettings.FindProperty("options"))
		{
			BUildConfigTarget target = (BUildConfigTarget)item.objectReferenceValue;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(target.title);
			EditorGUI.BeginDisabledGroup(!Builder.IsTargetRunable(target));
			if(GUILayout.Button(Styles.run)){
				try{
					Builder.RunTarget(item);
				}catch(Exception ex){
					EditorUtility.DisplayDialog(string.Format("Failed Running '{}'", target.name), "", "OK");
				}

			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}

	private void DisplayGUIHeader()
	{
		/*	Display global properties on all of the build targets.	*/
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(_outpdirectory, Styles.rootOutputDirectory);
		if (GUILayout.Button(Styles.choosePath))
		{
			string path = EditorUtility.OpenFolderPanel("Choose Output Directory", _outpdirectory.stringValue, "");
			if (Directory.Exists(path))
			{
				_outpdirectory.stringValue = path;
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.PropertyField(_verbose, Styles.Verbose);
	}

	public override void OnGUI(string searchContext)
	{

		m_BuilderConfigSettings.Update();
		BuilderConfigSettings settings = (BuilderConfigSettings)m_BuilderConfigSettings.targetObject;

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		this.scroll = EditorGUILayout.BeginScrollView(scroll);

		/*	*/
		DisplayGUIHeader();

		EditorGUILayout.Space();

		/*	*/
		EditorGUILayout.BeginHorizontal();

		DisplayLeftBuildTargets();

		EditorGUILayout.Separator();

		/**/
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("");
		/*	Displace the build target if selected and is a valid index.	*/
		if (selectedConfigIndex >= 0 && selectedConfigIndex < settings.options.Length)
		{
			/*	*/
			//configurations.GetArrayElementAtIndex(selectedConfigIndex);
			EditorGUILayout.PropertyField(configurations.GetArrayElementAtIndex(selectedConfigIndex), new GUIContent(""));

			BuildConfigTarget optionItem = BuilderConfigSettings.GetOrCreateSettings().options[selectedConfigIndex];
			EditorGUILayout.BeginHorizontal();
			bool isTargetSupported = Builder.isBuildTargetSupported(optionItem);
			EditorGUI.BeginDisabledGroup(!isTargetSupported);
			if (GUILayout.Button(Styles.build))
			{  //TODO add ICON
				Builder.BuildTarget(optionItem);
			}
			EditorGUILayout.LabelField(Builder.GetTargetLocationAbsolutePath(optionItem));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		DisplayRunList();

		EditorGUILayout.EndScrollView();

		/*	Summary information.	*/
		EditorGUILayout.LabelField("Number of targets: " + settings.options.Length.ToString());

		EditorGUILayout.BeginHorizontal();
		string ext = ".bcn";    //TODO relocate.
		if (GUILayout.Button(Styles.export))
		{
			/*	Export.	*/
			string path = EditorUtility.SaveFilePanel("Choose export file path", Directory.GetCurrentDirectory(), "Application.PruductName" + ext, ext);
			/*	*/
			if (path.Length != 0)
			{

			}
		}
		if (GUILayout.Button(Styles.import))
		{
			/*	Import.	*/
			string path = EditorUtility.OpenFilePanel("Choose import file path", Directory.GetCurrentDirectory(), ext);
			/*	*/
			if (path.Length != 0)
			{
				/*	Determine if valid file.	*/
				//EditorUtility.DisplayDialog("Select Texture", "You must select a texture first!", "OK");
				BuilderConfigSettings.loadFromPath(path);
			}
		}
		EditorGUILayout.EndHorizontal();
		//TODO improve.

		if (m_BuilderConfigSettings.hasModifiedProperties)
		{

		}
		m_BuilderConfigSettings.ApplyModifiedProperties();


		EditorGUI.indentLevel = indent;
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