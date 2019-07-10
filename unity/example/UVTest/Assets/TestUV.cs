using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUV : MonoBehaviour {

	public Material mat;

	public Vector4 GlobalTestOffset;
	// Use this for initialization
	void Start () {

		var mesh = new Mesh();
		//mesh.vertices = new Vector3[] { new Vector3(-1 ,-1,0.5f), new Vector3(1, -1, 0.5f), new Vector3(-1, 1, 0.5f), new Vector3(1, 1,0.5f) };
		mesh.vertices = new Vector3[] { new Vector3(-1 ,-1,0.1f), new Vector3(1, -1, 0.1f), new Vector3(-1, 1, 0.1f), new Vector3(1, 1,0.1f) };
		mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
		mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.RecalculateNormals();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.bounds = new Bounds(Vector3.zero,new Vector3(1000,1000,1000));
		gameObject.AddComponent<MeshFilter> ().mesh = mesh;
		gameObject.AddComponent<MeshRenderer> ().material = mat;
			
		Shader.SetGlobalVector ("_testoffset",GlobalTestOffset);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
