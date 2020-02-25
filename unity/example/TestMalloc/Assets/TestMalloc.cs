using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMalloc : MonoBehaviour {

	bool b = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect(0,0,300,300),  b.ToString())) {
			if (b) {
				Shader.EnableKeyword ("S_OUTSIDE_TEST");
			} else {
				Shader.DisableKeyword ("S_OUTSIDE_TEST");
			}
			b = !b;                         
		}
	}
}
