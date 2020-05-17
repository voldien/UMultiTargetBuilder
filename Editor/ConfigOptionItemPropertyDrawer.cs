using UnityEngine;
using UnityEditor;

namespace BuildMultiPlatform
{
    //TODO rename
    [CustomPropertyDrawer(typeof(BuildTarget))]
    public class ConfigOptionItemPropertyDrawer : PropertyDrawer
    {
        public class Styles
        {
            public static GUIContent metaInformation = new GUIContent("Meta Information");
            public static GUIContent title = new GUIContent("Title");
            public static GUIContent output = new GUIContent("outputDirectory");
			public static GUIContent BuildSettingLabel = new GUIContent("Build Settings");
            public static GUIContent target = new GUIContent("Target");
            public static GUIContent targeGroup = new GUIContent("Target Group");
            public static GUIContent optionFlagsLabel = new GUIContent("Option Flags");

            /*	Options Flags.	*/
            public static GUIContent development = new GUIContent("Development", "");
            public static GUIContent strict = new GUIContent("Development", "");
            public static GUIContent CRC = new GUIContent("Development", "");
            public static GUIContent AllowDebugging = new GUIContent("Development", "");
            public static GUIContent UncompressedAssetBundle = new GUIContent("Development", "");
            internal static GUIContent CompressWithLz4 = new GUIContent("Development", "");
            internal static GUIContent EnabledHeadlessMode = new GUIContent("Development", "");
            internal static readonly GUIContent CompressWithLz4HC = new GUIContent("Development", "");
            internal static readonly GUIContent IncludeTestAssemblies = new GUIContent("Development", "");
            internal static readonly GUIContent EnableCodeCoverage = new GUIContent("Development", "");
            internal static readonly GUIContent WaitForPlayerConnection = new GUIContent("Development", "");
            internal static readonly GUIContent EnableDeepProfilingSupport = new GUIContent("Development", "");
			/*	Scene options.	*/
			public static GUIContent SceneSettingsLabel = new GUIContent("Scene Settings");
            public static GUIContent useScene = new GUIContent("Use Default Scenes");
        }
        const int toggleOptionlineCount = 5;
        const int lineCount = 10 + toggleOptionlineCount;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            /*	Compute the height of all the fields.	*/
            float height = EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);

            switch ((BuildTargetGroup)property.FindPropertyRelative("targetGroup").intValue)
            {
                case BuildTargetGroup.Standalone:
                    break;
                case BuildTargetGroup.Android:
                    break;
            }

            SerializedProperty useDefaultScenes = property.FindPropertyRelative("useDefaultScenes");
            if (!useDefaultScenes.boolValue)
            {
                /*	Add additional height for the dynamic sized scene object.	*/
                SerializedProperty scenes = property.FindPropertyRelative("scenes");
                height += EditorGUI.GetPropertyHeight(scenes);
            }
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
            SerializedProperty name = property.FindPropertyRelative("title");
            SerializedProperty title = property.FindPropertyRelative("title");
            SerializedProperty outputDirectory = property.FindPropertyRelative("outputDirectory");
            /*	*/
            SerializedProperty _target = property.FindPropertyRelative("target");
            SerializedProperty targetGroup = property.FindPropertyRelative("targetGroup");
            SerializedProperty flags = property.FindPropertyRelative("options");
            SerializedProperty useDefaultScenes = property.FindPropertyRelative("useDefaultScenes");

            /*	*/
            float propertyHeight = position.height / (float)lineCount;
            const float textWidth = 100.0f;
            float defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            const float toggleTextWidth = 120.0f;

            // Each property rectangle view bounds.
            Rect metaRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect titleRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            //			Rect nameRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect enabledRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect outputRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect settingLabelRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);

