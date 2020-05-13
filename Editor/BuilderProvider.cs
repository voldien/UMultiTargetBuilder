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
    private SerializedProperty m_configurations;
    private SerializedProperty m_outpdirectory;
    private SerializedProperty m_verbose;
    private ReorderableList m_list;
    [NonSerialized]
    private int selectedConfigIndex = -1;

    public class Styles
    {
        public static GUIContent rootOutputDirectory = new GUIContent("Root output directory", "The root directory that all the target build will be stored.");
        public static GUIContent buildConfigs = new GUIContent("Build Configuration Set", "Set of build configuration.");
        public static GUIContent Verbose = new GUIContent("Verbose", "Output is verbosity.");
        public static GUIContent choosePath = new GUIContent("Choose Path", "Choose the output directory for where all the target will be save to.");

        public static GUIContent buildTargets = new GUIContent("Build Targets", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build all Targets.");
        public static GUIContent build = new GUIContent("Build", (Texture)EditorGUIUtility.IconContent("Settings").image, "Build target.");  //TODO add ICON EditorGUIUtility.IconContent("SettingsIcon")
        public static GUIContent run = new GUIContent("Run", "Run the target.");
        /*	*/
        public static GUIContent export = new GUIContent("Export", "Export ");
        public static GUIContent import = new GUIContent("Import", "Import ");
        public static GUIContent add = new GUIContent("Add", "Add additional target.");
        public static GUIContent addCopy = new GUIContent("Add Copy", "Add additional as a copy of the selected.");
        public static GUIContent remove = new GUIContent("Remove", "Remove selected target.");
    }

    public BuilderSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) { }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(BuilderConfigSettings.GetSettingFilePath());
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_BuilderConfigSettings = BuilderConfigSettings.GetSerializedSettings();

        /*	*/
        m_configurations = m_BuilderConfigSettings.FindProperty("options");
        m_outpdirectory = m_BuilderConfigSettings.FindProperty("rootOutputDirectory");
        m_verbose = m_BuilderConfigSettings.FindProperty("verbose");

        /*	Only create if the options variable found.	*/
        if (m_configurations != null)
        {

            /*	TODO validate.	*/
            m_list = new ReorderableList(m_BuilderConfigSettings, m_configurations, true, true, false, false)
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
        var item = m_configurations.GetArrayElementAtIndex(index);

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

        /*	*/
        EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("", "Enable target for building."));
        EditorGUI.LabelField(displayNameRect, displayName);
        EditorGUI.PropertyField(nameRect, name, new GUIContent(""));
        //TODO add build button.

        /*	If item selected.	*/
        if (isFocused && isActive)
            this.selectedConfigIndex = index;
    }

    private void DrawListHeader(Rect rect)
    {
        GUI.Label(rect, string.Format("Build Targets {0}", this.m_configurations.arraySize), EditorStyles.boldLabel);
    }

    public override void OnDeactivate()
    {

    }

    private void DisplayLeftBuildTargets()
    {
        /*	Begin the vertical left view for adding, selecting and removing build targets.	*/
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(230.0f));

        /*	*/
        m_list.DoLayoutList();

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(180.0f));
        if (GUILayout.Button(Styles.add))
        {
            /*	Add default instance.	*/
            int index = m_configurations.arraySize;
            m_configurations.InsertArrayElementAtIndex(m_configurations.arraySize);

            /*	Reset the values.	*/    //TODO add support for reseting the value.
                                          //SerializedProperty item = configurations.GetArrayElementAtIndex(index);
                                          //BuildConfigTarget _target = (BuildConfigTarget)item.serializedObject.targetObject;
        }

        /*	Determine if any valid item is currently selected.	*/
        bool isItemSelected = selectedConfigIndex >= 0 && selectedConfigIndex < m_configurations.arraySize;

        EditorGUI.BeginDisabledGroup(!isItemSelected);
        if (GUILayout.Button(Styles.addCopy))
        {
            /*	Add copy based on tehcurrent selected.	*/
            SerializedProperty item = m_configurations.GetArrayElementAtIndex(selectedConfigIndex);
            //TODO add support for copy the object.
            SerializedProperty copy = item.Copy();
            m_configurations.InsertArrayElementAtIndex(item.arraySize);
            //configurations.GetArrayElementAtIndex(item.arraySize).managedReferenceValue = copy.referenceValue;
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!isItemSelected);
        if (GUILayout.Button(Styles.remove))
        {
            /*	Add copy based on tehcurrent selected.	*/
            m_configurations.DeleteArrayElementAtIndex(selectedConfigIndex);
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
        BuilderConfigSettings settings = (BuilderConfigSettings)m_BuilderConfigSettings.targetObject;
        if (settings != null)
        {
            foreach (BuildConfigTarget target in settings.options)
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
                        EditorUtility.DisplayDialog(string.Format("Failed Running '{}'", target.name), "", "OK");
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
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(m_verbose, Styles.Verbose);
    }

    public override void OnGUI(string searchContext)
    {

        m_BuilderConfigSettings.Update();
        BuilderConfigSettings settings = (BuilderConfigSettings)m_BuilderConfigSettings.targetObject;

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        this.scroll = EditorGUILayout.BeginScrollView(scroll, false, true);

        /*	*/
        DisplayGUIHeader();

        EditorGUILayout.Space();

        /*	*/
        EditorGUILayout.BeginHorizontal();

        DisplayLeftBuildTargets();

        EditorGUILayout.Separator();

        /**/
        EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandHeight(true));
        EditorGUILayout.LabelField("");
        /*	Displace the build target if selected and is a valid index.	*/
        if (selectedConfigIndex >= 0 && selectedConfigIndex < settings.options.Length)
        {
            /*	*/
            //configurations.GetArrayElementAtIndex(selectedConfigIndex);
            EditorGUILayout.PropertyField(m_configurations.GetArrayElementAtIndex(selectedConfigIndex), new GUIContent(""));

            BuildConfigTarget optionItem = BuilderConfigSettings.GetOrCreateSettings().options[selectedConfigIndex];
            EditorGUILayout.BeginHorizontal();
            bool isTargetSupported = Builder.isBuildTargetSupported(optionItem);
            EditorGUI.BeginDisabledGroup(!isTargetSupported);
            if (GUILayout.Button(Styles.build))
            {
                Builder.BuildTarget(optionItem);
            }
            EditorGUILayout.LabelField(Builder.GetTargetLocationAbsolutePath(optionItem));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        DisplayRunList();


        EditorGUILayout.Separator();

        EditorGUILayout.EndScrollView();

        /*	Summary information.	*/
        EditorGUILayout.LabelField("Number of targets: " + settings.options.Length.ToString());

        EditorGUILayout.BeginHorizontal();
        string ext = ".bcn";    //TODO relocate.

        if (GUILayout.Button(Styles.buildTargets))
        {
            Builder.BuildFromConfig((BuilderConfigSettings)m_configurations.objectReferenceValue);
        }
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