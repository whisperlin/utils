using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludeerBehind : MonoBehaviour {
    public Camera mCamera;
    public LayerMask layer0;
    public LayerMask layer1;

    public RenderTexture rt0;
    public RenderTexture rt1;

    public Shader shader;
    Camera mCamera1;
    Camera mCamera2;

    public Material mat;
    public bool close = false;

    public Material mat2;

    public GameObject g;
    // Use this for initialization
    void Start () {
        if (null == mCamera)
            mCamera = GetComponent<Camera>();
        mCamera1 = new GameObject("_Behind").AddComponent<Camera>();
        mCamera2 = new GameObject("_Occluder").AddComponent<Camera>();
        mCamera1.CopyFrom(mCamera);
        mCamera2.CopyFrom(mCamera);
        mCamera1.transform.parent = mCamera.transform;
        mCamera2.transform.parent = mCamera.transform;
        mCamera1.transform.localPosition = Vector3.zero;
        mCamera2.transform.localPosition = Vector3.zero;
        mCamera1.transform.rotation = Quaternion.identity;
        mCamera2.transform.rotation = Quaternion.identity;
        mCamera1.cullingMask = layer0;
        mCamera2.cullingMask = layer1;
        rt0 = new RenderTexture(Screen.width, Screen.height, 24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
        rt1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);

        mCamera1.targetTexture = rt0;
        mCamera2.targetTexture = rt1;
        mCamera1.clearFlags = CameraClearFlags.SolidColor;
        mCamera2.clearFlags = CameraClearFlags.SolidColor;
        mCamera1.backgroundColor = Color.black;
        mCamera2.backgroundColor = Color.black;
        mCamera1.SetReplacementShader(Shader.Find("Unlit/DepthNormal"), null);
        mCamera2.SetReplacementShader(Shader.Find("Unlit/DepthNormal"), null);
        if(null== shader)
            shader = Shader.Find("Hidden/OccluderBehind");
        mat = new Material(shader);
        mat.SetTexture("_Behind",rt0);
        mat.SetTexture("_Occluder", rt1);
        Shader.SetGlobalTexture("_Behind", rt0);
       // Shader.SetGlobalTexture("_Occluder", rt1);
    }
	
	// Update is called once per frame
	void Update () {

        

        if (g != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                g.SetActive(!g.activeSelf);
            }
        }


    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetTexture("_MainTex", source);
        if(close)
            Graphics.Blit(source, destination);
        else
        Graphics.Blit(source, destination, mat);
    
    }
}
