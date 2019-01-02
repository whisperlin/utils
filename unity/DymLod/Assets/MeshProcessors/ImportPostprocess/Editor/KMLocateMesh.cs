using UnityEngine;
using UnityEditor;

public class KMLocateMesh : ScriptableObject {
    [MenuItem ("GameObject/Inspect Mesh Importer for selected Mesh %#m")]
	static void LocateMesh() {
		GameObject go = Selection.activeGameObject;
		if (go == null) {
			return;
		}
		// Find the first mesh in the GameObject.
		Mesh mesh = null;
		MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
		if (mf != null) { 
			mesh = mf.sharedMesh;
		} else {
			SkinnedMeshRenderer smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
			if (smr != null) {
				mesh = smr.sharedMesh;
			}
		}
		
		if (mesh != null) {
			Selection.objects = new Object[]{mesh};
		}
	}
}
