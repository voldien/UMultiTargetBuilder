using UnityEngine;
using UnityEditor;

namespace BuildMultiPlatform
{

	[CustomPropertyDrawer(typeof(BuildConfigTarget))]
	public class ConfigOptionItemPropertyDrawer : PropertyDrawer
	{

		const int toggleOptionlineCount = 5;
		const int lineCount = 10 + toggleOptionlineCount;
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			/*	Compute the height of all the fields.	*/
			float height = EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);

			switch((BuildTargetGroup)property.FindPropertyRelative("targetGroup").enumValue){
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
				//TODO determine the final height computation of the scene list.
				height += (scenes.arraySize + 1) * EditorGUIUtility.singleLineHeight;
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
			const float propertyHeight = position.height / (float)lineCount;
			const float textWidth = 80.0f;
			const float defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			const float toggleTextWidth = 90.0f;

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
			Rect optionDevelopmentRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionSrictRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			Rect optionCRCRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionAllowDebuggingRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			Rect optionUncompressedAssetBundleRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionEnableHeadlessModeRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			Rect optionCompressWithLz4Rect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionCompressWithLz4HCRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			Rect optionIncludeTestAssembliesRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionEnableCodeCoverageRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			Rect optionEnableDeepProfilingSupportRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;
			Rect optionWaitForPlayerConnectionRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
			nthCol = (nthCol+ 1) % nrCol;

			/*	TODO add additional options dependong on the target platform.	*/
			switch((BuildTargetGroup)flags.enumValue){
				case BuildTargetGroup.Standalone:
				break;
				case BuildTargetGroup.Android:
				break;
			}

			/*	Scene options.	*/
			Rect sceneLabelRect = new Rect(position.x, position.y + defaultHeight * 8, position.width, defaultHeight);
			Rect useDefaultScenesRect = new Rect(position.x, position.y + defaultHeight * 9, position.width, defaultHeight);
			Rect ScenesRect = new Rect(position.x, position.y + defaultHeight * 10, position.width, defaultHeight);

			//TODO compute the size of the scene.
			//Rect scenes = new Rect(targetRect.x, targetRect.y + nameRect.height, position.width, propertyHeight);

			//		EditorGUI.BeginChangeCheck();

			EditorGUI.LabelField(metaRect, new GUIContent("Meta Information"), EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			//TODO add label
			EditorGUI.PropertyField(titleRect, title, new GUIContent("Title"));
			EditorGUILayout.Separator();
			//EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("enabled"));
			EditorGUI.PropertyField(outputRect, outputDirectory, new GUIContent("outputDirectory"));
			EditorGUI.indentLevel--;

			EditorGUI.LabelField(settingLabelRect, new GUIContent("Build Settings"), EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			EditorGUI.PropertyField(targetRect, _target, new GUIContent("Target"));
			EditorGUI.PropertyField(targetGroupRect, targetGroup, new GUIContent("Target Group"));
			EditorGUI.PropertyField(optionRect, flags, new GUIContent("Option Flags"));

			//EditorGUI.ToggleLeft(optionRect, "", flags.intValue & 0x1 );

			EditorGUI.indentLevel--;

			EditorGUI.LabelField(sceneLabelRect, new GUIContent("Scene Settings"), EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUI.PropertyField(useDefaultScenesRect, useDefaultScenes, new GUIContent("Use Default Scenes"));
			if (!useDefaultScenes.boolValue)
			{
				/*	Draw each scenes property.	*/
				SerializedProperty scenes = property.FindPropertyRelative("scenes");
				EditorGUI.PropertyField(ScenesRect, scenes);

				//TODO move the logic for the property class for the scene.
				//TODO add button for copy the default.
				//TODO add 
			}

			EditorGUI.indentLevel--;

			//		if(EditorGUI.EndChangeCheck()){
			//			EditorUtility.SetDirty(property.serializedObject.targetObject);
			//		}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}