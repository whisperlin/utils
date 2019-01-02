using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(KMImportSettings))]
public class KMImportSettingsEditor : Editor {

	override public void OnInspectorGUI() {
		EditorGUILayout.HelpBox("This object holds all the information the Krabl mesh processor framework needs to import meshes.\n\n" +
			"DO NOT touch this unless you wish to loose your processor programs.\n\n",
			MessageType.Warning);
		
		if (KMImportSettings.DoDebug()) {
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Show Child Assets")) _showChildAssets(true);
			if (GUILayout.Button("Hide Child Assets")) _showChildAssets(false);
			GUILayout.EndHorizontal();
		}
	}
	
	void _showChildAssets(bool show) {
		HideFlags hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		if (show) hideFlags = HideFlags.None;
		
		string path = AssetDatabase.GetAssetPath(target);
		Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
		foreach (Object obj in objs) {
			if ((obj as ScriptableObject) != null && obj != target) {
				obj.hideFlags = hideFlags;

			}
		}
		AssetDatabase.SaveAssets();		
		AssetDatabase.ImportAsset(path);
	}
}
