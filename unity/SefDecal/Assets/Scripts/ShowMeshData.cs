using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMeshData : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter>();
		Vector3[] vs = mf.mesh.vertices;
		for (int i = 0; i < vs.Length; i++)
		{
			Debug.Log(vs[i].x + " " + vs[i].y + " "+vs[i].z + " "); 
		} 		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
