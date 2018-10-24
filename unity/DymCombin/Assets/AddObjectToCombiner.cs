using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObjectToCombiner : MonoBehaviour {

	public CombineDymMesh cbr;
	// Use this for initialization
	void Start () {
 
		MeshFilter [] fs = GetComponentsInChildren<MeshFilter> ();
		for (int i = 0; i < fs.Length; i++) {
			cbr.AddMesh (fs[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
