using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotationBakeObj : MonoBehaviour {

	MaterialPropertyBlock block;
	MeshRenderer r;

	public GameObject other;
	public int debug;
	Vector4 rot = new Vector4();
	void Start () {
		r = GetComponent<MeshRenderer> ();
		block = new MaterialPropertyBlock ();

	}
 
	// Update is called once per frame
	void Update () {
		if (r == null) {
			r = GetComponent<MeshRenderer> ();
		}
		Vector3  forward =  transform.forward;
		forward = new Vector3 (forward.x, 0, forward.z);
		forward.Normalize ();
		float delta = 0f;

 
		if (forward.z >= 0 && forward.x >= 0) {
			rot.Set (0f, 0f,  0.5f, 0f);
			delta = Vector3.Dot (new Vector3 (1, 0, 0), forward); 
			//Debug.Log ("one "+forward.ToString() +" "+ delta);
			debug = 1;
		} else if (forward.z <= 0 && forward.x >= 0) {
			
			rot.Set (0f, 0.5f, 0.5f, 0f);
			delta = Vector3.Dot (new Vector3 (1, 0, 0), forward); 
			//Debug.Log ("tow "+forward.ToString() +" "+ delta);
			debug = 2;
		} else if (forward.z <= 0 && forward.x <= 0) {
 
			rot.Set (0f, 0.5f,0.5f, 0.5f);
			delta = Vector3.Dot (new Vector3 (-1, 0, 0), forward); 
			debug = 3;
		} 
		else 
		{
			rot.Set (0f,0f,0.5f, 0.5f);
			delta = Vector3.Dot (new Vector3 (-1, 0, 0), forward); 
			debug = 4;
		}
		//r.sharedMaterial.SetVector ("_rotation",rot);
		//r.sharedMaterial.SetFloat ("_rotDelta", delta);
		//block.SetFloat ("_rotDelta", delta);
		//block.SetVector ("_rotation",rot);
		//block.SetFloat ("_rotDelta", delta);
		//r.SetPropertyBlock (block);
		if (null != other) {
			other.transform.rotation = this.transform.rotation;
		}
	}
}
