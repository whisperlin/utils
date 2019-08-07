using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class TestSkyBox : MonoBehaviour {

    public Cubemap cube;
    public Camera cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (null == cam)
        {
            cam = this.gameObject.AddComponent<Camera>();
            
        }

        if (null != cube)
            cam.RenderToCubemap(cube);


    }
}
