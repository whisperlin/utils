using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlobalDecal : MonoBehaviour {

	Camera cam;
	public Texture2D tex;
	int _WorldToGlobalDecal;
	int _GlobalDecalTex;
	// Use this for initialization
	void Start () {
		 _WorldToGlobalDecal = Shader.PropertyToID("WorldToGlobalDecal");
		 _GlobalDecalTex = Shader.PropertyToID("GlobalDecalTex");
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (null != cam) {
			Matrix4x4 V = cam.worldToCameraMatrix;
			Matrix4x4 P = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
			Matrix4x4 VP = P * V;
			Shader.SetGlobalMatrix(_WorldToGlobalDecal, VP);
		}
		if (null != tex) {
			Shader.SetGlobalTexture (_GlobalDecalTex, tex);
		}



	}
}
