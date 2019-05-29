using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class ImageEffectManager : MonoBehaviour {

    RenderTexture myRenderTexture;
    Camera camera = null;

	public MobileBloom bloom = new MobileBloom();

    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
        bloom.Init();
    }

     

    void OnPreRender()
    {
        myRenderTexture = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight, 16);
        myRenderTexture.filterMode = FilterMode.Trilinear;
        myRenderTexture.antiAliasing = 2;
        camera.targetTexture = myRenderTexture;
    }
    void OnPostRender()
    {
		 
        camera.targetTexture = null;
 
		bloom.OnRenderImage(myRenderTexture, myRenderTexture);

		Graphics.Blit(myRenderTexture, null as RenderTexture);
        RenderTexture.ReleaseTemporary(myRenderTexture);
    }
    void OnDestroy()
    {
        bloom.Release();
    }
}
