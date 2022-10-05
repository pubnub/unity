using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class PubNubPackageExporter : MonoBehaviour {

	[MenuItem("Assets/Export PubNub Package")]
	public static void ExportPackage() {
		bool AssetTestsFilter(string s) => !s.StartsWith("Assets/PubNub/PlayModeTests");

		var assets = AssetDatabase.FindAssets("", new[] { "Assets/PubNub" }).Select(AssetDatabase.GUIDToAssetPath).Where((System.Func<string, bool>)AssetTestsFilter).ToArray();

		Debug.Log("Folders to be exported:\n" + string.Join(", ", assets));

		AssetDatabase.ExportPackage(assets, "PubNub.unitypackage", ExportPackageOptions.Recurse);
	}
}