using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PubnubApi.Unity.Internal.EditorTools {
	[CustomEditor(typeof(PNConfigAsset))]
	public class PNConfigAssetEditor : Editor {
		private readonly string[] propNames = new[] {
			"PublishKey",
			"SubscribeKey",
			"SecretKey",
			"AuthKey",
			"CipherKey",
			"Secure",
			"EnableWebGLBuildMode",
			"LogToUnityConsole",
			"LogVerbosity"
		};

		private SerializedProperty externalJsonEnabled;
		private SerializedProperty externalJsonFile;
		private SerializedProperty publishKey;
		private SerializedProperty privateKey;

		private IEnumerable<SerializedProperty> props;

		private void OnEnable() {
			externalJsonEnabled = serializedObject.FindProperty("externalJsonEnabled");
			externalJsonFile = serializedObject.FindProperty("externalJsonFile");
			publishKey = serializedObject.FindProperty("PublishKey");
			privateKey = serializedObject.FindProperty("SubscribeKey");
		}

		public override void OnInspectorGUI() {
			props ??= propNames.Select(p => serializedObject.FindProperty(p));

			serializedObject.Update();

			// external file handling

			// TODO implement external source
			// EditorGUILayout.BeginVertical("helpbox");
			// EditorGUILayout.PropertyField(externalJsonEnabled,
			// 	new GUIContent("Use external key config"));
			// ExternalFileGui();
			// EditorGUILayout.EndVertical();


			// UserId info
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Note that you need to set the UserId variable on runtime, before passing the configuration object to the PubNub instance.", MessageType.Warning);
			EditorGUILayout.Space();

			EditorGUILayout.Space();

			// props
			foreach (var prop in props) {
				EditorGUILayout.PropertyField(prop);
			}

			// Demo keyset info
			if (publishKey.stringValue == "demo" || privateKey.stringValue == "demo") {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox(
					"The demo keyset has limited functionality, including some debounce between calls. Please consider switching to a production key.",
					MessageType.Warning);
				EditorGUILayout.Space();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void ExternalFileGui() {
			if (!externalJsonEnabled.boolValue || targets.Length > 1) {
				return;
			}

			externalJsonFile.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("External keyset file"), externalJsonFile.objectReferenceValue, typeof(TextAsset), target);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Browse")) {
				EditorGUIUtility.ShowObjectPicker<TextAsset>(externalJsonFile.objectReferenceValue, false, "ext:json", EditorGUIUtility.GetObjectPickerControlID());
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}