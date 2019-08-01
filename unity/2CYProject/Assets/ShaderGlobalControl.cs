using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderGlobalControl : MonoBehaviour {
	[Range(0,1)]
	public float env = 0;
	public bool updateEvryFrame = true;

	// Use this for initialization
	void Start () {
		UpstaeParams ();
	}
	void UpstaeParams()
	{
		Shader.SetGlobalFloat ("_Env",env);;
	}
	// Update is called once per frame
	void Update () {
		if (updateEvryFrame) {
			UpstaeParams ();
		}
		
	}
}
