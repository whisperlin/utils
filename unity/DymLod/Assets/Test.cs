using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	MeshFilter mf;
	public Mesh baseMesh;
	public Mesh curMesh;
	public bool highQuality  = true;
	int face = 0;
	// Use this for initialization
	void Start () {
		mf = GetComponent<MeshFilter> ();
		baseMesh = mf.sharedMesh;
		curMesh = new Mesh ();
		face = baseMesh.triangles.Length/3;
	}

	public static void MeshCopy(Mesh desc, Mesh src)
	{
		desc.Clear ();
		desc.vertices = src.vertices;
		desc.triangles = src.triangles;
		desc.uv = src.uv;
		desc.normals = src.normals;
		desc.colors = src.colors;
		if (src.uv2.Length>0)
			desc.uv2 = src.uv2;
		if (src.uv3.Length>0)
			desc.uv3 = src.uv3;
		if (src.uv4.Length>0)
			desc.uv4 = src.uv4;
		desc.tangents = src.tangents;

	}

	public float hSliderValue = 1.0F;
	void OnGUI() {
		float old = hSliderValue;
		hSliderValue = GUI.HorizontalSlider(new Rect(25, 25, 400, 30), hSliderValue, 0.01F, 1.0F);
		GUI.Label (new Rect (25, 125, 300, 30), "面数："+face);		
		GUI.Label (new Rect (25, 225, 300, 30), "hSliderValue："+hSliderValue);	
		if (old != hSliderValue) {
			MeshCopy (curMesh,baseMesh);
			KrablMeshUtility.SimplifyMesh (curMesh, (int)(hSliderValue*curMesh.triangles.Length/3), highQuality);
			mf.sharedMesh = curMesh;
			face = curMesh.triangles.Length/3;
		
		}
	}
	// Update is called once per frame
	void Update () {
		

		
	}
}