            /*	*/
            Rect targetRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect targetGroupRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);
            Rect optionRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);

            /*	Settings options.	*/
            Rect optionDevelopmentRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionSrictRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            nthRow++;

            Rect optionCRCRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionAllowDebuggingRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            nthRow++;

            Rect optionUncompressedAssetBundleRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionEnableHeadlessModeRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            nthRow++;

            Rect optionCompressWithLz4Rect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionCompressWithLz4HCRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            nthRow++;

            Rect optionIncludeTestAssembliesRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionEnableCodeCoverageRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            nthRow++;

            Rect optionEnableDeepProfilingSupportRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
            nthCol = (nthCol + 1) % nrCol;
            Rect optionWaitForPlayerConnectionRect = new Rect(position.x + nthCol * toggleTextWidth, position.y + defaultHeight * nthRow, position.width, defaultHeight);
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
            Rect ScenesRect = new Rect(position.x, position.y + defaultHeight * nthRow++, position.width, defaultHeight);

            //		EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(metaRect, Styles.metaInformation, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            //TODO add label
            EditorGUI.PropertyField(titleRect, title, Styles.title);
            EditorGUILayout.Separator();
            //EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("enabled"));
            EditorGUI.PropertyField(outputRect, outputDirectory, Styles.output);
            EditorGUI.indentLevel--;

            EditorGUI.LabelField(settingLabelRect, Styles.BuildSettingLabel, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(targetRect, _target, Styles.target);
            EditorGUI.PropertyField(targetGroupRect, targetGroup, Styles.targeGroup);
            EditorGUI.PropertyField(optionRect, flags, Styles.optionFlagsLabel);

            //EditorGUI.PrefixLabel()

            EditorGUI.ToggleLeft(optionDevelopmentRect, Styles.development, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.Development));
            EditorGUI.ToggleLeft(optionSrictRect, Styles.strict, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.StrictMode));
            /*	*/
            EditorGUI.ToggleLeft(optionCRCRect, Styles.CRC, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.ComputeCRC));
            EditorGUI.ToggleLeft(optionAllowDebuggingRect, Styles.AllowDebugging, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.AllowDebugging));
            /*	*/
            EditorGUI.ToggleLeft(optionUncompressedAssetBundleRect, Styles.UncompressedAssetBundle, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.UncompressedAssetBundle));
            EditorGUI.ToggleLeft(optionEnableHeadlessModeRect, Styles.EnabledHeadlessMode, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableHeadlessMode));
            /*	*/
            EditorGUI.ToggleLeft(optionCompressWithLz4Rect, Styles.CompressWithLz4, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.UncompressedAssetBundle));
            EditorGUI.ToggleLeft(optionCompressWithLz4HCRect, Styles.CompressWithLz4HC, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableHeadlessMode));
            /*	*/
            EditorGUI.ToggleLeft(optionIncludeTestAssembliesRect, Styles.IncludeTestAssemblies, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.UncompressedAssetBundle));
            EditorGUI.ToggleLeft(optionEnableCodeCoverageRect, Styles.EnableCodeCoverage, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableHeadlessMode));
            /*	*/
            EditorGUI.ToggleLeft(optionEnableDeepProfilingSupportRect, Styles.EnableDeepProfilingSupport, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.UncompressedAssetBundle));
            EditorGUI.ToggleLeft(optionWaitForPlayerConnectionRect, Styles.WaitForPlayerConnection, ((BuildOptions)flags.intValue).HasFlag(BuildOptions.EnableHeadlessMode));

            EditorGUI.indentLevel--;

            EditorGUI.LabelField(sceneLabelRect, Styles.SceneSettingsLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(useDefaultScenesRect, useDefaultScenes, Styles.useScene);
            if (!useDefaultScenes.boolValue)
            {
                /*	Draw scene property only if .	*/
                SerializedProperty scenes = property.FindPropertyRelative("scenes");
                EditorGUI.PropertyField(ScenesRect, scenes);
            }

            EditorGUI.indentLevel--;

            //		if(EditorGUI.EndChangeCheck()){
            //			EditorUtility.SetDirty(property.serializedObject.targetObject);
            //		}

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
		static internal void InternalToogle(BuildTarget target, BuildOptions flag, bool enabled)
		{
			if (enabled)
			{
				target.options = (BuildOptions)(target.options | flag);
			}
			else
			{
				target.options = (BuildOptions)(target.options & ~(flag));
			}
		}
    }


}