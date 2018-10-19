using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore;

[ExecuteInEditMode]
public class MakeRSMTarget : MonoBehaviour {


	public int targetWidth = 256;
	readonly int _G_WorldSpaceLightDir = Shader.PropertyToID("_G_WorldSpaceLightDir");
	readonly int WorldToLight = Shader.PropertyToID("_WorldToLight");
	readonly int WorldToLightInv = Shader.PropertyToID("_WorldToLightInv");
	readonly int ShadowTex = Shader.PropertyToID("_ShadowTex");
	readonly int _InvPower = Shader.PropertyToID("_InvPower");
	readonly int _RSM_LIGHT = Shader.PropertyToID("_RSM_LIGHT");
	readonly int _RSM_NORMAL = Shader.PropertyToID("_RSM_NORMAL");

	public RenderTexture[] targets;
	readonly static int BufferCount = 3;
	private RenderTexture[] mrtTexs = null;
	private RenderBuffer [] colorBuffers = null;
	public Camera cam;

	[Slider("强度控制",0,1)]
	public float InvPower = 0.2f;
	//[Slider("最大深度差",0.00001f,0.0001f)]
	//public float mad_depth_delta = 0.00002f ;
	// Use this for initialization
	void init_texture()
	{
		if (null != mrtTexs) 
			return;
		mrtTexs = new RenderTexture[BufferCount];
		colorBuffers = new RenderBuffer[BufferCount];
		cam.SetReplacementShader (Shader.Find("Test/RSM"),null);
		for (int i = 0; i < BufferCount; i++) {
			if(i==0)
				mrtTexs [i] = new RenderTexture (targetWidth, targetWidth, 24, RenderTextureFormat.BGRA32);
			else
				mrtTexs [i] = new RenderTexture (targetWidth, targetWidth, 0, RenderTextureFormat.BGRA32);
			colorBuffers [i] = mrtTexs [i].colorBuffer;
		}
		cam.SetTargetBuffers (colorBuffers, mrtTexs [0].depthBuffer);
	}
	void Start () {
		
			
		if (null == cam) {
			cam = GetComponent<Camera> ();
		}
		if (null == cam)
			return;
		
	
	}
 
	// Update is called once per frame
	void Update () {
		init_texture ();
		targets = mrtTexs;
		Matrix4x4 V = cam.worldToCameraMatrix;
		Matrix4x4 P = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
		Matrix4x4 VP = P * V;


		Matrix4x4 viewMat = cam.worldToCameraMatrix;
		Matrix4x4 projMat = GL.GetGPUProjectionMatrix( cam.projectionMatrix, false );
		Matrix4x4 viewProjMat = (projMat * viewMat);          
		 
         
		Shader.SetGlobalVector(_G_WorldSpaceLightDir, cam.transform.forward.normalized);
		Shader.SetGlobalFloat (_InvPower, InvPower);
		Shader.SetGlobalMatrix(WorldToLight, VP);
		//Shader.SetGlobalMatrix(WorldToLightInv, VP.inverse);
		 
		Shader.SetGlobalMatrix(WorldToLightInv, VP.inverse);       
		Shader.SetGlobalTexture (ShadowTex, mrtTexs[1]);
		Shader.SetGlobalTexture (_RSM_LIGHT, mrtTexs[0]);
		Shader.SetGlobalTexture (_RSM_NORMAL, mrtTexs[2]);

	}
}
