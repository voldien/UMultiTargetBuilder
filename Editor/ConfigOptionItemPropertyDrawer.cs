using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BuildConfigTarget))]
public class ConfigOptionItemPropertyDrawer: PropertyDrawer {

	const int lineCount = 8;
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		/*	Compute the height of all the fields.	*/
		float height = EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);

		SerializedProperty useDefaultScenes = property.FindPropertyRelative("useDefaultScenes");
		if (!useDefaultScenes.boolValue)
		{
			/*	Add additional height for the dynamic sized scene object.	*/
			//height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("scenes"));
		}
		return height;
	}
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		EditorGUI.BeginProperty(position, label, property);

		/*	*/
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;


		/*	Extract all properties.	*/
		SerializedProperty enabled = property.FindPropertyRelative("enabled");
		SerializedProperty name = property.FindPropertyRelative("name");
		SerializedProperty outputDirectory = property.FindPropertyRelative("outputDirectory");
		
		SerializedProperty _target = property.FindPropertyRelative("target");
		SerializedProperty targetGroup = property.FindPropertyRelative("targetGroup");
		SerializedProperty flags = property.FindPropertyRelative("options");
		SerializedProperty useDefaultScenes = property.FindPropertyRelative("useDefaultScenes");

		float propertyHeight = position.height / (float)lineCount;

		float textWidth = 80.0f;
		float defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		// Each property rectangle view bounds.
		Rect nameRect = new Rect(position.x, position.y + defaultHeight * 0, position.width, defaultHeight);
		Rect enabledRect = new Rect(position.x, position.y + defaultHeight * 1, position.width, defaultHeight);
		Rect outputRect = new Rect(position.x, position.y + defaultHeight * 2, position.width, defaultHeight);
		Rect settingLabelRect = new Rect(position.x, position.y + defaultHeight * 3, position.width, defaultHeight);


		Rect targetRect = new Rect(position.x, position.y + defaultHeight * 4, position.width, defaultHeight);
		Rect targetGroupRect = new Rect(position.x, position.y + defaultHeight * 5, position.width, defaultHeight);
		Rect optionRect = new Rect(position.x, position.y + defaultHeight * 6, position.width, defaultHeight);
		Rect sceneLabelRect = new Rect(position.x, position.y + defaultHeight * 7, position.width, defaultHeight);
		Rect useDefaultScenesRect = new Rect(position.x, position.y + defaultHeight * 8, position.width, defaultHeight);

		//TODO compute the size of the scene.
		//Rect scenes = new Rect(targetRect.x, targetRect.y + nameRect.height, position.width, propertyHeight);

//		EditorGUI.BeginChangeCheck();

		EditorGUI.PropertyField(nameRect, name, new GUIContent("Name"));
		EditorGUI.PropertyField(enabledRect, enabled, new GUIContent("enabled"));
		EditorGUI.PropertyField(outputRect, outputDirectory, new GUIContent("outputDirectory"));

		EditorGUI.LabelField(settingLabelRect, new GUIContent("Build Settings"));

		EditorGUI.PropertyField(targetRect, _target, new GUIContent("Target"));
		EditorGUI.PropertyField(targetGroupRect, targetGroup, new GUIContent("Target Group"));
		EditorGUI.PropertyField(optionRect, flags, new GUIContent("Option Flags"));

		EditorGUI.LabelField(sceneLabelRect, new GUIContent("Scene Settings"));
		EditorGUI.PropertyField(useDefaultScenesRect, useDefaultScenes, new GUIContent("Use Default Scenes"));
		if(!useDefaultScenes.boolValue){
			/*	Draw each scenes property.	*/
		}

//		if(EditorGUI.EndChangeCheck()){
//			EditorUtility.SetDirty(property.serializedObject.targetObject);
//		}

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}