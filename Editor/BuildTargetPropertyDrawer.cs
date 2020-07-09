using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BuildMultiPlatform
{
	[CustomPropertyDrawer(typeof(BuildTarget))]
	public class BuildTargetPropertyDrawer : PropertyDrawer
	{
		public class Styles
		{
			/*	*/
			public static GUIContent metaInformation = new GUIContent("Meta Information", "");
			public static GUIContent name = new GUIContent("Target name", "Name of the target in the project setting.");
			public static GUIContent title = new GUIContent("Title", "Title of the game, if empty the default title from the project player settings will be used.");
			public static GUIContent output = new GUIContent("Relative Directory", "Relative path from the global path.");
			public static GUIContent BuildSettingLabel = new GUIContent("Build Settings", "");
			public static GUIContent target = new GUIContent("Target", "Specific group target.");
			public static GUIContent targeGroup = new GUIContent("Target Group", "Target group.");
			public static GUIContent optionFlagsLabel = new GUIContent("Option Flags", "");

			/*	Options Flags.	*/
			public static GUIContent development = new GUIContent("Development", "");
			public static GUIContent strict = new GUIContent("Strict", "No error or warnings allowed.");
			public static GUIContent CRC = new GUIContent("Compute CRC", "");
			public static GUIContent AllowDebugging = new GUIContent("Allow Debugging", "");
			public static GUIContent UncompressedAssetBundle = new GUIContent("Uncompressed AssetBundle", "");
			internal static GUIContent CompressWithLz4 = new GUIContent("Compress With Lz4", "");
			internal static GUIContent EnabledHeadlessMode = new GUIContent("Headless Mode", "");
			internal static readonly GUIContent CompressWithLz4HC = new GUIContent("Compress With Lz4HC", "");
			internal static readonly GUIContent IncludeTestAssemblies = new GUIContent("Include Test Assemblies", "");
			internal static readonly GUIContent EnableCodeCoverage = new GUIContent("Enable Code Coverage", "");
			internal static readonly GUIContent WaitForPlayerConnection = new GUIContent("Wait For Player Connection", "");
			internal static readonly GUIContent EnableDeepProfilingSupport = new GUIContent("Enable Deep Profiling Support", "");
			/*	Scene options.	*/
			public static GUIContent SceneSettingsLabel = new GUIContent("Scene Settings");
			public static GUIContent useScene = new GUIContent("Use Default Scenes");
		}
		const int listLineCount = 2;
		const int toggleOptionlineCount = 6;    /*	Toggle grid number of lines.	*/
		const int attribueLines = 11;	/*	Number of attribues lines.	*/
		const int lineCount = attribueLines + toggleOptionlineCount + listLineCount;
		private ReorderableList m_list;
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			/*	Compute the height of all the fields.	*/
			float height = EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1) * 1.25f;

			switch ((BuildTargetGroup)property.FindPropertyRelative("targetGroup").intValue)
			{
				case BuildTargetGroup.Standalone:
					break;
				case BuildTargetGroup.Android:
					break;
			}

			/*	Add additional height for the dynamic sized scene object.	*/
			SerializedProperty scenes = property.FindPropertyRelative("scenes");
			height += scenes.arraySize * EditorGUIUtility.singleLineHeight * 1.00f;
			return height;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int nthRow = 0;
			int nthCol = 0;
			int nrCol = 2;

			EditorGUI.BeginProperty(position, label, property);

			/*	*/
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;

			/*	Extract all properties.	*/
			SerializedProperty enabled = property.FindPropertyRelative("enabled");
			SerializedProperty name = property.FindPropertyRelative("name");
			SerializedProperty title = property.FindPropertyRelative("title");
			SerializedProperty outputDirectory = property.FindPropertyRelative("outputDirectory");
			/*	*/
			SerializedProperty _target = property.FindPropertyRelative("target");
			SerializedProperty targetGroup = property.FindPropertyRelative("targetGroup");
			SerializedProperty flags = property.FindPropertyRelative("options");
			SerializedProperty useDefaultScenes = property.FindPropertyRelative("useDefaultScenes");
			SerializedProperty scenes = property.FindPropertyRelative("scenes");

			/*	*/
			float propertyHeight = position.height / (float)lineCount;
			float defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			const float toggleTextWidth = 230.0f;

			/* Each property rectangle view bounds.	*/
			Rect metaRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect targetNameRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect titleRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect outputRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect settingLabelRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);

			/*	*/
			Rect targetGroupRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect targetRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect optionRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);

			/*	Settings options.	*/
			Rect optionDevelopmentRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionSrictRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			Rect optionCRCRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionAllowDebuggingRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			Rect optionUncompressedAssetBundleRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionEnableHeadlessModeRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			Rect optionCompressWithLz4Rect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionCompressWithLz4HCRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			Rect optionIncludeTestAssembliesRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionEnableCodeCoverageRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			Rect optionEnableDeepProfilingSupportRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			Rect optionWaitForPlayerConnectionRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, toggleTextWidth, defaultHeight);
			nthCol = (nthCol + 1) % nrCol;
			nthRow++;

			/*	TODO add additional options dependong on the target platform.	*/
			switch ((BuildTargetGroup)flags.intValue)
			{
				case BuildTargetGroup.Standalone:
					break;
				case BuildTargetGroup.Android:
					break;
			}

			/*	Scene options.	*/
			Rect sceneLabelRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			Rect useDefaultScenesRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
			float  sceneheight = scenes.arraySize * EditorGUIUtility.singleLineHeight * 1.00f;
			Rect ScenesRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, sceneheight);

			/*	*/
			EditorGUI.LabelField(metaRect, Styles.metaInformation, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			//TODO add label
			EditorGUI.PropertyField(targetNameRect, name, Styles.name);
			EditorGUI.PropertyField(titleRect, title, Styles.title);
			EditorGUILayout.Separator();
			EditorGUI.PropertyField(outputRect, outputDirectory, Styles.output);
			EditorGUI.indentLevel--;

			EditorGUI.LabelField(settingLabelRect, Styles.BuildSettingLabel, EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			EditorGUI.PropertyField(targetRect, _target, Styles.target);
			EditorGUI.PropertyField(targetGroupRect, targetGroup, Styles.targeGroup);
			EditorGUI.PropertyField(optionRect, flags, Styles.optionFlagsLabel);

			/*	*/
			InternalToogle(flags, BuildOptions.Development, EditorGUI.ToggleLeft(optionDevelopmentRect, Styles.development, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.Development)));
			InternalToogle(flags, BuildOptions.StrictMode, EditorGUI.ToggleLeft(optionSrictRect, Styles.strict, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.StrictMode)));
			/*	*/
			InternalToogle(flags, BuildOptions.ComputeCRC, EditorGUI.ToggleLeft(optionCRCRect, Styles.CRC, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.ComputeCRC)));
			InternalToogle(flags, BuildOptions.AllowDebugging, EditorGUI.ToggleLeft(optionAllowDebuggingRect, Styles.AllowDebugging, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.AllowDebugging)));
			/*	*/
			InternalToogle(flags, BuildOptions.UncompressedAssetBundle, EditorGUI.ToggleLeft(optionUncompressedAssetBundleRect, Styles.UncompressedAssetBundle, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.UncompressedAssetBundle)));
			InternalToogle(flags, BuildOptions.EnableHeadlessMode, EditorGUI.ToggleLeft(optionEnableHeadlessModeRect, Styles.EnabledHeadlessMode, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableHeadlessMode)));
			/*	*/
			InternalToogle(flags, BuildOptions.CompressWithLz4, EditorGUI.ToggleLeft(optionCompressWithLz4Rect, Styles.CompressWithLz4, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.CompressWithLz4)));
			InternalToogle(flags, BuildOptions.CompressWithLz4HC, EditorGUI.ToggleLeft(optionCompressWithLz4HCRect, Styles.CompressWithLz4HC, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.CompressWithLz4HC)));
			/*	*/
			InternalToogle(flags, BuildOptions.IncludeTestAssemblies, EditorGUI.ToggleLeft(optionIncludeTestAssembliesRect, Styles.IncludeTestAssemblies, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.IncludeTestAssemblies)));
			InternalToogle(flags, BuildOptions.EnableCodeCoverage, EditorGUI.ToggleLeft(optionEnableCodeCoverageRect, Styles.EnableCodeCoverage, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableCodeCoverage)));
			/*	*/
			InternalToogle(flags, BuildOptions.EnableDeepProfilingSupport, EditorGUI.ToggleLeft(optionEnableDeepProfilingSupportRect, Styles.EnableDeepProfilingSupport, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableDeepProfilingSupport)));
			InternalToogle(flags, BuildOptions.WaitForPlayerConnection, EditorGUI.ToggleLeft(optionWaitForPlayerConnectionRect, Styles.WaitForPlayerConnection, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.WaitForPlayerConnection)));

			EditorGUI.indentLevel--;

			EditorGUI.LabelField(sceneLabelRect, Styles.SceneSettingsLabel, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUI.PropertyField(useDefaultScenesRect, useDefaultScenes, Styles.useScene);

			EditorGUI.BeginDisabledGroup(useDefaultScenes.boolValue);

			/*	Draw scene property only.	*/
			/*	Update the list only if null or changed property.	*/
			if (m_list == null)
			{
				m_list = BuildSceneAssetReorderableList(scenes);
			}
			else if (m_list.serializedProperty != scenes)
			{
				m_list = BuildSceneAssetReorderableList(scenes);
			}
			ScenesRect = EditorGUI.IndentedRect(ScenesRect);
			m_list.DoList(ScenesRect);
			EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel--;

			/*	Reset indent.	*/
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
		static internal void InternalToogle(SerializedProperty target, BuildOptions flag, bool enabled)
		{
			if (enabled)
			{
				target.intValue = (int)(target.intValue | (int)flag);
			}
			else
			{
				target.intValue = (int)(target.intValue & ~((int)flag));
			}
		}

		private ReorderableList BuildSceneAssetReorderableList(SerializedProperty property)
		{
			ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);

			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, string.Format("Number of Scenes: {0}", property.arraySize));
			};
            list.elementHeightCallback = (int index) => {
				return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 1.5f;
			};
			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				SerializedProperty item = property.GetArrayElementAtIndex(index);
				string itemName = string.Format("Scene ({0}):", index);
				EditorGUI.PropertyField(rect, item, new GUIContent(itemName, "Scene Object"));
			};
			return list;
		}
	}


}