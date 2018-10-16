using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDepthBuffer : MonoBehaviour {
     
	public Camera camera;

    void Start()
    {
		if (null == camera)
		{
			camera = Camera.main;
		}
        camera.depthTextureMode |= DepthTextureMode.Depth;
    }
    
}
