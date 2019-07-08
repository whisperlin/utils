using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[ExecuteInEditMode]
public class MakeVSMTarget : MonoBehaviour {

	//readonly int WorldToLight = Shader.PropertyToID("_WorldToLight");
	//readonly int ShadowTex = Shader.PropertyToID("_ShadowTex");
	//readonly int VSMPower = Shader.PropertyToID("_MaxDepthDelta");
	public RenderTexture target;
	public Camera cam;
	[Range(0.00001f,0.0001f)]
	 
	public float mad_depth_delta = 0.00002f ;
	// Use this for initialization
	void Start () {
		
	}
 
	// Update is called once per frame
	void Update () {
		if (null == cam) {
			cam = GetComponent<Camera> ();
		}
		if (null == cam)
			return;
		cam.SetReplacementShader (Shader.Find("Test/VSM"),null);
		bool d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
 
		//Matrix4x4  V = cam.worldToCameraMatrix;
		//Matrix4x4 P = cam.projectionMatrix;
		//Matrix4x4 VP = P*V;


		Matrix4x4 V = cam.worldToCameraMatrix;
		Matrix4x4 P = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
		Matrix4x4 VP = P * V;

		//MVP = P*V*M;

		Shader.SetGlobalTexture ("_ShadowTex", target);
 
		Shader.SetGlobalMatrix("_WorldToLight", VP);
		Shader.SetGlobalFloat("_MaxDepthDelta", mad_depth_delta);

		//Shader.SetGlobalMatrix(WorldToLight,  cam.worldToCameraMatrix*cam.projectionMatrix );
	}
}
