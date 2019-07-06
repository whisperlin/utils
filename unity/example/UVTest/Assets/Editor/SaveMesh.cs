using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SaveMesh : MonoBehaviour {
	[MenuItem("TA/Create A Mesh")]
	// Use this for initialization
	static void Start () {
		var mesh = new Mesh();
		mesh.vertices = new Vector3[] { new Vector3(-1 ,-1,0), new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1,0) };
		mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
		mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.RecalculateNormals();

		string path = EditorUtility.SaveFilePanelInProject ("save", "m", "asset", "save");
		if (path.Length > 0) {
			AssetDatabase.CreateAsset(mesh,path);
		}
	}
	

}
