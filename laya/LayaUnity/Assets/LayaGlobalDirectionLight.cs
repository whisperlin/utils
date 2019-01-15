using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class LayaGlobalDirectionLight: MonoBehaviour {

	Light light;
	static float [] temp = new float[6];
	static float [] temp3 = new float[3];

	public Color c;
	// Use this for initialization
	void Start () {
		light = GetComponent <Light>();
	}
	
	// Update is called once per frame
	void Update () {
		temp [0] = light.color.r;
		temp [1] = light.color.g;
		temp [2] = light.color.b;
		temp [3] = light.transform.forward.x;
		temp [4] = light.transform.forward.y;
		temp [5] = light.transform.forward.z;

		temp3 [0] = light.color.r;
		temp3 [1] = light.color.g;
		temp3 [2] = light.color.b;
		c = new Color (temp [0],temp [1],temp [2]);

		 
		Shader.SetGlobalVector ("u_DirectionLight.Color",light.color);
		Shader.SetGlobalVector ("u_DirectionLight.Direction",light.transform.forward);
 
		
	}
}
