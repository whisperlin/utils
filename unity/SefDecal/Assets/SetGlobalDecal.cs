using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlobalDecal : MonoBehaviour {

	Camera cam;
	public Texture2D tex;
	readonly int _WorldToGlobalDecal = Shader.PropertyToID("WorldToGlobalDecal");
	readonly int _GlobalDecalTex = Shader.PropertyToID("GlobalDecalTex");
	// Use this for initialization
	void Start () {
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
