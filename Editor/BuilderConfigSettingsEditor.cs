using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace BuildMultiPlatform
{

	[CustomEditor(typeof(BuilderConfigSettings))]
	public class BuilderConfigSettingsEditor : Editor
	{
		SerializedProperty configurations;
		SerializedProperty rootOutputDirectory;
		SerializedProperty _verbose;
		Vector2 scroll = Vector2.zero;

		private void OnEnable()
		{
			configurations = serializedObject.FindProperty("targets");
			_verbose = serializedObject.FindProperty("verbose");
			rootOutputDirectory = serializedObject.FindProperty("rootOutputDirectory");
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button(BuilderSettingsProvider.Styles.buildTargets))
			{
				Builder.BuildFromConfig((BuilderConfigSettings)configurations.objectReferenceValue);
			}
			GUILayout.Label("Number of targets: " + configurations.arraySize.ToString());
			EditorGUILayout.Separator();

			base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(BuilderSettingsProvider.Styles.add))
			{
				configurations.InsertArrayElementAtIndex(configurations.arraySize);
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();

			}
			if (GUILayout.Button(BuilderSettingsProvider.Styles.addCopy))
			{
				UnityEditor.BuildTarget _target = EditorUserBuildSettings.activeBuildTarget;
				BuildTargetGroup _targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
				configurations.InsertArrayElementAtIndex(configurations.arraySize);
				SerializedProperty rsef = configurations.GetArrayElementAtIndex(configurations.arraySize);

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


					BuildTarget optionItem = ((BuilderConfigSettings)serializedObject.targetObject).targets[i];

					/*	Draw build option configuration.	*/
					EditorGUILayout.PropertyField(option, GUIContent.none); // buildTarget.enumDisplayNames[buildTarget.enumValueIndex]

					if (GUILayout.Button(BuilderSettingsProvider.Styles.remove))
					{
						this.configurations.DeleteArrayElementAtIndex(i);
						/*	Break out of the iteraton.	*/
						break;
					}

					bool isTargetSupported = Builder.isBuildTargetSupported(optionItem);
					EditorGUI.BeginDisabledGroup(!isTargetSupported);
					if (GUILayout.Button(BuilderSettingsProvider.Styles.build))
					{
						Builder.BuildTarget(optionItem);
					}

					EditorGUILayout.Separator();
					/*	*/
					EditorGUILayout.BeginHorizontal();
					if (Builder.isBuildTargetSupported(optionItem))
					{
						GUILayout.Label("Supported target");
					}
					else
					{
						GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);

						//						GUILayout.Label(EditorGUIUtility.IconContent("sv_icon_dot4_pix16_gizmo"), GUILayout.MinWidth(16), GUILayout.MaxWidth(32));
						//						Color currentColor = EditorStyles.label.normal.textColor;
						//						EditorStyles.label.normal.textColor = Color.red;
						GUILayout.Label("Non Supported target");
						//						EditorStyles.label.normal.textColor = currentColor;
					}

					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.Separator();

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

			serializedObject.ApplyModifiedProperties();
		}
	}
}