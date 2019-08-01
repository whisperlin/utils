using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFellowCamera : MonoBehaviour {

    public Camera cam;
    public Transform target;
    public float distance = 10f;
    public float xRot = 45f;
    public float yRot = 30f;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {

        if (null == cam)
        {
            cam = Camera.main;
        }
        if (null == cam)
            return;
        if (null == target)
            return;


        if (null != target.parent)
        {
            target.parent = null;
        }
        cam.transform.position = target.position;
        cam.transform.rotation = Quaternion.identity;

        cam.transform.Rotate(Vector3.up, xRot );
        cam.transform.Rotate(Vector3.right, yRot);
        
        cam.transform.position = cam.transform.position - cam.transform.forward * distance;


    }
}
