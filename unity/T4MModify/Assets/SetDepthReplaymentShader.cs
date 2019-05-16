using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SetDepthReplaymentShader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Camera>().SetReplacementShader(Shader.Find("TA/GetDepthFromOrthCamera"),null);
	}
}
