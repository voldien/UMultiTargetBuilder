using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(BuilderConfigSettings))]
public class BuilderConfigSettingsEditor : Editor
{
	SerializedProperty configurations;
	SerializedProperty rootOutputDirectory;
	SerializedProperty _verbose;
	Vector2 scroll = Vector2.zero;

	private void OnEnable()
	{
		configurations = serializedObject.FindProperty("options");
		_verbose = serializedObject.FindProperty("verbose");
		rootOutputDirectory = serializedObject.FindProperty("rootOutputDirectory");
	}

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Build Targets"))
		{
			try
			{
				Builder.BuildFromConfig((BuilderConfigSettings)configurations.objectReferenceValue);
			}
			catch (Exception ex)
			{

			}
		}
		GUILayout.Label("Number of targets: " + configurations.arraySize.ToString());
		//EditorGUILayout.PropertyField(rootOutputDirectory);
		EditorGUILayout.Separator();


		base.OnInspectorGUI();
		serializedObject.Update();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add"))
		{
			configurations.InsertArrayElementAtIndex(configurations.arraySize);
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();

		}
		if (GUILayout.Button("Copy Current"))
		{
			BuildTarget _target = EditorUserBuildSettings.activeBuildTarget;
			BuildTargetGroup _targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			configurations.InsertArrayElementAtIndex(configurations.arraySize);
			SerializedProperty rsef = configurations.GetArrayElementAtIndex(configurations.arraySize);
			//			EditorBuildSettings.AddConfigObject
			EditorUtility.SetDirty(serializedObject.targetObject);
		}

		EditorGUILayout.EndHorizontal();

		scroll = EditorGUILayout.BeginScrollView(scroll);
		EditorGUILayout.BeginVertical();
		if (configurations.hasChildren)
		{
			for (int i = 0; i < configurations.arraySize; i++)
			{
				SerializedProperty option = configurations.GetArrayElementAtIndex(i);

				SerializedProperty buildOption = option.FindPropertyRelative("buildPlayerOptions");
				SerializedProperty buildTarget = option.FindPropertyRelative("target");
				BuildConfigTarget optionItem = ((BuilderConfigSettings)serializedObject.targetObject).options[i];

				option.GetHashCode();
				bool enabled = option.FindPropertyRelative("enabled").boolValue;
				bool Foldout = option.FindPropertyRelative("Foldout").boolValue;

				Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(Foldout, new GUIContent(optionItem.name));
				option.FindPropertyRelative("Foldout").boolValue = Foldout;

				EditorGUI.BeginDisabledGroup(!enabled);  //TODO negate

				EditorGUILayout.BeginHorizontal();
				/*	Draw build option configuration.	*/
				EditorGUILayout.PropertyField(option, new GUIContent("Build Config Options")); // buildTarget.enumDisplayNames[buildTarget.enumValueIndex]
				EditorGUILayout.EndHorizontal();

				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Remove"))
				{ // TODO ADD icon
					this.configurations.DeleteArrayElementAtIndex(i);
					/*	Break out of the iteraton.	*/
					break;
				}

				bool isTargetSupported = Builder.isBuildTargetSupported(optionItem);
				EditorGUI.BeginDisabledGroup(!isTargetSupported);
				if (GUILayout.Button("Build"))
				{  //TODO add ICON
					Builder.BuildTarget(optionItem);

					EditorGUI.ProgressBar(Rect.zero, 0, "");
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndFoldoutHeaderGroup();


				EditorGUILayout.Space();
				/*	*/
				EditorGUILayout.BeginHorizontal();
				if (Builder.isBuildTargetSupported(optionItem))
				{
					//EditorGUIUtility.IconContent("SettingsIcon")
					GUILayout.Label(EditorGUIUtility.IconContent("SettingsIcon"), GUILayout.MaxWidth(16));
					GUILayout.Label("Supported target");
				}
				else
				{
					GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
					//Value Color
					//TextFieldStyles.normal.textColor = Color.red;

					//Label Color 

					GUILayout.Label(EditorGUIUtility.IconContent("sv_icon_dot4_pix16_gizmo"), GUILayout.MinWidth(16), GUILayout.MaxWidth(32));
					Color currentColor = EditorStyles.label.normal.textColor;
					EditorStyles.label.normal.textColor = Color.red;
					GUILayout.Label("Non Supported target");
					EditorStyles.label.normal.textColor = currentColor;
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Separator();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();

		serializedObject.ApplyModifiedProperties();
	}

	public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
	{
		Texture2D tex = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
		return EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
	}

	//EditorGUILayout.BeginBuildTargetSelectionGrouping

	public static void CreateAsset<BuilderConfig>() where BuilderConfig : ScriptableObject
	{
		BuilderConfig asset = ScriptableObject.CreateInstance<BuilderConfig>();

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);

		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		/**/
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(BuilderConfig).ToString() + ".asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

}